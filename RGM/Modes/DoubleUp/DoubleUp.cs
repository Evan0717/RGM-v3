using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using MultiBroadcast;
using MultiBroadcast.API;
using RGM.API;

namespace RGM.Modes
{
    class DoubleUp
    {
        public static DoubleUp Instance;

        public static List<string> BlackListMods = Tools.GetMiniGamesList();
        public static Dictionary<string, List<string>> Mods = RGM.Instance.ModeList;

        public static List<string> ModeKeys = RGM.Instance.ModeList.Keys.Where(x => Mods[x][3] != "private" && !BlackListMods.Contains(x)).ToList();
        public static string mod1 = ModeKeys[UnityEngine.Random.Range(0, ModeKeys.Count())];
        public static string mod2 = ModeKeys[UnityEngine.Random.Range(0, ModeKeys.Count())];

        public List<string> Modes = new List<string>() { mod1, mod2 };

        public List<string> pl = new List<string>();

        public static string Description = $"<size=25><b>[<color=#{Mods[mod1][0]}>{mod1}</color> + <color=#{Mods[mod2][0]}>{mod2}</color>]</b></size>";

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            for (int i=0; i<2; i++)
            {
                var modeType = Type.GetType($"RGM.Modes.{Mods[Modes[i]][2]}");
                if (modeType != null)
                {
                    var modeInstance = Activator.CreateInstance(modeType);
                    var onEnabledMethod = modeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);
                }
            }

            foreach (var player in Player.List.Where(x => !x.IsNPC))
            {
                player.AddBroadcast(10, Description);
                player.SendConsoleMessage($"\n{Description}", "white");
            }
        }

        public void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            ev.Player.AddBroadcast(10, Description);
            ev.Player.SendConsoleMessage($"\n{Description}", "white");
        }
    }
}
