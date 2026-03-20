using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Scp173;

[TFTAbility("신기루", "공격에 성공할 경우, 3초 동안 투명화 상태가 됩니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp173, TFTAbilityPoint.Continuous, TFTAbilityType.Mirage, "😧")]
public class Mirage : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    void OnDied(DiedEventArgs ev)
    {
        if (ev.Attacker != null && ev.Attacker == Owner)
        {
            Owner.AddEffect(EffectType.Invisible, 1, 3);
        }
    }
}
