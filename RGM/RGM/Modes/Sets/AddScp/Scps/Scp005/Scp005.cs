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
using static RGM.Variables.Variable;
using static UnityEngine.UI.GridLayoutGroup;

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
                    foreach (var player in PlayerManager.List)
                    {
                        if (player.CurrentItem != null && player.CurrentItem.Serial == item.Serial)
                        {
                            player.AddHint("SCP-005",
"""
<size=25>이 동전은 <color=red>SCP-005</color>(<color=#a4fc16>Safe</color>)입니다.</size>
<size=20>모든 것을 열 수 있습니다.</size>
""", 0.12f);
                        }
                    }

                    yield return Timing.WaitForSeconds(0.1f);
                }
            }

            Timing.RunCoroutine(displayDesc());

            void OnInteractingDoor(InteractingDoorEventArgs ev)
            {
                if (ev.Player.CurrentItem == item)
                {
                    if (ev.Door.Type == DoorType.Scp079First)
                    {
                        ev.Player.AddHint("헤비도어", "이 헤비도어는 능력으로 개폐가 불가능합니다.", 1.2f);
                        return;
                    }

                    if (Warhead.IsInProgress)
                    {
                        ev.Player.AddHint("알파 핵탄투", "알파 핵탄투가 작동 중일때는 문을 개폐할 수 없습니다.", 1.2f);
                        return;
                    }

                    ev.IsAllowed = false;

                    if (ev.Door.IsOpen)
                        ev.Door.IsOpen = false;

                    else
                        ev.Door.IsOpen = true;
                }
            }

            void OnInteractingLocker(InteractingLockerEventArgs ev)
            {
                if (ev.Player.CurrentItem == item)
                {
                    ev.IsAllowed = false;

                    if (ev.InteractingChamber.IsOpen)
                        ev.InteractingChamber.IsOpen = false;

                    else
                        ev.InteractingChamber.IsOpen = true;
                }
            }

            void OnOpeningGenerator(OpeningGeneratorEventArgs ev)
            {
                if (ev.Player.CurrentItem == item)
                {
                    ev.IsAllowed = false;

                    ev.Generator.IsOpen = false;
                }
            }

            void OnClosingGenerator(ClosingGeneratorEventArgs ev)
            {
                if (ev.Player.CurrentItem == item)
                {
                    ev.IsAllowed = false;

                    ev.Generator.IsOpen = true;
                }
            }

            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInteractingLocker;
            Exiled.Events.Handlers.Player.OpeningGenerator += OnOpeningGenerator;
            Exiled.Events.Handlers.Player.ClosingGenerator += OnClosingGenerator;

            return item;
        }
    }
}
