using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("방어 · 통달", "방어력을 20% 얻습니다. 추가로, 모든 받는 데미지가 최대 70까지만 적용됩니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddDefense3, "⛔")]
public class AddDefense3 : TFTAbility
{
    const float MaxDamage = 70f;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;

        Owner.AddEffect(EffectType.DamageReduction, 40);
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.IsInstantKill)
        {
            ev.IsAllowed = false;
            ev.Player.Hurt(MaxDamage, ev.DamageHandler.Type);
            return;
        }

        if (ev.DamageHandler.Damage > MaxDamage)
            ev.DamageHandler.Damage = MaxDamage;
    }
}
