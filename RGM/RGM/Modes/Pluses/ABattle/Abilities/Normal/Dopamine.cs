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

[Ability("도파민", "다음으로 맞는 첫 공격의 데미지를 무시하고 체력으로 흡수합니다. (최대 200HP)", AbilityCategory.Common, AbilityType.NORMAL_DOPAMINE)]
public class Dopamine : Ability
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
        if (ev.Player != Owner || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub) || Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
            return;

        ev.IsAllowed = false;
        Owner.RemoveAbility(this);
        OnDisabled();

        Owner.Heal(ev.DamageHandler.Damage > 200 ? 200 : ev.DamageHandler.Damage);

        Owner.AddAbility(AbilityType.DUMMY_DOPAMINERELEASED);
        Owner.AddHint("도파민", $"<color={ABattle.RatingColor["일반"]}>도파민</color> 효과로 인해 데미지를 무시하고 체력으로 흡수했습니다.");
    }
}
