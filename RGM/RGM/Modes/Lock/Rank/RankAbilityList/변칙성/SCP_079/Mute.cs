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
    [RankAbility("Mute", "SCP-2176에 면역이 됩니다.", RankAbilityType.Mute, RankCategory.SCP_079, RankAbilityCategory.변칙성, "🔇")]
    public class Mute : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp079.LosingSignal += OnLosingSignal;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp079.LosingSignal -= OnLosingSignal;
        }

        void OnLosingSignal(LosingSignalEventArgs ev)
        {
            if (ev.Player == Owner)
                ev.IsAllowed = false;
        }
    }
}
