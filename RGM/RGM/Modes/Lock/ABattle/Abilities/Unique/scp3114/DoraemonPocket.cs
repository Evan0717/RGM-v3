using System.Collections.Generic;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Scp3114;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp3114;

[Ability("도라에몽 주머니", "변신을 해제할 때마다 아이템을 하나 지급받습니다.", AbilityCategory.Common, AbilityType.COMMON_SCP3114_DORAEMONPOCKET, RoleAbility.Scp3114)]
public class DoraemonPocket : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp3114.Revealed += OnRevealed;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp3114.Revealed -= OnRevealed;
    }

    public void OnRevealed(RevealedEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

        Item Item = ev.Player.AddItem(Tools.GetRandomValue(ItemTypes));

        ev.Player.CurrentItem = Item;
    }
}
