using SecretAPI.Features.UserSettings;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace RGM.UserSettings
{
    public static partial class SettingManager
    {
        private static CoroutineHandle _reloader;
        
        public static void Init()
        {
            if (!Timing.IsRunning(_reloader))
                _reloader = Timing.RunCoroutine(Reloader());
        }

        private static IEnumerator<float> Reloader()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(60f);
                
                CustomSetting.ResyncServer();
            }
        }
    }
}
