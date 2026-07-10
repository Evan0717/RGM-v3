using Exiled.API.Features.Items;

namespace RGM.Modes.Abilities.Rare;

[Ability("무기 전문가", "SCP-1853을 받습니다.", AbilityCategory.Rare, AbilityType.RARE_WEAPONEXPERT)]
public class WeaponExpert : Ability
{
    public override void OnEnabled()
    {
        Item scp1853 = Owner.AddItem(ItemType.SCP1853);
    }

    public override void OnDisabled()
    {
    }
}
