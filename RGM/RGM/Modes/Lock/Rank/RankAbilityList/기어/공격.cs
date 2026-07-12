using Exiled.Events.EventArgs.Player;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.기어
{
    [RankAbility("공격", "데미지가 16% 증가합니다.", RankAbilityType.공격, RankCategory.공통, RankAbilityCategory.기어_메인, "🔪")]
    public class 공격 : RankAbility
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
            {
                ev.DamageHandler.Damage *= 1.16f;
            }
        }
    }
}
