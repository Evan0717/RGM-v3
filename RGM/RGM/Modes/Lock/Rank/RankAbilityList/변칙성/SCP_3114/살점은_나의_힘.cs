using Exiled.Events.EventArgs.Player;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("살점은 나의 힘", "인간을 공격할 때마다 체력이 15 회복됩니다.", RankAbilityType.살점은_나의_힘, RankCategory.SCP_3114, RankAbilityCategory.변칙성, "🌊")]
    public class 살점은_나의_힘 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == Owner && ev.Player.IsHuman)
            {
                ev.Attacker.Heal(15);
            }
        }
    }
}
