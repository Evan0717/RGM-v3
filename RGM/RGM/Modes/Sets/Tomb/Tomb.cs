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
using Exiled.API.Features.Items;
using RGM.API.Features;
using MultiBroadcast.API;
using PlayerRoles;
using Respawning;
using RGM.API.DataBases;
using Exiled.API.Extensions;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Tomb)]
    class Tomb : Mode
    {
        public override string Name => "무덤";
        public override string Description => "살아남으려면 뭐라도 해야 합니다.";
        public override string Detail =>
"""
널리 펼쳐진 평지에는 <b>수많은 아이템</b>이 널려 있습니다.

<i>배틀그라운드와 흡사하죠.</b>
""";
        public override string Color => "000000";

        public static Tomb Instance;

        List<Player> pl = new List<Player>();

        CoroutineHandle _onModeStarted;

        Vector3 RandomPosition()
        {
            return new Vector3(UnityEngine.Random.Range(-44.64675f, 54.59153f), 336, UnityEngine.Random.Range(-95.17068f, 3.98947f));
        }

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves(); 

            Exiled.Events.Handlers.Player.Died += OnDied;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            Tools.LoadMap($"plane");

            PlayerManager.List.CopyTo(pl);

            List<ItemType> ItemTypes = Tools.EnumToList<ItemType>().Where(x => !Datas.ExceptItems.Contains(x)).ToList();
            List<ItemType> ammoTypes = Tools.EnumToList<ItemType>().Where(x => x.IsAmmo()).ToList();

            for (int i = 1; i <= 1205; i++)
            {
                try
                {
                    Item item = Item.Create(Tools.GetRandomValue(ItemTypes));

                    if (item is Firearm firearm)
                        firearm.MagazineAmmo = firearm.MaxMagazineAmmo;

                    item.CreatePickup(RandomPosition());
                }
                catch
                {
                }
            }

            for (int i = 1; i <= 400; i++)
            {
                try
                {
                    Item item = Item.Create(Tools.GetRandomValue(ammoTypes));

                    item.CreatePickup(RandomPosition());
                }
                catch
                {
                }
            }

            foreach (var player in PlayerManager.List)
            {
                try
                {
                    player.Role.Set(RoleTypeId.Tutorial);
                    player.Position = RandomPosition();
                }
                catch
                {
                }
            }

            yield return 0f;
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count() < 2)
                {
                    Round.IsLocked = false;

                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].DisplayNickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                }
            }
        }
    }
}
