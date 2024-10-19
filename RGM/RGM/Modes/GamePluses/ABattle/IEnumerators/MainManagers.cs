using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using PlayerRoles;
using MEC;
using Exiled.API.Features.Roles;

using static RGM.Variables.ServerManagers;

using static RGM.Modes.ABattleVariables.Abilities;
using static RGM.Modes.ABattleVariables.AbiltiyManagers;
using static RGM.Modes.ABattleVariables.MainManagers;
using static RGM.Modes.ABattleVariables.Cooldowns;
using static RGM.Modes.ABattleVariables.Serials;

using static RGM.Modes.ABattleFunctions.AbilityManagers;
using static RGM.Modes.ABattleFunctions.MainManagers;
using static RGM.Modes.ABattleFunctions.SpecificAbilities;

namespace RGM.Modes.ABattleIEnumerators
{
    public static class MainManagers
    {
        public static IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List.Where(x => !x.IsNPC))
            {
                PlayerWorkstation.Add(player, new List<Vector3>());
                PlayerAbilities.Add(player, new List<string>());
            }

            Timing.CallDelayed(UnityEngine.Random.Range(1, 11), () =>
            {
                if (UnityEngine.Random.Range(1, 6) == 1)
                    IsFeverModeEnabled = true;

                if (IsFeverModeEnabled)
                    Server.ExecuteCommand($"/mp load ABattle");
            });

            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    try
                    {
                        Hint CurrentHint = player.CurrentHint;
                        bool IsStatusHint = CurrentHint != null && (CurrentHint.Content.Contains("워크스테이션") || CurrentHint.Content.Contains("보유 업그레이드"));

                        if (CurrentHint == null || IsStatusHint)
                        {
                            if (player.IsAlive)
                                ShowStatus(player);

                            else
                            {
                                if (player.Role is SpectatorRole spectator)
                                {
                                    if (spectator.SpectatedPlayer != null && spectator.SpectatedPlayer.CurrentHint != null)
                                        player.ShowHint(spectator.SpectatedPlayer.CurrentHint.Content, 1.2f);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        public static IEnumerator<float> RequestManager()
        {
            while (true)
            {
                foreach (var Request in Requests.ToList())
                {
                    try
                    {
                        string[] req = Request.Split('/');

                        if (req[0] == "ABattle")
                        {
                            Player player = Player.Get(req[1]);

                            if (req[2] == "Add")
                            {
                                if (req[3] == "Random")
                                    AddAbility(player);

                                else
                                    AddAbility(player, req[3]);
                            }
                            else if (req[2] == "Vote")
                            {
                                string VoteNum;

                                if (req[3] == "Random")
                                    VoteNum = $"{UnityEngine.Random.Range(1, 4)}";

                                else
                                    VoteNum = req[3];

                                PlayerVotes.Add(player, VoteNum);

                                Timing.CallDelayed(1f, () =>
                                {
                                    if (PlayerVotes.ContainsKey(player))
                                        PlayerVotes.Remove(player);
                                });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                    finally
                    {
                        Requests.Remove(Request);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> SynergyManager()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    try
                    {
                        foreach (var synergy in Synergies)
                        {
                            if (synergy.Value.Skip(1).All(x => PlayerAbilities[player].Intersect(synergy.Value.Skip(1)).Count() >= synergy.Value.Skip(1).Count()))
                            {
                                if (!PlayerAbilities[player].Contains(synergy.Key))
                                    AddAbility(player, synergy.Key);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
