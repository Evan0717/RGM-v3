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
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.ChupaChups)]
    class ChupaChups : Mode
    {
        public override string Name => "츄파춥스";
        public override string Description => "Jailbird.. 요즘 하나씩은 다 가지고 있죠?";
        public override string Detail =>
"""
스폰하면 즉시 Jailbird를 얻습니다.
""";
        public override string Color => "2ECCFA";

        public static ChupaChups Instance;

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

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public IEnumerator<float> Spawned(Player player)
        {
            yield return Timing.WaitForSeconds(1);

            player.AddItem(ItemType.Jailbird);
        }
    }
}
