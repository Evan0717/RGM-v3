using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using UnityEngine;

namespace RGM.Modes;

public class ABattleEventHandler(ABattle aBattle)
{
    internal void RegisterEvents()
    {
        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Player.Jumping += OnJumping;
    }

    public void OnVerified(VerifiedEventArgs ev)
    {
        if (aBattle.PlayerWorkstations.ContainsKey(ev.Player))
            return;

        aBattle.PlayerWorkstations.Add(ev.Player, new List<WorkstationController>());
        aBattle.PlayerAbilities.Add(ev.Player, new List<Ability>());
    }

    public void OnJumping(JumpingEventArgs ev)
    {
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