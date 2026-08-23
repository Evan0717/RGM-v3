using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.NTF.Rare;

[Ability("관리 의무자", """
                   지휘관 카드, E11sr, 방탄복, 5.56mm 5세트, 섬광탄을 지급받습니다.
                   추가로, SCP 진영에게 입히는 피해가 40% 증가합니다.
                   """, 
    AbilityCategory.Rare, AbilityType.RARE_NTF_MANAGERIALOBLIGATIONPERSON, RoleAbility.NTF)]
public class ManagerialObligationPerson : Ability
{
    private static readonly List<RoleTypeId> ScpRoles =
    [
        RoleTypeId.Scp049,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp173,
        RoleTypeId.Scp939,
        RoleTypeId.Scp3114,
        RoleTypeId.Scp0492
    ];  
    
    private static readonly List<ItemType> ManageDuty =
    [
        ItemType.KeycardMTFCaptain,
        ItemType.GunE11SR,
        ItemType.ArmorCombat,
        ItemType.GrenadeFlash,
        ItemType.Ammo556x45,
        ItemType.Ammo556x45,
        ItemType.Ammo556x45,
        ItemType.Ammo556x45,
        ItemType.Ammo556x45
    ];
    public override void OnEnabled()
    {
        foreach (var item in ManageDuty)
            Owner.AddItem(item);
        
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != Owner || ScpRoles.Contains(ev.Attacker.Role)) return;
        if (!ScpRoles.Contains(ev.Player.Role)) return;
        if (ABattle.Instance.GetAbility(Owner, AbilityType.RARE_NTF_MANAGERIALOBLIGATIONPERSON) != this) return;
        
        ev.DamageHandler.Damage *= 1 + 0.4f * Owner.AbilityCount(AbilityType.RARE_NTF_MANAGERIALOBLIGATIONPERSON);
    }
}
