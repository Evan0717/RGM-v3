using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MapEditorReborn.Events.EventArgs;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using UnityEngine;
using MultiBroadcast.API;

namespace RGM.Modes;

public class ABattleEventHandler(ABattle aBattle)
{
    internal void RegisterEvents()
    {
        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Player.Jumping += OnJumping;
        Exiled.Events.Handlers.Player.Died += OnDied;

        MapEditorReborn.Events.Handlers.Map.LoadingMap += OnLoadingMap;
    }

    private void OnVerified(VerifiedEventArgs ev)
    {
        if (aBattle.PlayerWorkstations.ContainsKey(ev.Player))
        {
            aBattle.PlayerWorkstations.Add(ev.Player, new List<WorkstationController>());
            aBattle.PlayerAbilities.Add(ev.Player, new List<Ability>());
            aBattle.IsSelecting.Add(ev.Player, false);
            aBattle.IsLifeUsed.Add(ev.Player, false);
        }
    }

    private void OnJumping(JumpingEventArgs ev)
    {
        if (Physics.Raycast(ev.Player.Position, Vector3.down, out var hit, 5, (LayerMask)1))
        {
            if (hit.transform != null)
            {
                var controller = hit.transform.GetComponentInParent<WorkstationController>();

                if (controller != null)
                {
                    if (aBattle.PlayerWorkstations[ev.Player].Contains(controller))
                        return;

                    if (aBattle.Selections.ContainsKey(ev.Player))
                    {
                        ev.Player.ShowHint("<size=20>이미 능력 선택창이 열려 있습니다.\n이전 선택을 먼저 완료해주세요.</size>", 1.2f);
                        return;
                    }

                    if (!aBattle.PlayerWorkstations.TryGetValue(ev.Player, out var workstations))
                    {
                        aBattle.PlayerWorkstations.Add(ev.Player, [controller]);

                        aBattle.StartSelect(ev.Player);
                    }
                    else
                    {
                        if (!workstations.Contains(controller))
                        {
                            workstations.Add(controller);

                            aBattle.StartSelect(ev.Player);
                        }
                    }
                }
            }
        }
    }

    private void OnDied(DiedEventArgs ev)
    {
        ev.Player.RemoveAllAbilities();

        aBattle.PlayerWorkstations[ev.Player].Clear();
    }

    private void OnLoadingMap(LoadingMapEventArgs ev)
    {
        if (ev.NewMap.Name == "ABattle")
            Player.List.ToList().ForEach(x => x.AddBroadcast(10, "<size=25><b><i><color=#FF00EA>피</color><color=#EF00EB>버</color> <color=#CF00ED>모</color><color=#BF00EF>드</color><color=#AF00F0>가</color> <color=#8F00F3>활</color><color=#7F00F4>성</color><color=#6F00F5>화</color><color=#5F00F7>되</color><color=#4F00F8>었</color><color=#3F00F9>습</color><color=#2F00FB>니</color><color=#1F00FC>다</color><color=#0F00FD>!</color></i></b></size>"));
    }
}