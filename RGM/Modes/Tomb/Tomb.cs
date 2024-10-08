using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using RGM.API;
using MultiBroadcast.API;

namespace RGM.Modes
{
    class Tomb
    {
        public static Tomb Instance;

        public List<Player> pl = new List<Player>();

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public Vector3 RandomPosition()
        {
            return new Vector3(UnityEngine.Random.Range(-27.92969f, 44.88281f), 1043f, UnityEngine.Random.Range(-75.78906f, -2.71875f));
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load plane");

            Player Dummy = Player.List.ToList()[0];
            Player.List.CopyTo(pl);

            List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

            for (int i = 1; i < 500; i++)
            {
                Item Item = Item.Create(Tools.GetRandomValue(ItemTypes));

                Item.CreatePickup(RandomPosition());
            }

            foreach (var player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                player.Position = RandomPosition();
            }

            yield return 0f;
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
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
    }
}
