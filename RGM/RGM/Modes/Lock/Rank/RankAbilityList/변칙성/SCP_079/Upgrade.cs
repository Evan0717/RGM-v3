using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Patches.Events.Scp049;
using MEC;
using RGM.Modes;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("Upgrade", "경험치 획득량이 20% 증가합니다.", RankAbilityType.Upgrade, RankCategory.SCP_079, RankAbilityCategory.변칙성, "🔧")]
    public class Upgrade : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp079.GainingExperience += OnGainingExperience;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp079.GainingExperience -= OnGainingExperience;
        }

        void OnGainingExperience(GainingExperienceEventArgs ev)
        {
            if (ev.Player == Owner)
            {
                ev.Amount = (int)(ev.Amount * 1.2f);
            }
        }
    }
}
