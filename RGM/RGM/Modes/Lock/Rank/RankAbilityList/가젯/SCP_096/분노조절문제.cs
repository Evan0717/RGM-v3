using Exiled.API.Features.Roles;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("분노조절문제", "즉시 분노합니다. 대신 유지 시간이 8초로 조정됩니다.", RankAbilityType.분노조절문제, RankCategory.SCP_096, "😠", 30)]
    public class 분노조절문제 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.Role is Scp096Role scp096 && !scp096.RageManager.IsEnraged)
                return true;

            else
                return false;
        }

        protected override void OnGadgetUsed()
        {
            if (Owner.Role is Scp096Role scp096)
                scp096.Enrage(8);
        }
    }
}
