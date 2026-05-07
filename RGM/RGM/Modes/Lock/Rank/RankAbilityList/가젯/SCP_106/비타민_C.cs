using Exiled.API.Features.Roles;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("비타민 C", "즉시 기력을 20% 회복합니다.", RankAbilityType.비타민_C, RankCategory.SCP_106, "🍺", 130)]
    public class 비타민_C : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.Role is Scp106Role scp106 && scp106.Vigor < scp106.VigorAbility.Vigor.MaxValue)
                return true;

            else
                return false;
        }

        protected override void OnGadgetUsed()
        {
            if (Owner.Role is Scp106Role scp106)
                scp106.Vigor += scp106.VigorAbility.Vigor.MaxValue * 0.2f;
        }
    }
}
