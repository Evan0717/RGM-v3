using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using PlayerRoles;
using MultiBroadcast.API;
using RGM.API.Features;

namespace RGM.Modes
{
    public class DeathRun
    {
        public static DeathRun Instance;

        Player Tagger = null;

        public void OnEnabled()
        {
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            Server.ExecuteCommand("/mp load DeathRun");

            Tagger = Tools.GetRandomValue(Player.List.ToList());

            Tagger.Role.Set(RoleTypeId.Scientist);
            Tagger.Position = new Vector3(44.57422f, 999.7067f, 78.52734f);
            Tagger.ClearInventory();
            Tagger.ShowHint("곧 <color=orange>D계급</color>이 데스런에 입장합니다.\n그들을 막기 위해 <color=red>빨간 버튼</color>을 눌러 일회용 함정을 발동시킬 수 있습니다.\n\n<size=40><b>절대로 그들이 목적지에 도착하지 않도록 하세요!</b></size>", 10);

            for (int i=1; i<11; i++)
            {
                foreach (var player in Player.List)
                {
                    player.ClearPlayerBroadcasts();
                    player.AddBroadcast(2, $"<size=30><b><color=red>{11 - i}</color>초 후 게임이 시작됩니다. 준비하세요!</b></size>");
                }
                yield return Timing.WaitForSeconds(1f);
            }

            Timing.RunCoroutine(Timer());

            foreach (var player in Player.List.Where(x => x != Tagger))
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = new Vector3(48.86719f, 999.6483f, 86.34375f);
            }
        }

        public IEnumerator<float> Timer()
        {
            for (int i=1; i<271; i++)
            {
                foreach (var player in Player.List)
                {
                    player.ClearPlayerBroadcasts();
                    player.AddBroadcast(2, $"<size=25><b><color=yellow>과학자</color>가 총기를 입수하기까지 <color=red>{271 - i}</color>초 남았습니다.</b></size>");
                }
                yield return Timing.WaitForSeconds(1f);
            }

            foreach (var player in Player.List.Where(x => x.IsAlive))
            {
                if (player != Tagger)
                    player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);
            }

            Tagger.AddItem(ItemType.GunE11SR);
            for (int i=1; i<11; i++)
                Tagger.AddItem(ItemType.Ammo556x45);
        }
    }
}
