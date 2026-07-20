using System.Linq;
using Exiled.API.Extensions;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.CHI;

[Ability("혼돈의 가방", "인벤토리에 있는 아이템들이 전부 변경됩니다.", AbilityCategory.Common, AbilityType.COMMON_CHI_BAGOFCHAOS, RoleAbility.CHI)]
public class BagOfChaos : Ability
{
    public override void OnEnabled()
    {
        int Count = Owner.Items.Where(x => !x.IsAmmo).ToList().Count;

        Owner.ClearItems();

        for (int i = 1; i < Count + 1; i++)
        {
            try { Owner.AddItem(Tools.GetRandomValue(Tools.EnumToList<ItemType>().Where(x => !x.IsAmmo()).ToList())); }
            catch { }
        }
    }

    public override void OnDisabled()
    {
    }
}
