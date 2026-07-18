using MEC;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.DUMMY_NOAFK, AbilityType.DUMMY_NOAFK, AbilityType.DUMMY_NOAFK)]
[Ability("AFK", "<자리 비움 3회 완료> 게임 하고 싶은 거 맞죠..? 용기가 가상하시니 선물을 드릴게요.", AbilityCategory.Synergy, AbilityType.SYNERGY_AFK)]
public class AFK : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            for (int i = 0; i < 3; i++) {
                Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, AbilityCategory.Epic, 1)[0]);
            }
            for (int i = 0; i < 5; i++) {
                Owner.AddAbility(ABattle.Instance.GetRandomAbilities(
                    Owner, AbilityCategory.Rare, 1,[AbilityType.RARE_DND, AbilityType.RARE_TELEPORTATION])[0]);
            }
            for (int i = 0; i < 7; i++) {
                Owner.AddAbility(ABattle.Instance.GetRandomAbilities(
                    Owner, AbilityCategory.Common, 1,[AbilityType.NORMAL_REROLL])[0]);
            }
        });
    }

    public override void OnDisabled()
    {
    }
}
