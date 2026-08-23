namespace RGM.Modes.Abilities.Unique.ClassD;

[Ability("주거침입죄", "SCP-268을 지급받습니다.", AbilityCategory.Normal, AbilityType.RARE_CLASSD_TRESPASSING, RoleAbility.ClassD)]
public class Trespassing : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.SCP268);
    }
}
