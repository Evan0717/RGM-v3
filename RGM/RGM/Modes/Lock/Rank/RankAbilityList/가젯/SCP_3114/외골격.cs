using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("외골격", "흄 쉴드를 모두 소모하여 60% 만큼 체력으로 치환합니다.", RankAbilityType.외골격, RankCategory.SCP_3114, "💀", 120)]
    public class 외골격 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.MaxHumeShield <= Owner.HumeShield)
                return false;

            else
                return true;
        }
        protected override void OnGadgetUsed()
        {
            var hume = Owner.HumeShield;

            Owner.HumeShield = 0;

            Owner.Heal(hume * 0.6f);
        }
    }
}
