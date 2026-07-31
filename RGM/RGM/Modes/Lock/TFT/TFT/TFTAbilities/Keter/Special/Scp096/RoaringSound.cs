using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace DAONTFT.Core.TFT.Keter.Scp096;

[TFTAbility("괴성", "능력 키(ALT)를 눌러 모든 적들을 잠시 패닉 상태에 빠지게 만듭니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Scp096, TFTAbilityPoint.ALT, TFTAbilityType.RoaringSound, "😱")]
public class RoaringSound : TFTAbility
{
    CoroutineHandle _cooldown;
    int RoaringSoundCooldown = 0;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;

        _cooldown = Timing.RunCoroutine(cooldown());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;

        Timing.KillCoroutines(_cooldown);
    }

    IEnumerator<float> cooldown()
    {
        while (true)
        {
            for (int i = 0; i < 60; i++)
            {
                if (RoaringSoundCooldown > 0)
                    RoaringSoundCooldown--;

                Data.Description = $"능력 키(ALT)를 눌러 모든 적들을 잠시 패닉 상태에 빠지게 만듭니다. ({(RoaringSoundCooldown == 0 ? "사용 가능" : $"{RoaringSoundCooldown}초 남음")})";

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }

    IEnumerator<float> OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        if (ev.Player != Owner)
            yield break;

        if (RoaringSoundCooldown <= 0)
        {
            RoaringSoundCooldown = 60;

            Tools.PlayGlobalAudio("GmanRoaringSound");

            foreach (var player in Player.List.Where(x => !x.IsNPC && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, x.ReferenceHub) && x.IsAlive))
            {
                player.EnableEffect(EffectType.Ensnared, 1, 1f);
                player.EnableEffect(EffectType.Flashed, 1, 3f);
                player.EnableEffect(EffectType.Blurred, 100, 15f);
                player.EnableEffect(EffectType.Deafened, 100, 15f);
                player.EnableEffect(EffectType.Blinded, 1, 15f);
                player.EnableEffect(EffectType.SinkHole, 1, 8f);
                player.EnableEffect(EffectType.Slowness, 120, 6f);
                player.EnableEffect(EffectType.Disabled, 100, 15f);
                player.EnableEffect(EffectType.Stained, 100, 5f);
                player.EnableEffect(EffectType.AmnesiaItems, 1, 10f);
                player.EnableEffect(EffectType.Bleeding, 1, 30f);
            }

            yield return Timing.WaitForSeconds(0.65f);
        }
    }
}
