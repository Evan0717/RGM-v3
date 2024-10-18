using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using InventorySystem.Items.Usables.Scp330;

namespace RGM.Modes
{
    public class TrickorTreat
    {
        public static TrickorTreat Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Dying += OnDying;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            var Scp330 = player.AddItem(ItemType.SCP330);

            for (int i=1; i<4; i++)
            {
                var Candy = Tools.GetRandomValue(Tools.EnumToList<CandyKindID>());
                player.TryAddCandy(Candy);
            }

            if (player.IsScp)
                player.CurrentItem = Scp330;
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            List<CandyKindID> CandyList = Tools.EnumToList<CandyKindID>();
            {
                var toGive = Tools.GetRandomValue(CandyList);
                ev.Attacker.TryAddCandy(toGive);

                if (ev.Player.IsScp)
                    Server.ExecuteCommand($"/forceeq {ev.Player.Id} 42");
            }
        }
    }
};
