using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features.Items;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Tutorial;

[Ability("SCP 연구자", "SCP 아이템 중 하나를 지급받습니다.", AbilityCategory.Common, AbilityType.NORMAL_TUTORIAL_RESEARCHER, RoleAbility.Tutorial)]
public class Researcher : Ability
{
    public override void OnEnabled()
    {
        Item SCPItem = Owner.AddItem(Tools.EnumToList<ItemType>().Where(x => x.ToString().Contains("SCP")).ToList().GetRandomValue());
    }

    public override void OnDisabled()
    {
    }
}
