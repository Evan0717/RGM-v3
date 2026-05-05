using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("아드레날린", "10초간 데미지를 40% 적게 받습니다. (분노 중 한정)", RankAbilityType.아드레날린, RankCategory.SCP_096, "📴", 120)]
    public class 아드레날린 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.Role is Scp096Role scp096 && scp096.RageManager.IsEnraged)
                return true;

            else
                return false;
        }

        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.DamageReduction, 40, 10);
        }
    }
}
