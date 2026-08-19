using System.Linq;
using Exiled.API.Extensions;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Tutorial;

[Ability("SCP 연구자", "SCP 아이템 중 하나를 지급받습니다.", AbilityCategory.Normal, AbilityType.NORMAL_TUTORIAL_RESEARCHER, RoleAbility.Tutorial)]
public class Researcher : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(Tools.EnumToList<ItemType>().Where(x => x.ToString().Contains("SCP")).ToList().GetRandomValue());
    }
}
