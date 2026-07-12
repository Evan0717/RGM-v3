using RGM.Modes;
using UnityEngine;
using Exiled.API.Features.Items;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("이중 탄창", "즉시 탄창을 30 장전합니다.", RankAbilityType.이중_탄창, RankCategory.시설_경비, "󾠯", 60)]
    public class 이중_탄창 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.CurrentItem == null || Owner.CurrentItem is not Firearm)
                return false;

            else
                return true;
        }

        protected override void OnGadgetUsed()
        {
            if (Owner.CurrentItem is Firearm firearm)
                firearm.MagazineAmmo = (byte)Mathf.Min(firearm.MagazineAmmo + 30, firearm.MaxMagazineAmmo);
        }
    }
}
