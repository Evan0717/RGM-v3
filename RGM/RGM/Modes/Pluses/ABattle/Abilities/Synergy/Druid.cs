using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using MapEditorReborn.API.Features;
using MEC;
using RGM.API.Features;
using UnityEngine;
using RGM.API.DataBases;
using Exiled.API.Features.DamageHandlers;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.RARE_SALAMANDRA, AbilityType.RARE_UNDINE, AbilityType.RARE_GNOME, AbilityType.RARE_SYLPH)]
[Ability("드루이드", "<살라만드라, 운디네, 노움, 실프> 4대 정령의 가호가 당신과 함께합니다. 90% 확률로 상대방의 공격을 반사합니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_DRUID)]
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
        if (ev.Player != Owner || ev.Attacker == null || Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
            return;

        if (UnityEngine.Random.Range(1, 11) != 1)
        {
            ev.IsAllowed = false;

            ev.Attacker.Hurt(ev.Player, ev.DamageHandler.Damage, ev.DamageHandler.Type, new DamageHandlerBase.CassieAnnouncement(null), deathText: "정령의 힘에 의해 사망하였습니다.");
            ev.Attacker.ShowHint("당신의 공격이 반사되었습니다.");
            ev.Player.ShowHint($"상대의 공격이 반사되었습니다.");
        }
    }
}
