using DAONTFT.Core.Classes;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Euclid.Scp;

[TFTAbility("본능", "가장 가까운 인간의 위치를 확인합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp, TFTAbilityPoint.Continuous, TFTAbilityType.FindLocation, "📥")]
public class FindLocation : TFTAbility
{
    CoroutineHandle _findLocation;

    public override void OnEnabled()
    {
        _findLocation = Timing.RunCoroutine(findLocation());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_findLocation);
    }

    IEnumerator<float> findLocation()
    {
        string id = "FInd Location";
        HintServiceMeow.Core.Models.Hints.Hint hint = new HintServiceMeow.Core.Models.Hints.Hint
        {
            Text = $"",
        };

        while (Owner.IsScp)
        {
            if (Owner.TryGetNearestPlayer(out Player player, out float radius, Player.List.Where(x => x.IsScp).ToList()))
            {
                hint = new HintServiceMeow.Core.Models.Hints.Hint
                {
                    Text = $"<size=20>가장 가까운 인간: {(int)radius}m</size>",
                    Id = id,
                    XCoordinate = 0,
                    YCoordinate = 900,
                    Alignment = HintAlignment.Right,
                };

                Owner.AddCustomHint(hint);

                yield return Timing.WaitForSeconds(1f);

                Owner.RemoveHint(hint);
            }
            else
            {
                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
