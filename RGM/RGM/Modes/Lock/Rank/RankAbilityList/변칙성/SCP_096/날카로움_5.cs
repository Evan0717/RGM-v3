using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;
using Exiled.API.Enums;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("날카로움 Ⅴ", "공격력이 90으로 고정됩니다.", RankAbilityType.날카로움_5, RankCategory.SCP_096, RankAbilityCategory.변칙성, "🔪")]
    public class 날카로움_5 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (Owner.Role is not Scp096Role scp096) return;
            if (ev.DamageHandler.Type != DamageType.Scp096) return;
            if (ev.Attacker != null && ev.Attacker == Owner && ev.Attacker.ReferenceHub == scp096.Owner.ReferenceHub)
                ev.DamageHandler.Damage = 90;
        }
    }
}
