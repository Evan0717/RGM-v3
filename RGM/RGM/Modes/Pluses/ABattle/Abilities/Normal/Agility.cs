using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("민첩", "회피율이 3% 증가합니다.", AbilityCategory.Common, AbilityType.NORMAL_AGILITY)]
public class Agility : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Player != Owner || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub) || Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
            return;

        if (Random.Range(1, 101) < 4)
        {
            ev.IsAllowed = false;

            ev.Attacker.AddHint("이런, 미끄러져 버렸군요.", $"이런, 미끄러져 버렸군요.", 1.2f);
            ev.Player.AddHint("아슬아슬하게 회피했군요!", $"아슬아슬하게 회피했군요!", 1.2f);
        }
    }
}
