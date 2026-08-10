using Exiled.Events.EventArgs.Player;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

//[Ability("Claymore", "지급된 동전을 튕기면 사용자가 보는 방향이 앞이 되는 Claymore를 설치합니다.", AbilityCategory.Rare, AbilityType.RARE_CLAYMORE)]
public class Claymore : Ability
{
    public override void OnEnabled()
    {
        
    }

    public override void OnDisabled()
    {
        
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        // SchematicObject lava = ObjectSpawner.SpawnSchematic("Claymore", new Vector3(x, y, z));

    }
}