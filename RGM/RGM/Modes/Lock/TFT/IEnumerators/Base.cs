using DAONTFT.Core.Classes;
using CustomPlayerEffects;
using Discord;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Hints;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.UI.Utilities;
using InventorySystem.Items;
using MEC;
using Mirror;
using MultiBroadcast.API;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using SLPlayerRotation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using RGM.API.Features;

namespace DAONTFT.Core.IEnumerators
{
    public static class Enumerator
    {
        public static IEnumerator<float> UpgradeDisplay(Player Owner)
        {
            Hint hint = new Hint
            {
                Text = $"",
            };

            while (true)
            {
                if (Owner.IsAlive)
                {
                    List<string> queue = new();

                    if (PlayerTFTAbilities.TryGetValue(Owner, out var abilities))
                    {
                        foreach (var ability in abilities)
                        {
                            queue.Add($"{ability.Data.GetFormattedName()}ㅣ{ability.Data.Description}");
                        }
                    }
                    else
                    {
                        queue.Add("증강 정보 없음");
                    }

                    hint = new Hint
                    {
                        Text = $"<size=15>{string.Join("\n", queue)}</size>",
                        Id = "증강 리스트",
                        XCoordinate = -300,
                        YCoordinate = 100,
                        Alignment = HintAlignment.Right
                    };

                    Owner.AddCustomHint(hint);

                    yield return Timing.WaitForSeconds(1);

                    Owner.RemoveHint(hint);
                }
                else if (Owner.Role is SpectatorRole spectator && spectator.SpectatedPlayer != null)
                {
                    List<string> queue = new();

                    if (PlayerTFTAbilities.TryGetValue(spectator.SpectatedPlayer, out var abilities))
                    {
                        foreach (var ability in abilities)
                        {
                            queue.Add($"{ability.Data.GetFormattedName()}ㅣ{ability.Data.Description}");
                        }
                    }
                    else
                    {
                        queue.Add("증강 정보 없음");
                    }

                    hint = new Hint
                    {
                        Text = $"<size=15>{string.Join("\n", queue)}</size>",
                        Id = "증강 리스트",
                        XCoordinate = -300,
                        YCoordinate = 100,
                        Alignment = HintAlignment.Left
                    };

                    Owner.AddCustomHint(hint);

                    yield return Timing.WaitForSeconds(1);

                    Owner.RemoveHint(hint);
                }
                else
                {
                    yield return Timing.WaitForSeconds(1);
                }
            }
        }
    }
}
