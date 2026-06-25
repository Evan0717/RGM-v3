using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using static DAONTFT.Core.Variables.Base;
using RGM.API.Features;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("생존 전문가", "죽음에 이르는 피해를 받았을 때, 3초 간 살아남습니다. (쿨타임 2분)", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.Survivor, "💝")]
public class Survivor : TFTAbility
{
    bool isEnabled = false;
    int cooldown = 0;
    CoroutineHandle _cooldownProcess;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;

        _cooldownProcess = Timing.RunCoroutine(cooldownProcess());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;

        Timing.KillCoroutines(_cooldownProcess);
    }

    IEnumerator<float> cooldownProcess()
    {
        while (true)
        {
            if (cooldown > 0)
                cooldown--;

            Data.Description = $"죽음에 이르는 피해를 받았을 때, 3초 간 살아남습니다. ({(cooldown == 0 ? "사용 가능" : $"재사용까지 { cooldown}초")})";

            yield return Timing.WaitForSeconds(1f);
        }
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner || IsLifeUsed[Owner] || BlockDamageTypes.Contains(ev.DamageHandler.Type))
            return;

        if (cooldown <= 0)
        {
            if (!isEnabled)
            {
                isEnabled = true;

                ev.IsAllowed = false;

                ev.Player.EnableEffect(EffectType.Blinded, 1, 3);
                ev.Player.AddEffect(EffectType.MovementBoost, 10, 3);

                GodModePlayers.Add(ev.Player);

                Timing.CallDelayed(4f, () =>
                {
                    if (GodModePlayers.Contains(ev.Player))
                        GodModePlayers.Remove(ev.Player);

                    isEnabled = false;
                });

                IsLifeUsed[Owner] = true;

                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    IsLifeUsed[Owner] = false;
                });

                cooldown = 120;
            }
        }
    }
}
