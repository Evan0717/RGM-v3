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

[TFTAbility("응당한 대가", "즉시 55 경험치를 얻습니다. 20초 간 전력이 0이 됩니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.JustPrice, "💸")]
public class JustPrice : TFTAbility
{
    public override void OnEnabled()
    {
        IEnumerator<float> enumerator()
        {
            if (Owner.Role is Scp079Role scp079)
                scp079.AddExperience(55);

            for (int i = 0; i < 20; i++)
            {
                if (Owner.Role is Scp079Role scp079_)
                    scp079_.Energy = 0;

                yield return Timing.WaitForSeconds(1f);
            }
        }

        Timing.RunCoroutine(enumerator());
    }

    public override void OnDisabled()
    {
    }
}
