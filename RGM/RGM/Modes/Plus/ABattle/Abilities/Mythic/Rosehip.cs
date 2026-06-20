using Exiled.API.Extensions;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Mythic;

[Ability("장미칼", "이 명검은 무한으로 발산하는 힘을 가지고 있습니다..", AbilityCategory.Mythic, AbilityType.MYTHIC_ROSEHIP)]
public class Rosehip : Ability
{
    ushort serial = 0;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.SCP1509);
        serial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (serial == ev.Player.CurrentItem.Serial && ev.Item != null)
        {
            if (serial == ev.Item.Serial)
                ev.Player.AddHint("장미칼", $"<b><color={ABattle.RatingColor["신화"]}>장미칼</color></b> 능력이 있는 <b>SCP-1509</b>입니다!");
        }
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != null &&
            ev.Attacker == Owner &&
            ev.Attacker.CurrentItem != null && 
            ev.Attacker.CurrentItem.Serial == serial)
        {
            ev.Player.Role.Set(Tools.EnumToList<RoleTypeId>().GetRandomValue(x => x.GetSide() == ev.Attacker.Role.Type.GetSide()), RoleSpawnFlags.None);
        }
    }
}