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

namespace RGM.Modes;

public class ABattleEventHandler(ABattle aBattle)
{
    public static ABattleEventHandler Instance;

    internal void RegisterEvents()
    {
        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Player.Jumping += OnJumping;
        Exiled.Events.Handlers.Player.Died += OnDied;
        Exiled.Events.Handlers.Player.Escaping += OnEscaping;

        Exiled.Events.Handlers.Scp079.GainingLevel += OnGainingLevel;

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
                        ev.Player.ShowHint("<size=20>이미 능력 선택창이 열려 있습니다.\n버그에 걸린 경우 아무 능력이나 선택해보세요.</size>", 1.2f);
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
        aBattle.Reset(ev.Player);
    }

    private IEnumerator<float> OnEscaping(EscapingEventArgs ev)
    {
        if (ev.Player.Role.Type == RoleTypeId.ClassD || ev.Player.Role.Type == RoleTypeId.Scientist)
        {
            List<AbilityType> _abilities = aBattle.PlayerAbilities[ev.Player].Select(x => x.Data.AbilityType).ToList();

            aBattle.Reset(ev.Player);

            yield return Timing.WaitForOneFrame;

            foreach (var ability in _abilities)
                ev.Player.AddAbility(ability);

            ev.Player.AddBroadcast(10, $"<size=25><b>탈출하였으므로 모든 능력을 제거한 후, 수복하였습니다.</b></size>");
        }
    }

    private void OnGainingLevel(GainingLevelEventArgs ev)
    {
        ev.Player.AddAbility(ABattle.Instance.GetRandomAbilities(AbilityCategory.Scp079, 1).First());
    }

    private void OnLoadingMap(LoadingMapEventArgs ev)
    {
        if (ev.NewMap.Name == "ABattle")
            Player.List.ToList().ForEach(x => x.AddBroadcast(10, "<size=25><b><i><color=#FF00EA>피</color><color=#EF00EB>버</color> <color=#CF00ED>모</color><color=#BF00EF>드</color><color=#AF00F0>가</color> <color=#8F00F3>활</color><color=#7F00F4>성</color><color=#6F00F5>화</color><color=#5F00F7>되</color><color=#4F00F8>었</color><color=#3F00F9>습</color><color=#2F00FB>니</color><color=#1F00FC>다</color><color=#0F00FD>!</color></i></b></size>"));
    }
}