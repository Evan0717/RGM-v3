using Scp914;
using Scp914.Processors;

namespace RGM.Modes.Abilities.Rare;

[Ability("강화", "현재 들고 있는 아이템을 강화합니다. (Very Fine 기준)", AbilityCategory.Rare, AbilityType.RARE_UPGRADE)]
public class Upgrade : Ability
{
    public override void OnEnabled()
    {
        if (Owner.CurrentItem != null)
        {
            if (Scp914Upgrader.TryGetProcessor(Owner.CurrentItem.Type, out Scp914ItemProcessor processor))
                processor.UpgradeInventoryItem(Scp914KnobSetting.VeryFine, Owner.CurrentItem.Base);
        }
    }

    public override void OnDisabled()
    {
    }
}
