using Exiled.Events.EventArgs.Scp079;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("Upgrade", "경험치 획득량이 17% 증가합니다.", RankAbilityType.Upgrade, RankCategory.SCP_079, RankAbilityCategory.변칙성, "🔧")]
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
                ev.Amount = (int)(ev.Amount * 1.17f);
            }
        }
    }
}
