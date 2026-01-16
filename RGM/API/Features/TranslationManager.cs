using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using MEC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace RGM.API.Features
{
    /// <summary>
    /// Google Cloud Translation API (v2 REST) wrapper built for Exiled/MEC.
    /// - No Task/async
    /// - Request queue (rate friendly)
    /// - In-flight coalescing (same text/target shares a single API call)
    /// - LRU cache
    /// - Retry with exponential backoff on 429/403/5xx/network
    /// </summary>
    public static class TranslationManager
    {
        // ===== Public config =====
        public static bool IsEnabled { get; set; } = true;
        public static bool Debug { get; set; } = false;

        /// <summary>Google Cloud Translation API key.</summary>
        public static string ApiKey { get; set; } = string.Empty;

        /// <summary>Minimum delay between outbound requests (seconds).</summary>
        public static float MinInterval { get; set; } = 0.20f;

        /// <summary>Max retry count on transient failures (429/5xx/network).</summary>
        public static int MaxRetries { get; set; } = 2;

        /// <summary>Base delay for backoff (seconds).</summary>
        public static float RateLimitBackoffBase { get; set; } = 0.5f;

        /// <summary>Max delay for backoff (seconds).</summary>
        public static float RateLimitBackoffMax { get; set; } = 8.0f;

        /// <summary>LRU cache size (translation entries).</summary>
        public static int CacheCapacity { get; set; } = 2000;

        /// <summary>
        /// RGM ServerSpecificSetting id for language choice.
        /// (matches RGM.UserSettings.ServerSpecificSettings.Translation)
        /// </summary>
        public static int LanguageSettingId { get; set; } = 12054;

        // ===== Internals =====
        private sealed class PendingCallbacks
        {
            public readonly List<Action<string>> Success = new();
            public readonly List<Action<string>> Error = new();
        }

        private sealed class Request
        {
            public string Text;
            public string Source;
            public string Target;
            public string Key;
            public int Retry;
            public PendingCallbacks Callbacks;
        }

        private static readonly Queue<Request> _queue = new();
        private static readonly Dictionary<string, PendingCallbacks> _inFlight = new();
        private static readonly LruCache<string, string> _cache = new(2000);
        private static CoroutineHandle _worker;
        private static bool _warnedNoKey;

        // ===== Public API =====

        public static void StartWorker()
        {
            if (_worker.IsRunning)
                return;

            _cache.Capacity = Math.Max(10, CacheCapacity);
            _worker = Timing.RunCoroutine(Worker(), Segment.FixedUpdate);
        }

        public static void StopWorker()
        {
            if (_worker.IsRunning)
                Timing.KillCoroutines(_worker);

            _worker = default;
        }

        public static void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Returns the player's preferred language option string (e.g., "ko" / "en-us").
        /// Falls back to "ko" when unavailable.
        /// </summary>
        public static string GetPreferredLanguage(Player player, string fallback = "ko")
        {
            try
            {
                if (player == null)
                    return fallback;

                if (SettingBase.TryGetSetting<DropdownSetting>(player, LanguageSettingId, out var setting) &&
                    !string.IsNullOrWhiteSpace(setting.SelectedOption))
                {
                    return setting.SelectedOption;
                }
            }
            catch
            {
                // ignore
            }

            return fallback;
        }

        /// <summary>
        /// Normalizes language codes to Google v2 target format.
        /// - "en-us" -> "en"
        /// - "zh-cn" -> "zh-CN" (keeps common forms)
        /// </summary>
        public static string NormalizeLanguageCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return "en";

            code = code.Trim();
            string lower = code.ToLowerInvariant();

            // RGM uses "en-us" option; Google v2 generally expects "en".
            if (lower == "en-us" || lower == "en_us")
                return "en";

            // Keep Google's special casing when user already supplies it.
            if (lower == "zh-cn") return "zh-CN";
            if (lower == "zh-tw") return "zh-TW";

            return lower;
        }

        public static void TranslatePreserveNewlines(
            string text,
            string target,
            Action<string> onSuccess,
            Action<string> onError = null,
            string source = null)
        {
            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // 빈 줄까지 유지하려면 그대로 둬야 함
            var results = new string[lines.Length];
            int remaining = lines.Length;

            for (int i = 0; i < lines.Length; i++)
            {
                int idx = i;

                // 빈 줄은 번역 요청 안 하고 그대로 처리 (쿼터 절약)
                if (string.IsNullOrEmpty(lines[idx]))
                {
                    results[idx] = "";
                    if (--remaining == 0) onSuccess(string.Join("\n", results));
                    continue;
                }

                TranslationManager.Translate(
                    lines[idx],
                    target,
                    translated =>
                    {
                        results[idx] = translated;
                        if (--remaining == 0)
                            onSuccess(string.Join("\n", results));
                    },
                    err =>
                    {
                        // 하나라도 실패하면 전체 실패 처리(원하면 부분 성공으로 바꿀 수 있음)
                        onError?.Invoke(err);
                    },
                    source
                );
            }
        }

        /// <summary>
        /// Translate with optional source. If source is null/empty, Google will auto-detect.
        /// Callback will always be invoked (success -> translated text, error -> original text).
        /// </summary>
        public static void Translate(
            string text,
            string targetLanguage,
            Action<string> onSuccess,
            Action<string> onError = null,
            string sourceLanguage = null)
        {
            if (onSuccess == null)
                throw new ArgumentNullException(nameof(onSuccess));

            if (!IsEnabled)
            {
                onSuccess(text);
                return;
            }

            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                if (!_warnedNoKey)
                {
                    _warnedNoKey = true;
                    Log.Warn("[TranslationManager] ApiKey is empty. Translation will be bypassed.");
                }

                onSuccess(text);
                return;
            }

            string target = NormalizeLanguageCode(targetLanguage);
            string source = string.IsNullOrWhiteSpace(sourceLanguage) ? null : NormalizeLanguageCode(sourceLanguage);

            // Cache key includes source (or *) and target.
            string cacheKey = BuildKey(text, source, target);

            if (_cache.TryGet(cacheKey, out string cached))
            {
                onSuccess(cached);
                return;
            }

            StartWorker();

            // In-flight coalescing
            if (_inFlight.TryGetValue(cacheKey, out var callbacks))
            {
                callbacks.Success.Add(onSuccess);
                if (onError != null) callbacks.Error.Add(onError);
                return;
            }

            callbacks = new PendingCallbacks();
            callbacks.Success.Add(onSuccess);
            if (onError != null) callbacks.Error.Add(onError);
            _inFlight[cacheKey] = callbacks;

            _queue.Enqueue(new Request
            {
                Text = text,
                Source = source,
                Target = target,
                Key = cacheKey,
                Retry = 0,
                Callbacks = callbacks,
            });
        }

        /// <summary>
        /// A "smart" translate: only translate when the text likely isn't in target language.
        /// This keeps costs down for mixed-language servers.
        /// </summary>
        public static void TranslateIfNeeded(
            string text,
            string targetLanguage,
            Action<string> onDone,
            Action<string> onError = null,
            string sourceLanguage = null)
        {
            if (!ShouldTranslate(text, targetLanguage))
            {
                onDone?.Invoke(text);
                return;
            }

            Translate(text, targetLanguage, onDone, onError, sourceLanguage);
        }

        /// <summary>
        /// Heuristic: translate if target is en and text contains Hangul; translate if target is ko and text looks mostly latin.
        /// </summary>
        public static bool ShouldTranslate(string text, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string t = NormalizeLanguageCode(targetLanguage);

            bool hasHangul = text.Any(c => c >= 0xAC00 && c <= 0xD7A3);
            bool hasLatin = text.Any(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));

            if (t == "en")
                return hasHangul;

            if (t == "ko")
                return hasLatin && !hasHangul;

            // For other targets, rely on auto-detecting and translate if there is any letter.
            return hasHangul || hasLatin;
        }

        // ===== Worker =====
        private static IEnumerator<float> Worker()
        {
            while (IsEnabled)
            {
                if (_queue.Count == 0)
                {
                    yield return Timing.WaitForSeconds(0.05f);
                    continue;
                }

                Request req = _queue.Dequeue();

                // It might have been completed via cache between enqueue/dequeue (rare but possible)
                if (_cache.TryGet(req.Key, out string cached))
                {
                    Complete(req, cached, null);
                    yield return Timing.WaitForSeconds(MinInterval);
                    continue;
                }

                yield return Timing.WaitForSeconds(Math.Max(0f, MinInterval));

                yield return Timing.WaitUntilDone(SendRequest(req));
            }
        }

        private static IEnumerator<float> SendRequest(Request req)
        {
            string url = $"https://translation.googleapis.com/language/translate/v2?key={ApiKey}";

            WWWForm form = new WWWForm();
            form.AddField("q", req.Text);
            form.AddField("target", req.Target);
            if (!string.IsNullOrWhiteSpace(req.Source))
                form.AddField("source", req.Source);

            using UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return Timing.WaitUntilDone(request.SendWebRequest());

            if (request.result != UnityWebRequest.Result.Success)
            {
                string err = request.error;
                int code = (int)request.responseCode;

                if (Debug)
                    Log.Warn($"[TranslationManager] HTTP fail ({code}): {err}");

                if (IsRetryable(code) && req.Retry < MaxRetries)
                {
                    req.Retry++;
                    float delay = ComputeBackoff(req.Retry);
                    if (Debug)
                        Log.Debug($"[TranslationManager] retry {req.Retry}/{MaxRetries} in {delay:0.00}s");

                    yield return Timing.WaitForSeconds(delay);
                    _queue.Enqueue(req);
                    yield break;
                }

                Complete(req, req.Text, err ?? $"HTTP {code}");
                yield break;
            }

            try
            {
                // { data: { translations: [ { translatedText: "...", detectedSourceLanguage?: "..." } ] } }
                var root = JObject.Parse(request.downloadHandler.text);
                string translated = root["data"]?["translations"]?.First?["translatedText"]?.Value<string>();

                if (translated == null)
                    throw new Exception("translatedText not found");

                // Google returns HTML entities sometimes.
                translated = WebUtility.HtmlDecode(translated);

                _cache.Set(req.Key, translated);
                Complete(req, translated, null);
            }
            catch (Exception e)
            {
                if (Debug)
                    Log.Warn($"[TranslationManager] Parse fail: {e.Message}");

                Complete(req, req.Text, e.Message);
            }
        }

        private static void Complete(Request req, string result, string error)
        {
            if (!_inFlight.TryGetValue(req.Key, out var callbacks))
                return;

            _inFlight.Remove(req.Key);

            foreach (var cb in callbacks.Success.ToList())
            {
                try { cb?.Invoke(result); }
                catch { /* ignore */ }
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                foreach (var cb in callbacks.Error.ToList())
                {
                    try { cb?.Invoke(error); }
                    catch { /* ignore */ }
                }
            }
        }

        private static string BuildKey(string text, string source, string target)
        {
            string src = string.IsNullOrWhiteSpace(source) ? "*" : source;
            return $"{src}|{target}|{text}";
        }

        private static bool IsRetryable(int httpCode)
        {
            // 0: network / unknown
            if (httpCode == 0) return true;
            if (httpCode == 429) return true;
            if (httpCode == 403) return true; // quota/rate can be 403 in some cases
            if (httpCode >= 500 && httpCode <= 599) return true;
            return false;
        }

        private static float ComputeBackoff(int retry)
        {
            // base * 2^(retry-1) + small jitter
            float pow = (float)Math.Pow(2, Math.Max(0, retry - 1));
            float delay = RateLimitBackoffBase * pow;
            delay = Math.Min(delay, RateLimitBackoffMax);

            // jitter 0~150ms
            delay += UnityEngine.Random.Range(0f, 0.15f);
            return delay;
        }

        // ===== Simple LRU cache =====
        private sealed class LruCache<TKey, TValue>
        {
            private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _map = new();
            private readonly LinkedList<(TKey Key, TValue Value)> _list = new();

            public int Capacity { get; set; }

            public LruCache(int capacity)
            {
                Capacity = Math.Max(1, capacity);
            }

            public bool TryGet(TKey key, out TValue value)
            {
                if (_map.TryGetValue(key, out var node))
                {
                    _list.Remove(node);
                    _list.AddFirst(node);
                    value = node.Value.Value;
                    return true;
                }

                value = default;
                return false;
            }

            public void Set(TKey key, TValue value)
            {
                if (_map.TryGetValue(key, out var existing))
                {
                    _list.Remove(existing);
                    _map.Remove(key);
                }

                var node = new LinkedListNode<(TKey Key, TValue Value)>((key, value));
                _list.AddFirst(node);
                _map[key] = node;

                while (_map.Count > Capacity)
                {
                    var last = _list.Last;
                    if (last == null) break;
                    _map.Remove(last.Value.Key);
                    _list.RemoveLast();
                }
            }

            public void Clear()
            {
                _map.Clear();
                _list.Clear();
            }
        }
    }
}
