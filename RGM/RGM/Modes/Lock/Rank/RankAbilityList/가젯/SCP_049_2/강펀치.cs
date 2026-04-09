using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using MEC;
using MultiBroadcast.Commands.Subcommands;
using PlayerRoles;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("강펀치", "다음 공격의 데미지가 55 추가됩니다.", RankAbilityType.강펀치, RankCategory.SCP_049_2, "👊")]
    public class 강펀치 : RankGadgetAbility
    {
        bool isEnabled = false;

        protected override bool CanUseGadget()
        {
            return !isEnabled;
        }

        protected override void OnGadgetUsed()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            isEnabled = true;

            void OnHurting(HurtingEventArgs ev)
            {
                if (ev.Attacker == Owner)
                    ev.DamageHandler.Damage += 55;

                Exiled.Events.Handlers.Player.Hurting -= OnHurting;
                isEnabled = false;
            }
        }
    }
}
