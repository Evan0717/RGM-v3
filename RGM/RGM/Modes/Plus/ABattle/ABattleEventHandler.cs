using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using UnityEngine;

using Exiled.Events.EventArgs.Scp079;
using RGM.API.DataBases;
using PlayerRoles;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Scp1507;

using static RGM.Variables.Variable;
using Exiled.API.Enums;

namespace RGM.Modes;

public class ABattleEventHandler(ABattle aBattle)
{
    public static ABattleEventHandler Instance;

    internal void RegisterEvents()
    {
        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        Exiled.Events.Handlers.Player.Jumping += OnJumping;
        Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        Exiled.Events.Handlers.Player.Died += OnDied;

        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;

        Exiled.Events.Handlers.Scp1507.SpawningFlamingos += OnSpawningFlamingos;
    }

    internal void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Verified -= OnVerified;
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        Exiled.Events.Handlers.Player.Jumping -= OnJumping;
        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        Exiled.Events.Handlers.Player.Died -= OnDied;

        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;

        Exiled.Events.Handlers.Scp1507.SpawningFlamingos -= OnSpawningFlamingos;
    }

    private void OnVerified(VerifiedEventArgs ev)
    {
        Verified(ev.Player);
    }

    public void Verified(Player player)
    {
        if (!aBattle.PlayerWorkstations.ContainsKey(player))
        {
            aBattle.PlayerWorkstations.Add(player, new List<WorkstationController>());
            aBattle.PlayerAbilities.Add(player, new List<Ability>());
            aBattle.IsSelecting.Add(player, false);
            aBattle.IsLifeUsed.Add(player, false);

            aBattle.ExtraModeNotion(player);
        }
    }

    private void OnSpawned(SpawnedEventArgs ev)
    {
        Timing.RunCoroutine(Spawned(ev.Player));
    }

    public IEnumerator<float> Spawned(Player player)
    {
        yield return Timing.WaitForSeconds(1);

        if (player.IsAlive)
        {
            ABattle.ApplyPrelude(player);
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
                    if (!ABattle.CurrentExtraModes.Contains("대출") && aBattle.PlayerWorkstations[ev.Player].Contains(controller))
                        return;

                    if (ABattle.CurrentExtraModes.Contains("대출") && aBattle.PlayerWorkstations[ev.Player].Contains(controller) && Random.Range(1, 6) == 1)
                    {
                        if (GodModePlayers.Contains(ev.Player))
                            GodModePlayers.Remove(ev.Player);

                        ev.Player.RemoveAllAbilities();
                        ev.Player.Kill("욕심을 부리다가 아사했습니다.");
                        return;
                    }

                    if (aBattle.Selections.ContainsKey(ev.Player))
                        aBattle.Selections[ev.Player].Clear();

                    if (ABattle.CurrentExtraModes.Contains("대출") && aBattle.PlayerWorkstations[ev.Player].Contains(controller))
                        aBattle.StartSelect(ev.Player);

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

    private IEnumerator<float> OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (ev.Player.IsDead || ev.NewRole.IsDead() || ev.Player.GetAbilities().Count() == 0)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () => 
            {
                aBattle.Reset(ev.Player);
            });
        }
        else
        {
            if (ev.Reason == SpawnReason.Escaped)
            {
                Timing.RunCoroutine(aBattle.RestoreAbilities(new List<Player>() { ev.Player }));
            }
        }

        yield break;
    }

    private void OnDied(DiedEventArgs ev)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            aBattle.Reset(ev.Player);
        });
    }

    public void OnPinging(PingingEventArgs ev)
    {
        Vector3 pos = ev.Position;

        if (Physics.Raycast(new Vector3(pos.x, pos.y + 1, pos.z), Vector3.down, out var hit, 5, (LayerMask)1))
        {
            if (hit.transform != null)
            {
                var controller = hit.transform.GetComponentInParent<WorkstationController>();

                if (controller != null)
                {
                    if (!ABattle.CurrentExtraModes.Contains("대출") && aBattle.PlayerWorkstations[ev.Player].Contains(controller))
                        return;

                    if (ABattle.CurrentExtraModes.Contains("대출"))
                    {
                        if (aBattle.PlayerWorkstations[ev.Player].Contains(controller) && Random.Range(1, 6) == 1)
                        {
                            if (GodModePlayers.Contains(ev.Player))
                                GodModePlayers.Remove(ev.Player);

                            ev.Player.RemoveAllAbilities();
                            ev.Player.Kill("욕심을 부리다가 아사했습니다.");
                            return;
                        }
                    }

                    if (aBattle.Selections.ContainsKey(ev.Player))
                        aBattle.Selections[ev.Player].Clear();

                    if (ABattle.CurrentExtraModes.Contains("대출"))
                        aBattle.StartSelect(ev.Player);

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

    private void OnSpawningFlamingos(SpawningFlamingosEventArgs ev)
    {
        Timing.RunCoroutine(aBattle.RestoreAbilities(ev.SpawnablePlayers.ToList()));
    }
}