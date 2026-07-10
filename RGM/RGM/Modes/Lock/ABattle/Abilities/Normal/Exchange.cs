using Exiled.Events.EventArgs.Player;
using Scp914.Processors;
using Scp914;

namespace RGM.Modes.Abilities.Normal;

[Ability("교환", "현재 들고 있는 아이템을 강화합니다. (1:1 기준)", AbilityCategory.Common, AbilityType.NORMAL_EXCHANGE)]
public class Exchange : Ability
{
    public override void OnEnabled()
    {
        if (Owner.CurrentItem != null)
        {
            if (Scp914Upgrader.TryGetProcessor(Owner.CurrentItem.Type, out Scp914ItemProcessor processor))
                processor.UpgradeInventoryItem(Scp914KnobSetting.OneToOne, Owner.CurrentItem.Base);
        }
    }

    public override void OnDisabled()
    {
    }

    public void OnHurting(HurtingEventArgs ev)
    {
    }
}
