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
                if (!AudioClipStorage.AudioClips.ContainsKey(clipName))
                    AudioClipStorage.LoadClip(clipPath, clipName);

                AudioClipPlayback audioClipPlayback = audioPlayer.AddClip(clipName, volume, loop, destroyOnEnd);

                return audioClipPlayback;
            }
            else
            {
                return null;
            }
        }
    }
}