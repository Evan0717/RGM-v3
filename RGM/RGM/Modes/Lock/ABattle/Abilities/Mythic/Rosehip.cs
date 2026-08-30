using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Mythic;

[Ability("장미칼", """
                이 명검은 무한으로 발산하는 힘을 가지고 있습니다...
                50% 확률로 진영을 변경하며, 변경 실패 시 적을 『사망』 시킵니다.
                """, AbilityCategory.Mythic, AbilityType.MYTHIC_ROSEHIP)]
public class Rosehip : Ability
{
    private ushort _serial;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.SCP1509);
        _serial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }
    
    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _serial)
            return;
            
        ev.Player.AddHint("장미칼", $"<b><color={ABattle.RatingColor["신화"]}>장미칼</color></b> 능력이 있는 <b>SCP-1509</b>입니다!");
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null ||
            ev.Attacker.CurrentItem == null ||
            ev.Attacker.CurrentItem.Serial != _serial) return;
        if (UnityEngine.Random.Range(1, 101) <= 50)
        {
            ev.IsAllowed = false;
            ev.Player.Role.Set(Tools.EnumToList<RoleTypeId>().GetRandomValue(x => x.GetSide() == ev.Attacker.Role.Type.GetSide()), RoleSpawnFlags.None);
            return;
        }
        
        ev.Player.Hurt(amount: ev.Player.MaxHealth, damageType: DamageType.Crushed, attacker: ev.Attacker);
    }
}