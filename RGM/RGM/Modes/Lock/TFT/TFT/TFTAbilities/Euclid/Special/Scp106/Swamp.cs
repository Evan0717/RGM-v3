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
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Scp106;

[TFTAbility("끈적이는 늪", "가까이 있는 인간들의 이동 속도를 느리게 만듭니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp106, TFTAbilityPoint.Once, TFTAbilityType.Swamp, "💧")]
public class Swamp : TFTAbility
{
    CoroutineHandle _swamp;

    public override void OnEnabled()
    {
        Data.Description = "3m 거리 내의 인간들에게 \"부식\" 효과를 적용합니다.";
        _swamp = Timing.RunCoroutine(swamp());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_swamp);
    }

    IEnumerator<float> swamp()
    {
        while (true)
        {
            foreach (var player in Player.List.Where(x => !x.IsScp && x.IsAlive))
            {
                if (Vector3.Distance(player.Position, Owner.Position) <= 3)
                {
                    player.AddEffect(EffectType.Corroding, 1, 1);
                }
            }

            yield return Timing.WaitForSeconds(1);
        }
    }
}
