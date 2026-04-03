using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.Handlers;
using Exiled.Events.Patches.Events.Scp049;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using RGM.Modes;
using SLPlayerRotation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("흉내쟁이", "흉내 쿨다운이 0.1초로 조정됩니다.", RankAbilityType.흉내쟁이, RankCategory.SCP_939, RankAbilityCategory.변칙성, "☺")]
    public class 흉내쟁이 : RankAbility
    {
        CoroutineHandle handle;

        public override void OnEnabled()
        {
            handle = Timing.RunCoroutine(enumerator());
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(handle);
        }

        IEnumerator<float> enumerator()
        {
            while (true)
            {
                if (Owner.Role is Scp939Role scp939)
                    scp939.MimicryCooldown = 0;

                yield return Timing.WaitForSeconds(0.1f);
            }
        }
    }
}
