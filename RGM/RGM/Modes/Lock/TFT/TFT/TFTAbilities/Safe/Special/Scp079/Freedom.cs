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
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.Scp079;

[TFTAbility("자유", "이미 작동된 발전기를 제외한 나머지 발전기들은 2분 간 잠깁니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.Freedom, "🔒")]
public class Freedom : TFTAbility
{
    public override void OnEnabled()
    {
        IEnumerator<float> enumerator()
        {
            for (int i = 0; i < 120; i++)
            {
                foreach (var generator in Generator.List)
                {
                    if (generator.State != GeneratorState.Engaged)
                        generator.State = GeneratorState.Unlocked;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        Timing.RunCoroutine(enumerator());
    }

    public override void OnDisabled()
    {
    }
}
