using Exiled.Events.EventArgs.Player;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("날카로움 Ⅴ", "공격력이 30 증가합니다.", RankAbilityType.날카로움_5, RankCategory.SCP_096, RankAbilityCategory.변칙성, "🔪")]
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

        void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker == Owner)
                ev.DamageHandler.Damage += 30;
        }
    }
}
