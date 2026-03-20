using CustomPlayerEffects;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Keter.Human;

[TFTAbility("악마와의 계약", "죽기 전까지 투명해지는 대신 최대 체력이 1이 됩니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.DevilContract, "👿")]
public class DevilContract : TFTAbility
{
    CoroutineHandle _invisible;

    public override void OnEnabled()
    {
        Owner.MaxHealth = 1;
        Owner.Health = 1;

        _invisible = Timing.RunCoroutine(invisible());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_invisible);
    }

    IEnumerator<float> invisible()
    {
        while (Owner.IsAlive)
        {
            if (Owner.TryGetEffect(EffectType.Invisible, out StatusEffectBase statusEffect))
            {
                if (!statusEffect.IsEnabled)
                {
                    Owner.AddEffect(EffectType.Invisible, 1);
                }
            }

            yield return Timing.WaitForOneFrame;
        }
    }
}
