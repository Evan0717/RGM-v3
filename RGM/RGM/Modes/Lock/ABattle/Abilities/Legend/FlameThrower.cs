using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.MicroHID.Modules;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("화염 방사기", "위력은 41%로 낮아지지만, 상대를 불태우고 자동으로 충전되는 화염 방사기를 받습니다.", AbilityCategory.Legend, AbilityType.LEGEND_FLAMETHROWER)]
public class FlameThrower : Ability
{
    private ushort _flamethrowerSerial;
    CoroutineHandle _onStarted;

    public override void OnEnabled()
    {
        Item ft = Owner.AddItem(ItemType.MicroHID);
        _flamethrowerSerial = ft.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.ChangingMicroHIDState += OnChangingMicroHIDState;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;

        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    private IEnumerator<float> OnStarted()
    {
        while (true)
        {
            foreach (var Item in Item.List.Where(x => x.Type == ItemType.MicroHID))
            {
                if (_flamethrowerSerial != Item.Serial) continue;
                MicroHid MicroHID = (MicroHid)Item;

                if (MicroHID.Energy < 1)
                    MicroHID.Energy += 0.03f;
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }

    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _flamethrowerSerial)
            return;
        
        ev.Player.AddHint("화염 방사기", $"<b><color={ABattle.RatingColor["전설"]}>화염 방사기</color></b> 능력이 있는 <b>마이크로 H.I.D</b>입니다!");
    }

    private void OnChangingMicroHIDState(ChangingMicroHIDStateEventArgs ev)
    {
        if (_flamethrowerSerial == ev.Item.Serial && ev.NewPhase == MicroHidPhase.WindingUp)
            ev.NewPhase = MicroHidPhase.Firing;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Player == ev.Attacker)
            return;

        if (ev.Attacker.CurrentItem == null || _flamethrowerSerial != ev.Attacker.CurrentItem.Serial) return;
        ev.DamageHandler.Damage *= 0.41f;

        ev.Player.EnableEffect(EffectType.Burned, 1, 1.5f);
    }
}
