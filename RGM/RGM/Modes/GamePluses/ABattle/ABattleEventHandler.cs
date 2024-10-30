using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using UnityEngine;

namespace RGM.Modes;

public class ABattleEventHandler(ABattle aBattle)
{
    internal void RegisterEvents()
    {
        Exiled.Events.Handlers.Player.Jumping += OnJumping;
    }

    private void OnJumping(JumpingEventArgs ev)
    {
        if (ABattle.Instance.Selections.ContainsKey(ev.Player))
        {
            ev.Player.ShowHint("<size=20>이미 능력 선택창이 열려 있습니다.\n이전 선택을 먼저 완료해주세요.</size>", 1.2f);
            return;
        }

        if (Physics.Raycast(ev.Player.Position, Vector3.down, out var hit, 5, (LayerMask)1))
        {
            if (hit.transform != null)
            {
                var controller = hit.transform.GetComponentInParent<WorkstationController>();

                if (controller != null)
                {
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
}