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

                TapeGift._canSpawn = true;

                foreach (var player in Player.List)
                {
                    player.AddBroadcast(20, $"<size=20><b><color=#7289da>Discord</color>에 가입하여 <color=#C8FE2E>실시간 업데이트 현황</color>을 확인하고, 서버에 대한 <color=#F781D8>아이디어</color>를 나누고, <color=#FF4000>상점</color>을 이용하세요!</b></size>");
                    player.AddBroadcast(20, $"<b><size=25>플라밍고를 소환하는 테이프가 재사용 가능해집니다.</size></b>");
                }
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
                        if (FpcExtensionMethods.IsGrounded(player.ReferenceHub))
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

        public static IEnumerator<float> ChattingCooldown()
        {
            while (true)
            {
                ChatCooldown.Clear();

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
    }
}
