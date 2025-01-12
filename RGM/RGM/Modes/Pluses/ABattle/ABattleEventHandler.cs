using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MapEditorReborn.Events.EventArgs;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using UnityEngine;
using MultiBroadcast.API;
using Exiled.Events.EventArgs.Scp079;
using RGM.API.DataBases;
using PlayerRoles;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Scp1507;

using static RGM.Variables.ServerManagers;

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

        Exiled.Events.Handlers.Scp079.GainingLevel += OnGainingLevel;

        Exiled.Events.Handlers.Scp1507.SpawningFlamingos += OnSpawningFlamingos;

        MapEditorReborn.Events.Handlers.Map.LoadingMap += OnLoadingMap;
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

    private IEnumerator<float> OnSpawned(SpawnedEventArgs ev)
    {
        if (ev.Player.IsDead)
            yield break;

        yield return Timing.WaitForSeconds(1);

        if (aBattle.CurrentExtraMode == "골드 전주곡")
        {
            if (ev.Player.Role.Type == RoleTypeId.Scp079)
                ev.Player.AddAbility(ABattle.Instance.GetRandomAbilities(AbilityCategory.Scp079, 1).First());

            else
                ev.Player.AddAbility(ABattle.Instance.GetRandomAbilities(AbilityCategory.Epic, 1).First());
        }
        else if (aBattle.CurrentExtraMode == "프리즘 전주곡")
        {
            if (ev.Player.Role.Type == RoleTypeId.Scp079)
            {
                for (int i = 0; i < Random.Range(1, 3); i++)
                    ev.Player.AddAbility(ABattle.Instance.GetRandomAbilities(AbilityCategory.Scp079, 1).First());
            }

            else
            {
                AbilityCategory getRandom()
                {
                    if (Random.Range(1, 11) == 1)
                        return AbilityCategory.Mythic;

                    if (Random.Range(1, 11) == 1)
                        return AbilityCategory.Legend;

                    return AbilityCategory.Epic;
                }

                ev.Player.AddAbility(ABattle.Instance.GetRandomAbilities(getRandom(), 1).First());
            }
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
                    if (aBattle.CurrentExtraMode != "대출" && aBattle.PlayerWorkstations[ev.Player].Contains(controller))
                        return;

                    if (aBattle.CurrentExtraMode == "대출")
                    {
                        if (aBattle.PlayerWorkstations[ev.Player].Contains(controller) && Random.Range(1, 11) == 1)
                        {
                            IEnumerator<float> die()
                            {
                                while (ev.Player.IsAlive)
                                {
                                    if (GodModePlayers.Contains(ev.Player))
                                        GodModePlayers.Remove(ev.Player);

                                    ev.Player.Hurt(ev.Player.MaxHealth / 10, "욕심을 부리다가 아사했습니다.");

                                    yield return Timing.WaitForOneFrame;
                                }
                            }

                            Timing.RunCoroutine(die());
                            return;
                        }
                    }

                    if (aBattle.Selections.ContainsKey(ev.Player))
                        aBattle.Selections[ev.Player].Clear();

                    if (aBattle.CurrentExtraMode == "대출")
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
            Timing.RunCoroutine(aBattle.RestoreAbilities(new List<Player>() { ev.Player }));

        yield break;
    }

    private void OnGainingLevel(GainingLevelEventArgs ev)
    {
        ev.Player.AddAbility(ABattle.Instance.GetRandomAbilities(AbilityCategory.Scp079, 1).First());
    }

    private void OnSpawningFlamingos(SpawningFlamingosEventArgs ev)
    {
        Timing.RunCoroutine(aBattle.RestoreAbilities(ev.SpawnablePlayers.ToList()));
    }

    private void OnLoadingMap(LoadingMapEventArgs ev)
    {
        if (ev.NewMap.Name == "ABattle")
            Player.List.ToList().ForEach(x => x.AddBroadcast(10, "<size=25><b><i><color=#FF00EA>피</color><color=#EF00EB>버</color> <color=#CF00ED>모</color><color=#BF00EF>드</color><color=#AF00F0>가</color> <color=#8F00F3>활</color><color=#7F00F4>성</color><color=#6F00F5>화</color><color=#5F00F7>되</color><color=#4F00F8>었</color><color=#3F00F9>습</color><color=#2F00FB>니</color><color=#1F00FC>다</color><color=#0F00FD>!</color></i></b></size>"));
    }
}