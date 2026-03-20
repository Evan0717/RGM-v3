using DAONTFT.Core.Classes;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

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
            for (int i = 0; i < 180; i++)
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
            RoaringSoundCooldown = 180;

            Function.PlayGlobalAudio("GmanRoaringSound");

            foreach (var player in Player.List.Where(x => !x.IsNPC && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, x.ReferenceHub) && x.IsAlive))
            {
                player.EnableEffect(EffectType.Flashed, 1, 0.3f);
                player.EnableEffect(EffectType.Blinded, 1, 7.5f);
                player.EnableEffect(EffectType.SinkHole, 1, 12f);
                player.EnableEffect(EffectType.Slowness, 150, 5.5f);
            }

            yield return Timing.WaitForSeconds(0.65f);

            for (int i = 1; i < 71; i++)
            {
                Warhead.Shake();

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
