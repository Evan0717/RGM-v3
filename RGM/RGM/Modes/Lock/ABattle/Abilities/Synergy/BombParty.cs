using System.Linq;
using Exiled.API.Features.Items;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.RARE_BOMBERMAN, AbilityType.RARE_BOMBERMAN, AbilityType.RARE_BOMBERMAN)]
[Ability("폭탄 파티", "<봄버맨 x3> 자격은 충분합니다. 서버 내 모든 적에게 봄버맨을 떨굽니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_BOMBPARTY)]
public class BombParty : Ability
{
    public override void OnEnabled()
    {
        foreach (var enemy in PlayerManager.List.Where(x => HitboxIdentity.IsEnemy(x.ReferenceHub, Owner.ReferenceHub)))
        {
            var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Owner);
            g.FuseTime = 3f;
            g.SpawnActive(enemy.Position, Owner);
        }
    }

    public override void OnDisabled()
    {
    }
}
