using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Toys;
using HarmonyLib;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using MultiBroadcast;
using site02;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles.FirstPersonControl;
using PlayerRoles;
using RGM.API.Features;
using MultiBroadcast.API;

namespace RGM.Modes
{
    class Spleef
    {
        public static Spleef Instance;

        public List<Player> pl = new List<Player>();
        public List<ItemType> StartupItems = null;
        public Door door = Tools.GetRandomValue(Door.List.ToList());
        public Dictionary<Player, float> OnGround = new Dictionary<Player, float>();

        public void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;
            Server.FriendlyFire = true;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            Server.ExecuteCommand($"/mp load Spleef");

            Player.List.ToList().CopyTo(pl);

            foreach (var player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = new Vector3(41.26563f, 1084.793f, -114.7344f);
            }

            for (int i=1; i<11; i++)
            {
                Player.List.ToList().ForEach(x => x.ShowHint($"<b>{11 - i}초 뒤 게임이 시작됩니다.</b>", 1.2f));

                yield return Timing.WaitForSeconds(1f);
            }

            IEnumerator<float> RemovingPlatform(Primitive Platform)
            {
                Platform.Color = Color.green;

                yield return Timing.WaitForSeconds(0.3f);

                Platform.Color = Color.yellow;

                yield return Timing.WaitForSeconds(0.3f);

                Platform.Color = Color.red;

                yield return Timing.WaitForSeconds(0.3f);

                Platform.Destroy();
            }

            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive).ToList())
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 3f, (LayerMask)1))
                    {
                        OnGround[player] = 3;

                        if (hit.transform.name == "Platform") 
                        {
                            Primitive Platform = hit.transform.gameObject.GetComponent<PrimitiveObject>().Primitive;

                            Timing.RunCoroutine(RemovingPlatform(Platform));
                        }

                        else if (hit.collider.name == "Lava")
                            player.Kill("용암을 좋아한 나머지 뛰어들어갔습니다.");
                    }
                    else
                    {
                        if (player.IsAlive && OnGround.ContainsKey(player))
                        {
                            OnGround[player] -= 0.1f;

                            if (OnGround[player] <= 0)
                                player.Kill("꼼수를 쓰면 안되죠!");
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public IEnumerator<float> IsFallDown()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player.IsAlive && OnGround.ContainsKey(player) && !player.IsNoclipPermitted && player.Role.Type != RoleTypeId.Scp079)
                    {
                        if (FpcExtensionMethods.IsGrounded(player.ReferenceHub))
                            OnGround[player] = 5;
                        else
                        {
                            OnGround[player] -= 0.1f;

                            if (OnGround[player] <= 0)
                                player.Kill("공허에 빨려들어갔습니다. (5초 이상 낙하)");
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count < 2)
                {
                    Round.IsLocked = false;

                    Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].Nickname}"));
                }
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Falldown)
            {
                ev.IsAllowed = false;
            }
        }
    }
}
