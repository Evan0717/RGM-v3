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
using UnityEngine;

namespace RGM.Modes
{
    class Spleef
    {
        public static Spleef Instance;

        public List<Player> pl = new List<Player>();
        public List<ItemType> StartupItems = null;
        public Door door = RGM.GetRandomValue(Door.List.ToList());

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
                player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                player.Position = new Vector3(41.58984f, 1044.594f, -113.3477f);
            }

            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive).ToList())
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 1f, (LayerMask)1))
                    {
                        if (hit.transform.name == "Platform")
                        {
                            Primitive Platform = hit.transform.gameObject.GetComponent<PrimitiveObject>().Primitive;
                            Platform.Color = new Color(255, 0, 0);

                            Timing.CallDelayed(0.75f, Platform.Destroy);
                        }

                        else if (hit.transform.name == "Lava")
                            player.Kill("용암을 좋아한 나머지 뛰어들어갔습니다.");
                    }
                    else
                        player.Hurt(12.05f, "떨어지지 않으려면 계속 움직이세요.");
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
                    Round.IsLocked = false;
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == Exiled.API.Enums.DamageType.Falldown)
            {
                ev.IsAllowed = false;
            }
        }
    }
}
