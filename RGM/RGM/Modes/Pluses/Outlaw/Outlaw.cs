using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using RGM.API.DataBases;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Outlaw)]
    public class Outlaw : Mode
    {
        public override string Name => "무법자";
        public override string Description => "모두가 총기 하나를 가지고 시작합니다.";
        public override string Detail =>
"""
무기 아이템 중에서 랜덤으로 지급받습니다.
""";
        public override string Color => "9F81F7";

        public static Outlaw Instance;

        public List<ItemType> SpecialWeapons = new List<ItemType>() 
        { 
            ItemType.MicroHID,
            ItemType.ParticleDisruptor,
            ItemType.Jailbird,
            ItemType.GrenadeHE
        };

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            foreach (var player in Player.List.Where(x => x.IsAlive && x.Role.Type != RoleTypeId.Scp079))
                Spawned(player);

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                if (player.IsAlive && player.Role.Type != RoleTypeId.Scp079)
                {
                    Item Weapon = player.AddItem(Tools.GetRandomValue(Tools.EnumToList<ItemType>().Where(x => x.ToString().Contains("Gun") || SpecialWeapons.Contains(x)).ToList()));

                    if (Weapon.Type == ItemType.GrenadeHE)
                        player.AddItem(ItemType.GrenadeHE, 2);

                    if (Weapon is Firearm firearm)
                    {
                        if (firearm.AmmoType != AmmoType.None)
                        {
                            for (int i = 0; i < 3; i++)
                                player.AddItem(firearm.AmmoType.GetItemType());
                        }
                    }
                }
            });
        }
    }
}
