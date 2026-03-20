using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("매복자", "10% 확률로 100% 추가 피해를 줍니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.Ambush, "🌿")]
public class Ambush : TFTAbility
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
        if (ev.Attacker == null || ev.Attacker != Owner || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (UnityEngine.Random.Range(1, 11) == 1)
        {
            ev.DamageHandler.Damage *= 2;

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Attacker.ShowHitMarker(2);
            });
        }
    }
}
