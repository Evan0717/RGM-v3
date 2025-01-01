using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using RGM.API.Features;
using RGM.API.DataBases;
using UnityEngine;
using Exiled.API.Features.Items;
using PlayerRoles;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.RandomItem)]
    class RandomItem : Mode
    {
        public override string Name => "랜덤박스";
        public override string Description => "60초마다 랜덤한 아이템을 얻을 수 있습니다!";
        public override string Detail =>
"""
무작위 아이템들이 동일한 확률로 지급됩니다.

이후, 60초마다 무작위 아이템들을 하나 더 받습니다.
""";
        public override string Color => "BFFF00";

        public static RandomItem Instance;

        List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();
        List<ItemType> ignoreItemTypes = new List<ItemType>() 
        { 
            ItemType.SCP330
        };

        public override void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive && x.Role.Type != RoleTypeId.Scp079))
                {
                    Item Item = player.AddItem(Tools.GetRandomValue(ItemTypes.Where(x => !ignoreItemTypes.Contains(x)).ToList()));

                    player.ShowHint($"<color=#F3F781>{Trans.Item[Item.Type]}</color>(을)를 지급받았습니다.", 5);
                }

                yield return Timing.WaitForSeconds(60f);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            player.ClearInventory();

            for (int i = 1; i < 9; i++)
            {
                Item Item = player.AddItem(Tools.GetRandomValue(ItemTypes.Where(x => !ignoreItemTypes.Contains(x)).ToList()));
            }
        }
    }
}
