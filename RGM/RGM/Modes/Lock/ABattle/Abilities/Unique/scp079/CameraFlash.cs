using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Scp079;
using MEC;
using UnityEngine;
using LabApi.Features.Wrappers;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("카메라 플래시", "핑이 찍힌 장소에 점화된 섬광탄이 생성됩니다. (쿨타임 24초)", AbilityCategory.Common, AbilityType.COMMON_SCP079_CAMERAFLASH, RoleAbility.Scp079)]
public class CameraFlash : Ability
{
    bool isScp079Cooldown = false;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (!isScp079Cooldown)
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

                isScp079Cooldown = true;

                Timing.CallDelayed(24, () =>
                {
                    isScp079Cooldown = false;
                });
            });
        }
    }
}
