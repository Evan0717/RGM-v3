using System;
using Random = UnityEngine.Random;

namespace RGM.Modes.Abilities.Rare;

[Ability("일회용 반사경", """
                    다음에 획득하는 능력에 반사경 효과가 적용됩니다.
                    [전설] 반사경 능력을 보유 중인 경우 해당 능력은 획득할 수 없습니다.
                    """,
    AbilityCategory.Rare, AbilityType.RARE_MINIREFLECTOR)]

public class MiniReflector : Ability
{
    private const byte MaxChainCount = 3;
    private const byte ReflectionChance = 45;

    public override void OnEnabled() => ABattle.Instance.AddingAbility += OnAddingAbility;

    public override void OnDisabled() => ABattle.Instance.AddingAbility -= OnAddingAbility;

    private void OnAddingAbility(AddingAbilityEventArgs ev)
    {
        if (!ev.IsAllowed || !ev.IsSelectionAbility || ev.Player != Owner || !ev.AllowReflector ||
            ev.ReflectorChain >= MaxChainCount)
            return;

        if (ev.AbilityType == AbilityType.LEGEND_REFLECTOR)
        {
            ev.IsAllowed = false;
            return;
        }

        var shouldReflect = !ABattle.Instance.Abilities[ev.AbilityType].IsUnique &&
            ABattle.Instance.Abilities[ev.AbilityType].Category != AbilityCategory.Ancient &&
            ABattle.Instance.Abilities[ev.AbilityType].Category != AbilityCategory.Synergy &&
            Convert.ToByte(Random.Range(1, 101)) <= ReflectionChance;

        Owner.RemoveAbility(this);
        Owner.AddAbility(AbilityType.DUMMY_DESTROYEDREFLECTOR);

        if (!shouldReflect)
            return;

        _ = ABattle.Instance.AddAbilityAsync(ev.Player,
            [ev.AbilityType],
            reflectorChain: ev.ReflectorChain + 1,
            allowReflector: ev.AllowReflector,
            extraReflectorChain: ev.ExtraReflectorChain);
    }
}