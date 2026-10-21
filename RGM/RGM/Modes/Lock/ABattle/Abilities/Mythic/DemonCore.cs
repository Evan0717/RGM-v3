using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Mythic;

[Ability("데몬 코어", "수상한 플루토늄 반구입니다. 임계질량을 넘는 순간 무슨 일이 일어날지 모릅니다.", 
    AbilityCategory.Mythic, AbilityType.MYTHIC_DEMONCORE,
    RoleAbility.None, false, AbilityHolidayType.Halloween)]

public class DemonCore : Ability
{
    private ushort _demonCoreSerial;
    
    public override void OnEnabled()
    {
        Item jb = Owner.AddItem(ItemType.Jailbird);
        _demonCoreSerial = jb.Serial;
    }
    
    
    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _demonCoreSerial)
            return;
        
        ev.Player.AddHint("데몬 코어", $"<b><color={ABattle.RatingColor["신화"]}>데몬 코어</color></b> 능력이 있는 <b>제일버드</b>입니다!");
    }
}