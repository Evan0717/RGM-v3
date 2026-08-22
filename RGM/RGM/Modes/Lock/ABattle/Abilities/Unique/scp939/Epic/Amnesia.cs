using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp939.Epic;

[Ability("기억 소거", "자신을 기준으로 반경 22m 내의 모든 적에게 기억 소거 효과를 적용합니다.",
    AbilityCategory.Epic, AbilityType.EPIC_SCP939_AMNESIA, RoleAbility.Scp939)]

public class Amnesia : Ability
{
    private CoroutineHandle _abilitystart;
    public override void OnEnabled()
    {
        _abilitystart = Timing.RunCoroutine(OnAbilityStarted());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_abilitystart);
    }

    private IEnumerator<float> OnAbilityStarted()
    {
        while (true)
        {
            foreach (var near in PlayerManager.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, Owner.Position) <= 22))
            {
                if (Owner != near && HitboxIdentity.IsEnemy(Owner.ReferenceHub, near.ReferenceHub))
                {
                    near.EnableEffect(EffectType.AmnesiaItems, 1, 1.5f);
                    near.EnableEffect(EffectType.AmnesiaVision, 1, 1.5f);
                }
            }
            yield return Timing.WaitForSeconds(1f);
        }
    }
}