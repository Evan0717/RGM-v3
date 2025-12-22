using Exiled.API.Features.Items;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using RGM.API.DataBases;
using static RGM.Variables.Variable;
using UnityEngine;
using System.IO;

namespace RGM.Modes
{
    [Mode(ModeCategory.OnlySub, ModeInfo.Plus, ModeType.Radio)]
    public class Radio : Mode
    {
        public override string Name => "라디오";
        public override string Description => "배경 음악이 상시 재생됩니다. 뭐가 재생될지는 모르죠.";
        public override string Detail =>
"""
랜덤게임모드 서버의 오디오 파일 내에서 랜덤으로 재생됩니다.
""";
        public override string Color => "A9A9F5";

        public static Radio Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                string audioDir = Paths.Plugins + "/audio/";
                string[] audioFiles = Directory.GetFiles(audioDir).Select(Path.GetFileName).ToArray();

                AudioClipPlayback clip = GlobalPlayer.TryPlay(audioFiles.GetRandomValue(), 1f, false);

                yield return Timing.WaitForSeconds((int)clip.Duration.TotalSeconds + Random.Range(1, 21));
            }
        }
    }
}
