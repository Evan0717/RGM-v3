using System.Linq;
using Exiled.API.Features.Items;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Tutorial;

[Ability("SCP 연구자", "SCP 아이템 중 하나를 지급받습니다.", AbilityCategory.Tutorial, AbilityType.TUTORIAL_RESEARCHER)]
public class Researcher : Ability
{
    public override void OnEnabled()
    {
        Item SCPItem = Owner.AddItem(Tools.GetRandomValue(Tools.EnumToList<ItemType>().Where(x => x.ToString().Contains("SCP")).ToList()));
    }

    public override void OnDisabled()
    {
    }
}
