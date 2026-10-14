using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Epic;

[Ability("!!마쉬멜로우!!", """
                      즉시 마쉬멜로우맨이 됩니다. 
                      최대 체력이 400 증가하고, 자신의 기본 공격 데미지가 50 증가하며, 이동 속도가 20%p 증가합니다.
                      """, 
    AbilityCategory.Epic, AbilityType.EPIC_MARSHMELLOW, 
    RoleAbility.None, false, AbilityHolidayType.Halloween, isUnique:true)]
public class MarshMellow : Ability
{
    public override void OnEnabled()
    {
        Owner.MaxHealth += 400;
        Owner.Health += 400;
        for (int i = 0; i < 5; i++)
        {
            Owner.AddAbility(AbilityType.NORMAL_SWIFT);
        }
        Owner.AddItem(ItemType.MarshmallowItem);
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != Owner || ev.Attacker == null) return;
        if (ev.DamageHandler.Type != DamageType.Marshmallow) return;
        
        ev.DamageHandler.Damage += 50f;

    }
}
