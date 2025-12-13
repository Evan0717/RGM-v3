using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp106;

[Ability("끈적한 늪", "4m 내의 인간들을 느리게 만듭니다.", AbilityCategory.Scp106, AbilityType.SCP106_STICKYSWAMP)]
public class StickySwamp : Ability
{
    CoroutineHandle _stickySwamp1;

    public override void OnEnabled()
    {
        _stickySwamp1 = Timing.RunCoroutine(StickySwamp1());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_stickySwamp1);
    }

    public IEnumerator<float> StickySwamp1()
    {
        while (true)
        {
            foreach (var near in PlayerManager.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, Owner.Position) <= 4))
            {
                if (Owner != near && HitboxIdentity.IsEnemy(Owner.ReferenceHub, near.ReferenceHub))
                    near.EnableEffect(EffectType.SinkHole, 10, 0.5f);
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
