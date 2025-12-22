using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using ProjectMER.Features;
using MEC;
using RGM.API.Features;
using UnityEngine;
using RGM.API.DataBases;
using Exiled.API.Features.DamageHandlers;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.RARE_SALAMANDRA, AbilityType.RARE_UNDINE, AbilityType.RARE_GNOME, AbilityType.RARE_SYLPH)]
[Ability("드루이드", "<살라만드라, 운디네, 노움, 실프> 4대 정령의 가호가 당신과 함께합니다. 80% 확률(<color=red>SCP</color>의 경우 50%)로 상대방의 공격을 반사합니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_DRUID)]
public class Druid : Ability
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
        if (ev.Player != Owner || ev.Attacker == null || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub) || Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
            return;

        float reflectChance = ev.Player.IsScpRole() ? 0.5f : 0.8f;

        if (UnityEngine.Random.Range(0f, 1f) <= reflectChance)
        {
            ev.IsAllowed = false;

            ev.Attacker.Hit(ev.Player, ev.Amount);
            ev.Attacker.AddHint("드루이드", "당신의 공격이 반사되었습니다.");
            ev.Player.AddHint("드루이드", $"상대의 공격이 반사되었습니다.");
        }
    }
}
