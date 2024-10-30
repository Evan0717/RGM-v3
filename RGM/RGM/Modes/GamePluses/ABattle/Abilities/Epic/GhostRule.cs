using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Epic;

[Ability("고스트룰", "유령이 되어 문을 통과할 수 있게 됩니다.", AbilityCategory.Epic, AbilityType.EPIC_GHOSTRULE)]
public class GhostRule : Ability
{
    CoroutineHandle _ghost;

    public override void OnEnabled()
    {
        _ghost = Timing.RunCoroutine(Ghost());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_ghost);
    }

    public IEnumerator<float> Ghost()
    {
        while (true)
        {
            if (Physics.Raycast(Owner.Position, Owner.CameraTransform.forward, out RaycastHit hit, 1f, (LayerMask)1))
            {
                if (hit.transform.TryGetComponentInParent<DoorObject>(out DoorObject comp))
                    Owner.EnableEffect(EffectType.Ghostly, 0.5f);
            }

            yield return Timing.WaitForSeconds(0.1f);
        }
    }
}
