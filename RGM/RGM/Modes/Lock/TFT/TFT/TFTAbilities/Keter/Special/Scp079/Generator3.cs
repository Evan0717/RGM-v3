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

namespace DAONTFT.Core.TFT.Keter.Scp079;

[TFTAbility("발전기Ⅲ", "초당 전력이 1.2만큼 충전됩니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Scp079, TFTAbilityPoint.Continuous, TFTAbilityType.Generator3, "🔌")]
public class Generator3 : TFTAbility
{
    CoroutineHandle _generatorLoop;

    public override void OnEnabled()
    {
        _generatorLoop = Timing.RunCoroutine(generatorLoop());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_generatorLoop);
    }

    IEnumerator<float> generatorLoop()
    {
        while (true)
        {
            if (Owner.Role is Scp079Role scp079Role)
            {
                scp079Role.Energy += 1.2f;
            }

            yield return Timing.WaitForSeconds(1);
        }
    }
}
