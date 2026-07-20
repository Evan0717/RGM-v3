using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp173;

[Ability("신기루", "데미지를 입을 때 6% 확률로 1초 동안 투명화가 됩니다.", AbilityCategory.Common, AbilityType.COMMON_SCP173_MIRAGE, RoleAbility.Scp173)]
public class Mirage : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (UnityEngine.Random.Range(1, 101) <= 6)
        {
            Owner.EnableEffect(EffectType.Invisible, 1, 1f * Owner.AbilityCount(AbilityType.COMMON_SCP173_MIRAGE));
        }
    }
}
