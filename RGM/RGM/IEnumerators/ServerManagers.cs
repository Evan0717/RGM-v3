using MEC;
using PlayerRoles.FirstPersonControl;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;
using RGM.API.Interfaces;

using static RGM.Variables.ServerManagers;
using Exiled.API.Features.Roles;
using MultiBroadcast.API;
using Christmas.Scp2536.Gifts;
using System.Diagnostics;
using PlayerRoles.PlayableScps.Scp1507;
using InventorySystem.Items.FlamingoTapePlayer;
using Exiled.API.Enums;

namespace RGM.IEnumerators
{
    public static class ServerManagers
    {
        public static IEnumerator<float> SyncSpectatedHint()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player.Role is SpectatorRole spectator)
                    {
                        if (spectator.SpectatedPlayer != null && spectator.SpectatedPlayer.CurrentHint != null)
                            player.ShowHint(spectator.SpectatedPlayer.CurrentHint.Content, 1.2f);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> ThrowawayBroadcast()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(60 * 5, 60 * 15 + 1));

                foreach (var player in Player.List)
                    player.AddBroadcast(20, $"<size=20><b><color=#7289da>Discord</color>에 가입하여 <color=#C8FE2E>실시간 업데이트 현황</color>을 확인하고, 서버에 대한 <color=#F781D8>아이디어</color>를 나누고, <color=#FF4000>상점</color>을 이용하세요!</b></size>");
            }
        }

        public static IEnumerator<float> IsFallDown()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive))
                {
                    if (OnGround.ContainsKey(player) && !player.IsNoclipPermitted && player.Role.Type != RoleTypeId.Scp079)
                    {
                        if (player.ReferenceHub.IsGrounded())
                            OnGround[player] = 5;

                        else
                        {
                            OnGround[player] -= 0.1f;

                            if (OnGround[player] <= 0)
                            {
                                player.Kill("공허에 빨려들어갔습니다. (5초 이상 낙하)");

                                OnGround[player] = 5;
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public static IEnumerator<float> InputCooldown()
        {
            while (true)
            {
                ChatCooldown.Clear();
                EmotionCooldown.Clear();

                yield return Timing.WaitForSeconds(2f);
            }
        }

        public static IEnumerator<float> Ball()
        {
            while (!Round.IsStarted)
            {
                foreach (Player player in Player.List.Where(x => x.IsAlive))
                {
                    foreach (Transform Ball in Balls)
                    {
                        GameObject _ball = Ball.gameObject;

                        if (Vector3.Distance(_ball.transform.position, player.Position) < 2)
                        {
                            _ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                            rig.AddForce(player.GameObject.transform.forward + new Vector3(0, 0.01f, 0), ForceMode.Impulse);
                        }

                        /*
                        else if (Vector3.Distance(_ball.transform.position, player.Position) > 45)
                            _ball.transform.position = new Vector3(player.Position.x, player.Position.y + 2, player.Position.z);
                        */
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> RenewalPlayersInfo()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => PlayersInfo.ContainsKey(x.UserId) && x.IsAlive))
                {
                    PlayersInfo[player.UserId] = new PlayerInfo
                    {
                        RoleType = player.Role.Type,
                        MaxHealth = player.MaxHealth,
                        Health = player.Health,
                        ActiveEffects = player.ActiveEffects.ToList(),
                        Items = player.Items.ToList(),
                        CurrentItem = player.CurrentItem,
                        Position = new Vector3(player.Position.x, player.Position.y, player.Position.z)
                    };
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> HumanLoop()
        {
            while (!Round.IsEnded)
            {
                foreach (var player in Player.List)
                {
                    if (player.IsHuman)
                    {
                        if (!JumpScareCooldown.Contains(player))
                        {
                            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 25) &&
                                hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                            {
                                if (Player.TryGet(hit.collider.GetComponentInParent<ReferenceHub>(), out Player t) && player != t && t.IsScp)
                                {
                                    JumpScareCooldown.Add(player);

                                    Timing.CallDelayed(60, () =>
                                    {
                                        JumpScareCooldown.Remove(player);
                                    });

                                    AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {player.Nickname}", condition: (hub) =>
                                    {
                                        return hub == player.ReferenceHub;
                                    }, onIntialCreation: (p) =>
                                    {
                                        Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 12050);
                                    });

                                    audioPlayer.AddClip($"facingScp-{UnityEngine.Random.Range(1, 7)}", volume: 2);

                                    Timing.CallDelayed(3, () =>
                                    {
                                        audioPlayer.AddClip("chase", volume: 2);
                                    });
                                }
                            }
                        }
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> Scp079Broadcast()
        {
            yield return Timing.WaitUntilTrue(() => { return Round.IsStarted; });

            while (!Round.IsEnded)
            {
                if (UnityEngine.Random.Range(1, 1001) == 1)
                    GlobalPlayer.AddClip($"scp079-{UnityEngine.Random.Range(1, 3)}", volume: 1.5f);

                int citizenCount = Player.List.Where(x => x.Role.Type == RoleTypeId.ClassD || x.Role.Type == RoleTypeId.Scientist).Count();

                if (citizenCount == 1 && !IsWarningAlone)
                {
                    IsWarningAlone = true;

                    GlobalPlayer.AddClip("scp079-4", volume: 1.2f);
                }
                if (citizenCount == 0 && !IsClearCitizen)
                {
                    IsClearCitizen = true;

                    GlobalPlayer.AddClip("scp079-3", volume: 1.2f);
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
        
        public static IEnumerator<float> Detonation()
        {
            while (!Round.IsEnded)
            {
                if (Warhead.IsDetonated)
                {
                    foreach (var player in Player.List)
                    {
                        if (player.CurrentRoom.Type != RoomType.Surface)
                        {
                            if (GodModePlayers.Contains(player))
                                GodModePlayers.Remove(player);

                            player.Kill("제한된 구역입니다.");
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
