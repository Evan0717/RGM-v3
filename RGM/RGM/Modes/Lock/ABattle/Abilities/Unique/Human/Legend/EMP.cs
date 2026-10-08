using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Human.Legend;

[Ability("EMP",
    """
    시설에 강력한 전자기장 공격을 가합니다.
    모든 시설이 10초 간 정전되며, SCP-079의 신호를 50초간 차단시키고, 레벨을 1로 초기화합니다.
    사용 시 재사용 대기시간 150초가 적용됩니다.
    """, 
    AbilityCategory.Legend, AbilityType.LEGEND_HUMAN_EMP, RoleAbility.Human)]

public class EMP : Ability
{
    private ushort _empSerial;
    private const float EmpCooldown = 150f;

    public override void OnEnabled()
    {
        Item emp = Owner.AddItem(ItemType.Radio);
        _empSerial = emp.Serial;
        
    }
    
    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _empSerial)
            return;
        
        ev.Player.AddHint("EMP", $"<b><color={ABattle.RatingColor["전설"]}>EMP</color></b> 능력이 있는 <b>무전기</b>입니다!");
    }

    private void OnTogglingRadio(TogglingRadioEventArgs ev)
    {
        if (_empSerial != ev.Item.Serial) return;
        /*
         * EMP 능력 구현
         *
         * 1. 발동 시, 10초간 전체 구역을 정전하며, 모든 문과 엘레베이터의 버튼을 Lock.
         * 2. SCP-079에게 signal lost를 50초간 적용하며, 동시에 레벨을 1로 초기화.
         */
    }
}