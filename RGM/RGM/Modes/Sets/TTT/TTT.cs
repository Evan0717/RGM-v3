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
using System.Reflection;
using static RGM.Variables.ServerManagers;
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.TTT)]
    class TTT : Mode
    {
        public override string Name => "TTT";
        public override string Description => "테러리스트 타운에서 일어난 마피아 게임 (자세한 설명 필독)";
        public override string Detail =>
$"""
Trouble in Terrorist Town의 약자.

<b><size=30>[승리 조건]</size></b>
<color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인 팀</color>(탐정, 무죄인) - <color=red>배신자</color>들을 처단하세요.
<color=red>배신자 팀</color>(배신자) - <color=red>배신자 팀</color> 구성원을 제외한 나머지를 모두 사살하세요.

<b><size=30>[참고]</size></b>
• 탐정은 <color={RoleTypeId.FacilityGuard.GetColor().ToHex()}>시설 경비</color>의 모습을 하고 있습니다. <color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>들은 가급적이면 그의 명령을 따라야 합니다.
• <color=red>배신자</color>들에게는 무전기와 SCP-1853(이)가 추가로 지급됩니다.
• 가끔씩 진통제, 고폭 수류탄, 섬광탄이 추가로 지급될 수 있습니다.
• <b><i><color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>은 잘못된 유저를 죽이면 심각한 피해를 입습니다!</i></b>

<b>[Map Credit]</b>
@vasileii
""";
        public override string Color => "F78181";

        public static TTT Instance;

        Player detective;
        List<Player> traitors = new List<Player>();
        List<ItemType> main = new List<ItemType> 
        {
            ItemType.GunA7,
            ItemType.GunCrossvec,
            ItemType.GunFSP9,
            ItemType.GunAK,
            ItemType.GunShotgun,
            ItemType.GunE11SR
        };
        List<ItemType> secondary = new List<ItemType> 
        { 
            ItemType.GunCom45,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunRevolver
        };

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Died += OnDied;

            Timing.RunCoroutine(OnModeStarted());
        }

        void spawn(Player player)
        {
            GodModePlayers.Add(player);

            player.Role.Set(RoleTypeId.ClassD);
            player.AddItem(main.GetRandomValue());
            player.AddItem(secondary.GetRandomValue());
            player.Position = Tools.GetObjectList("Spot Random").GetRandomValue().position;
            switch (Random.Range(1, 16)) 
            {
                case 1:
                    player.AddItem(ItemType.Flashlight);
                    break;

                case 2:
                    player.AddItem(ItemType.GrenadeHE);
                    break;

                case 3:
                    player.AddItem(ItemType.Painkillers);
                    break;
            }
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load {Maps.GetRandomValue()}");

            foreach (var player in Player.List)
            {
                spawn(player);
            }

            for (int i = 0; i < 10; i++)
            {
                foreach (var player in Player.List)
                    player.ShowHint($"{10 - i}초 후 게임이 시작됩니다.", 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }

            Timing.RunCoroutine(Timer());

            foreach (var player in Player.List.Where(x => x.IsDead))
            {
                spawn(player);
            }

            GodModePlayers.Clear();

            for (float i = 1; i < Player.List.Count / 10 + 1; i++)
            {
                Player traitor = Player.List.Where(x => !traitors.Contains(x)).GetRandomValue();
                traitor.AddItem(ItemType.Radio);
                traitor.AddItem(ItemType.SCP1853);

                traitors.Add(traitor);
            }

            detective = Player.List.Where(x => !traitors.Contains(x)).GetRandomValue();
            detective.Role.Set(RoleTypeId.FacilityGuard, RoleSpawnFlags.None);
            detective.RankName = "탐정";
            detective.RankColor = "cyan";
            foreach (var item in new List<ItemType>
            {
                ItemType.ArmorCombat,
                ItemType.Painkillers,
                ItemType.Adrenaline,
                ItemType.Medkit
            })
            {
                detective.AddItem(item);
            }

            foreach (var player in Player.List)
            {
                if (traitors.Contains(player))
                {
                    player.ShowHint($"당신은 <color=red>배신자</color>입니다. <color=red>배신자</color>들을 제외한 나머지를 모두 사살하세요.", 10);
                }
                else if (player == detective)
                {
                    player.ShowHint($"당신은 <color=#2ECCFA>탐정</color>입니다. <color=red>배신자</color>들을 처단하세요.", 10);
                }
                else
                {
                    player.ShowHint($"당신은 <color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>입니다. <color=#2ECCFA>탐정</color>과 함께 <color=red>배신자</color>들을 처단하세요.", 10);
                }
            }

            yield break;
        }

        public IEnumerator<float> Timer()
        {
            for (int i = 1; i < 180; i++)
            {
                if (Round.IsEnded)
                    yield break;

                Player.List.ToList().ForEach(x => x.AddBroadcast(1, $"<size=25><color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인 팀 승리</color>까지</color> {180 - i}초</size>"));

                yield return Timing.WaitForSeconds(1f);
            }

            foreach (var player in Player.List.Where(x => x.IsAlive))
            {
                if (traitors.Contains(player))
                {
                    if (GodModePlayers.Contains(player))
                        GodModePlayers.Remove(player);

                    player.Kill("제한시간이 초과하였습니다.");
                }
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var player in Player.List)
            {
                if (traitors.Contains(player))
                {
                    player.RankName = "배신자";
                    player.RankColor = "red";
                }
                else if (player != detective)
                {
                    player.RankName = "무죄인";
                    player.RankColor = "orange";
                }
            }
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
            });
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (Round.IsEnded)
                return;

            if (ev.Attacker != null)
            {
                if (ev.Attacker != detective && !traitors.Contains(ev.Attacker))
                {
                    if (ev.Player != detective && !traitors.Contains(ev.Player))
                    {
                        ev.Attacker.Hurt(50, "같은 무죄인을 죽이는 실수를 범해서는 안됐습니다.");
                    }
                }
            }

            if (traitors.Contains(ev.Player))
            {
                ev.Player.RankName = "배신자";
                ev.Player.RankColor = "red";
            }
            else if (ev.Player != detective)
            {
                ev.Player.RankName = "무죄인";
                ev.Player.RankColor = "orange";
            }

            if (traitors.Where(x => x.IsAlive).Count() == 0)
            {
                Round.IsLocked = false;

                if (detective != null && detective.IsAlive)
                    detective.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);

                foreach (var player in Player.List)
                {
                    player.AddBroadcast(20, $"<color=orange>무죄인</color> 팀의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(Player.List.Where(x => !traitors.Contains(x)).ToList(), 1));
            }
            if (Player.List.Where(x => !traitors.Contains(x)).Where(x => x.IsAlive).Count() == 0)
            {
                Round.IsLocked = false;

                foreach (var player in Player.List)
                {
                    player.AddBroadcast(20, $"<color=red>배신자</color> 팀의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(Player.List.Where(x => traitors.Contains(x)).ToList(), 3));
            }
        }
    }
}
