using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Scp079;
using LabApi.Features.Wrappers;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.Scp079;

[TFTAbility("카메라 플래시", "핑ㅣ해당 위치에 섬광탄을 떨굽니다. (쿨타임 24초)", TFTAbilityLevel.Keter, TFTAbilityCategory.Scp079, TFTAbilityPoint.Continuous, TFTAbilityType.CameraFlash, "📷")]
public class CameraFlash : TFTAbility
{
    float cooldown = 0;
    CoroutineHandle _flashCooldown;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;

        _flashCooldown = Timing.RunCoroutine(flashCooldown());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;

        Timing.KillCoroutines(_flashCooldown);
    }

    IEnumerator<float> flashCooldown()
    {
        while (true)
        {
            if (cooldown > 0)
                cooldown--;

            Data.Description = $"핑ㅣ해당 위치에 섬광탄을 떨굽니다. ({(cooldown == 0 ? "사용 가능" : $"{cooldown}초 남음")})";

            yield return Timing.WaitForSeconds(1f);
        }
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (cooldown <= 0)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                var g = (FlashGrenade)Exiled.API.Features.Items.Item.Create(ItemType.GrenadeFlash, ev.Player);
                g.FuseTime = 4f;
                g.SpawnActive(ev.Position, ev.Player);

                LightSourceToy light = LightSourceToy.Create(ev.Position);
                light.Position = ev.Position;
                light.Range = 5;
                light.Color = new Color(1, 1, 0, 1);
                light.Rotation = Quaternion.Euler(0, 0, 0);


                Timing.CallDelayed(5, () =>
                {
                    light.Destroy();
                });

                cooldown = 24;
            });
        }
    }
}
