using MEC;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.LEGEND_GAMBLER, AbilityType.LEGEND_MAGICIAN)]
[Ability("광대", "<도박사, 마술사> 도무지 알 수 없는 녀석입니다. Joker을 획득합니다.",
    AbilityCategory.Synergy, AbilityType.SYNERGY_CLOWN)]
public class Clown : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(1, () =>
        {
            Owner.AddAbility(AbilityType.MYTHIC_JOKER);
        });
    }
}