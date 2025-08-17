using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using ProjectMER.Events.Arguments;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using RemoteAdmin;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.ServerManagers;

namespace RGM.Modes.Sets.AddScp.Scps
{
    public static class Scp005
    {
        public static Item Create()
        {
            Item item = Item.Create(ItemType.Coin);
            
            IEnumerator<float> displayDesc()
            {
                while (true)
                {
                    foreach (var player in Player.List)
                    {
                        if (player.CurrentItem != null && player.CurrentItem.Serial == item.Serial)
                        {
                            if (!player.IsBypassModeEnabled)
                                player.IsBypassModeEnabled = true;

                            player.AddHint("SCP-005",
"""
<size=25>이 동전은 <color=red>SCP-005</color>(<color=#a4fc16>Safe</color>)입니다.</size>
<size=20>모든 것을 열 수 있습니다.</size>
""", 0.12f);
                        }
                        else
                        {
                            if (player.IsBypassModeEnabled)
                                player.IsBypassModeEnabled = false;
                        }
                    }

                    yield return Timing.WaitForSeconds(0.1f);
                }
            }

            Timing.RunCoroutine(displayDesc());

            return item;
        }
    }
}
