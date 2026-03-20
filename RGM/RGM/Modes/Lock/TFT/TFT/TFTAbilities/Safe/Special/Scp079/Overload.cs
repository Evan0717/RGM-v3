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

[TFTAbility("과부화", "1분 동안 전력이 무제한이 됩니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.Overload, "🔮")]
public class Overload : TFTAbility
{
    public override void OnEnabled()
    {
        IEnumerator<float> _overload()
        {
            for (int i = 0; i < 60; i++)
            {
                if (Owner.Role is Scp079Role scp079)
                {
                    scp079.Energy = scp079.MaxEnergy;
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        Timing.RunCoroutine(_overload());
    }

    public override void OnDisabled()
    {
    }
}
