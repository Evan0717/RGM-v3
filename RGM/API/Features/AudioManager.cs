using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.API.Features
{
    public static class AudioManager
    {
        public static AudioClipPlayback TryPlay(this AudioPlayer audioPlayer, string clipName, float volume = 1, bool loop = false, bool destroyOnEnd = true)
        {
            string audioDir = Paths.Plugins + "/audio/";
            string clipPath = System.IO.Path.Combine(audioDir, clipName + ".ogg");

            if (System.IO.File.Exists(clipPath))
            {
                AudioClipStorage.LoadClip(clipPath, clipName);

                AudioClipPlayback audioClipPlayback = audioPlayer.AddClip(clipName, volume, loop, destroyOnEnd);

                return audioClipPlayback;
            }
            else
            {
                Log.Warn($"오디오 파일 '{clipName}.ogg'을(를) 찾을 수 없습니다.");

                return null;
            }
        }
    }
}