using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("평화주의자", "무기를 소지할 수 없게 됩니다. 대신 초당 최대 체력이 1씩 증가합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.Peace, "💞")]
public class Peace : TFTAbility
{
    CoroutineHandle _peaceLoop;

    public override void OnEnabled()
    {
        foreach (var item in Owner.Items.ToList())
        {
            if (item.Type.IsWeapon())
                Owner.DropItem(item);
        }

        Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
        
        _peaceLoop = Timing.RunCoroutine(peaceLoop());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;

        Timing.KillCoroutines(_peaceLoop);
    }

    IEnumerator<float> peaceLoop()
    {
        while (true)
        {
            Owner.MaxHealth += 1;
            Owner.Health += 1;

            yield return Timing.WaitForSeconds(1f);
        }
    }

    void OnItemAdded(ItemAddedEventArgs ev)
    {
        if (ev.Player == Owner) 
        {
            if (ev.Item.Type.IsWeapon())
                ev.Player.DropItem(ev.Item);
        }
    }
}
