using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Scp1509;
using MEC;
using Mirror;
using MultiBroadcast;

using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Set, ModeType.Develop)]
    class Develop : Mode
    {
        public override string Name => "필독";
        public override string Description => "[📃 전체 모드] 드롭다운을 활용하여 모드 설명을 읽어 보세요!";
        public override string Detail =>
"""
여기는 자세한 설명이 쓰이는 자리입니다.
""";
        public override string Color => "FFFFFF";

        public static Develop Instance;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves();
            Exiled.API.Features.Map.IsDecontaminationEnabled = false;

            Exiled.Events.Handlers.Scp1509.Resurrecting += OnResurrecting;
        }

        public override void OnDisabled()
        {
            Round.IsLocked = false;
            Respawn.ResumeWaves();
            Exiled.API.Features.Map.IsDecontaminationEnabled = true;

            Exiled.Events.Handlers.Scp1509.Resurrecting -= OnResurrecting;
        }

        void OnResurrecting(ResurrectingEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
