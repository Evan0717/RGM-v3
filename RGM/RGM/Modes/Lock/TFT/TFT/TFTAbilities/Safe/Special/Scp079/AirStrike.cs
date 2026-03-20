using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using InventorySystem.Items.Usables.Scp330;
using LabApi.Features.Wrappers;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.Scp079;

[TFTAbility("폭격", "핑ㅣ해당 위치에 고폭 수류탄을 투척합니다. (1회)", TFTAbilityLevel.Safe, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.AirStrike, "💣")]
public class AirStrike : TFTAbility
{
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

        Timing.CallDelayed(0.1f, () =>
        {
            var g = (ExplosiveGrenade)Exiled.API.Features.Items.Item.Create(ItemType.GrenadeHE, ev.Player);
            g.SpawnActive(ev.Position, ev.Player);

            LightSourceToy light = LightSourceToy.Create(ev.Position);
            light.Position = ev.Position;
            light.Range = 3;
            light.Color = new Color(1, 0, 0, 1);
            light.Rotation = Quaternion.Euler(0, 0, 0);


            Timing.CallDelayed(3, () =>
            {
                light.Destroy();
            });

            OnDisabled();
        });
    }
}
