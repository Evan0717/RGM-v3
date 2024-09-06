using MEC;
using PlayerRoles;
using Exiled.API.Features;
using Exiled.API.Enums;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features.Roles;

namespace RGM.Modes
{
    public class Leader
    {
        public static Leader Instance;

        public Dictionary<string, Player> Leaders = new Dictionary<string, Player>();
        public List<Player> respawned = new List<Player>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }
        
        public IEnumerator<float> OnModeStarted()
        {
            Leaders = new Dictionary<string, Player>
            {
                { "CDP", null },
                { "RSC", null },
                { "FCD", null },
                { "SCP", null },
                { "NTF", null },
                { "CHI", null },
            };
            Round.IsLocked = false;
            yield return Timing.WaitForSeconds(1f);
            SelectLeader(Player.List.Where(x => x.Role == RoleTypeId.ClassD).ToList(), "CDP");
            SelectLeader(Player.List.Where(x => x.Role == RoleTypeId.Scientist).ToList(), "RSC");
            SelectLeader(Player.List.Where(x => x.Role == RoleTypeId.FacilityGuard).ToList(), "FGD");
            SelectLeader(Player.List.Where(x => x.IsScp).ToList(), "SCP");
            yield break;
        }

        public void OnChaningRole(Exiled.Events.EventArgs.Player.ChangingRoleEventArgs ev)
        {
            if (ev.Reason != SpawnReason.Respawn) return;
            respawned.Add(ev.Player);
        }

        public void OnRespawningTeam(Exiled.Events.EventArgs.Server.RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == SpawnableTeamType.None) return;

            Timing.CallDelayed(1f, () =>
            {
                SelectLeader(respawned, (byte)ev.NextKnownTeam == 1 ? "CHI" : "NTF");
                respawned.Clear();
            });
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            foreach (var l in Leaders.ToList())
            {
                if (l.Value == ev.Player)
                {
                    ev.Player.CustomInfo = null;
                    Leaders[l.Key] = null;
                    break;
                }
            }
        }

        public void SelectLeader(List<Player> players, string team)
        {
            if (!Leaders.ContainsKey(team.ToUpper())) return;

            Player p = players.RandomItem();
            switch (team)
            {
                default:
                    p.Broadcast(10, $"<size=25>YOU ARE A <color=#CEF6E3>LEADER</color>.</size>");
                    Leaders[team] = p;
                    p.CustomInfo = "LEADER";
                    p.MaxHealth = p.MaxHealth + 100;
                    p.Health += 100;
                    p.ClearInventory();
                    p.AddItem(ItemType.KeycardO5);
                    p.AddItem(ItemType.GunLogicer);
                    p.SetAmmo(AmmoType.Nato762, 200);
                    p.AddItem(ItemType.ArmorHeavy);
                    p.AddItem(ItemType.SCP500);
                    p.AddItem(ItemType.SCP500);
                    p.AddItem(ItemType.SCP268);
                    p.AddItem(ItemType.GrenadeHE);
                    break;
                case "CHI":
                case "NTF":
                    if (Leaders[team] != null) break;

                    p.Broadcast(10, $"<size=25>YOU ARE A <color=#CEF6E3>LEADER</color>.</size>");
                    Leaders[team] = p;
                    p.CustomInfo = "LEADER";
                    p.MaxHealth = p.MaxHealth + 100;
                    p.Health += 100;
                    p.ClearInventory();
                    p.AddItem(ItemType.KeycardO5);
                    p.AddItem(ItemType.GunLogicer);
                    p.SetAmmo(AmmoType.Nato762, 200);
                    p.AddItem(ItemType.ArmorHeavy);
                    p.AddItem(ItemType.SCP500);
                    p.AddItem(ItemType.SCP500);
                    p.AddItem(ItemType.SCP268);
                    p.AddItem(ItemType.GrenadeHE);
                    break;
                case "SCP":
                    p.Broadcast(10, $"<size=25>YOU ARE A <color=#F5A9A9>LEADER</color>.</size>");
                    Leaders[team] = p;
                    if (p.Role == RoleTypeId.Scp079)
                    {
                        if (p.Role is Scp079Role scp079)
                        {
                            scp079.Level = 5;
                        }
                    }
                    else
                    {
                        p.CustomInfo = "LEADER";
                    }
                    break;
            }
        }

        Dictionary<Team, bool> buffedTeam = new Dictionary<Team, bool>();

        public void OnEscapingEvent(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (!(Leaders.Where(x => x.Value == ev.Player).Count() > 0)) return;
            foreach (var p in Player.List.Where(x => x != ev.Player && x.Role.Team == ev.NewRole.GetTeam()))
            {
                Player.List.ToList().ForEach(x => x.Broadcast(10, $"LEADER HAS ESCAPED"));
                if (ev.Player.Role.Team == ev.NewRole.GetTeam()) // 순수 탈출
                {
                    if (buffedTeam.ContainsKey(ev.NewRole.GetTeam()))
                    {
                        buffedTeam[ev.NewRole.GetTeam()] = true;
                    }
                    else
                    {
                        buffedTeam.Add(ev.NewRole.GetTeam(), true);
                    }
                    p.MaxHealth = p.MaxHealth + 50;
                    p.Health += 50;
                }
                else // 체포 탈출
                {
                    if (buffedTeam.ContainsKey(ev.NewRole.GetTeam()))
                    {
                        buffedTeam[ev.NewRole.GetTeam()] = false;
                    }
                    else
                    {
                        buffedTeam.Add(ev.NewRole.GetTeam(), false);
                    }
                    p.MaxHealth = p.MaxHealth + 25;
                    p.Health += 25;
                }
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Timing.CallDelayed(0.5f, () =>
            {
                if (buffedTeam.TryGetValue(ev.Player.Role.Team, out bool full))
                {
                    if (full)
                    {
                        ev.Player.MaxHealth = ev.Player.MaxHealth + 50;
                        ev.Player.Health += 50;
                    }
                    else
                    {
                        ev.Player.MaxHealth = ev.Player.MaxHealth + 25;
                        ev.Player.Health += 25;
                    }
                }
            });
        }
    }
}
