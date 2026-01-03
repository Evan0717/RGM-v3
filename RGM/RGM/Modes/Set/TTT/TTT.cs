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
using static RGM.Variables.Variable;
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using System.Windows.Forms;
using UserSettings.AudioSettings;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.TTT)]
    class TTT : Mode
    {
        public override string Name => "TTT";
        public override string Description => "테러리스트 타운에서 일어난 마피아 게임 (자세한 설명 필독)";
        public override string Detail =>
$"""
{desc}
""";
        public override string Color => "F78181";

        public static TTT Instance;

        string desc =
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
• <color={RoleTypeId.ClassD.GetColor().ToHex()}>전과자</color>는 <color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color> 팀이지만 <color=red>배신자</color>에게는 <color=red>배신자</color>로 보입니다. 주의하세요!
• <color=#c753d9>소울메이트</color>들은 서로의 위치를 확인할 수 있습니다.
• <color=#000000>O5 평의회</color>는 혼자서 살아남아야 하는 대신, 많은 체력과 아이템들을 가지고 시작합니다.

<b>[Map Credit]</b>
@vasileii, @sleeplessbutter
""";
        bool IsStarted = false;
        Player detective;
        Player O5;
        List<Player> traitors = new List<Player>();
        List<Player> mimics = new List<Player>();
        List<Player> soulMates = new List<Player>();
        List<Player> instantKillCooldown = new List<Player>();
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

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Died += OnDied;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.Shot -= OnShot;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Died -= OnDied;

            Timing.KillCoroutines(_onModeStarted);
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
            Tools.LoadMap($"{Maps.GetRandomValue()}");

            foreach (var player in PlayerManager.List)
            {
                spawn(player);
            }

            for (int i = 0; i < 20; i++)
            {
                foreach (var player in PlayerManager.List)
                    player.AddHint("TTT 안내", $"""
<align=left><size=25>
{desc}
</size></align>

{20 - i}초 후 게임이 시작됩니다.



""", 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }

            Timing.RunCoroutine(Timer());
            Timing.RunCoroutine(FindLocate());

            foreach (var player in PlayerManager.List.Where(x => x.IsDead))
            {
                spawn(player);
            }

            GodModePlayers.Clear();

            int traitorCount = 0;
            int playerCount = PlayerManager.List.Count();

            if (playerCount >= 2 && playerCount <= 5)
                traitorCount = 1;
            else if (playerCount >= 6 && playerCount <= 12)
                traitorCount = 2;
            else if (playerCount >= 13 && playerCount <= 19)
                traitorCount = 3;
            else if (playerCount >= 20 && playerCount <= 26)
                traitorCount = 4;
            else if (playerCount >= 27 && playerCount <= 30)
                traitorCount = 5;
            else if (playerCount >= 31 && playerCount <= 35)
                traitorCount = 6;

            for (int i = 0; i < traitorCount; i++)
            {
                Player traitor = PlayerManager.List.Where(x => !traitors.Contains(x)).GetRandomValue();
                traitor.AddItem(ItemType.Radio);
                traitor.AddItem(ItemType.SCP1853);

                traitors.Add(traitor);
            }

            for (int i = 0; i < traitorCount / 3; i++)
            {
                Player mimic = PlayerManager.List.Where(x => !traitors.Contains(x) && !mimics.Contains(x)).GetRandomValue();

                mimics.Add(mimic);
            }

            if (playerCount >= 15)
            {
                for (int i = 0; i < 2; i++)
                {
                    Player soulMate = PlayerManager.List.Where(x => !traitors.Contains(x) && !mimics.Contains(x) && !soulMates.Contains(x)).GetRandomValue();

                    soulMates.Add(soulMate);
                }

                O5 = PlayerManager.List.Where(x => !traitors.Contains(x) && !mimics.Contains(x) && !soulMates.Contains(x)).GetRandomValue();
            }

            detective = PlayerManager.List.Where(x => !traitors.Contains(x) && !mimics.Contains(x) && !soulMates.Contains(x) && O5 != x).GetRandomValue();
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

            foreach (var player in PlayerManager.List)
            {
                if (traitors.Contains(player))
                {
                    player.AddHint("TTT 배신자", $"당신은 <color=red>배신자</color>입니다. <color=red>배신자</color>들을 제외한 나머지를 모두 사살하세요.\n<size=25>{traitors.Count()}명의 <color=red>배신자</color>가 존재합니다.\n<b>[ALT]ㅣ근접한 플레이어를 즉시 처형할 수 있습니다. (쿨다운 10초)</b></size>", 20);
                }
                else if (mimics.Contains(player))
                {
                    player.AddHint("TTT 전과자", $"당신은 <color={RoleTypeId.ClassD.GetColor().ToHex()}>전과자</color>입니다. <color=#2ECCFA>탐정</color>과 함께 <color=red>배신자</color>들을 처단하세요.\n<size=25>당신은 <color=red>배신자</color>들로 하여금 같은 팀으로 착각하게 만드는 폭력적인 비주얼을 가지고 있습니다.</size>", 20);
                }
                else if (soulMates.Contains(player))
                {
                    player.AddHint("TTT 소울메이트", $"당신은 <color=#c753d9>소울메이트</color>입니다. 당신의 짝이 어디있는지 실시간으로 확인할 수 있습니다.", 20);
                }
                else if (player == detective)
                {
                    player.AddHint("TTT 탐정", $"당신은 <color=#2ECCFA>탐정</color>입니다. <color=red>배신자</color>들을 처단하세요.", 20);
                }
                else if (player == O5)
                {
                    player.AddHint("TTT O5", $"당신은 <color=#000000>O5 평의회</color>입니다. 끝까지 생존하거나, 나머지를 전부 죽이세요!", 20);
                    player.MaxHealth = 350;
                    player.Health = player.MaxHealth;
                    player.AddItem(ItemType.SCP330);
                }
                else
                {
                    player.AddHint("TTT 무죄인", $"당신은 <color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>입니다. <color=#2ECCFA>탐정</color>과 함께 <color=red>배신자</color>들을 처단하세요.", 20);
                }
            }

            IsStarted = true;
        }

        public IEnumerator<float> Timer()
        {
            for (int i = 1; i < 600; i++)
            {
                if (Round.IsEnded)
                    yield break;

                PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(1, $"<size=25><color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인 팀 승리</color>까지</color> {600 - i}초</size>"));

                yield return Timing.WaitForSeconds(1f);
            }

            if (O5 != null && O5.IsAlive)
            {
                foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x != O5))
                {
                    if (GodModePlayers.Contains(player))
                        GodModePlayers.Remove(player);

                    player.Kill("아뿔싸! O5 평의회가 살아있었군요!");
                }
            }
            else
            {
                foreach (var player in PlayerManager.List.Where(x => x.IsAlive))
                {
                    if (traitors.Contains(player))
                    {
                        if (GodModePlayers.Contains(player))
                            GodModePlayers.Remove(player);

                        player.Kill("제한시간이 초과하였습니다.");
                    }
                }
            }
        }

        public IEnumerator<float> FindLocate()
        {
            while (!Round.IsEnded)
            {
                foreach (var traitor in traitors.Where(x => x.IsAlive))
                {
                    if (Tools.TryGetLookPlayer(traitor, 100, out Player t, out RaycastHit? hit))
                    {
                        if (traitors.Contains(t) || mimics.Contains(t))
                            traitor.AddHint("TTT 배신자 확인", $"그는 당신의 동료, 같은 <color=red>배신자</color>입니다.", 1.2f);

                        else
                            traitor.AddHint("TTT 무죄인 확인", $"[ALT]ㅣ해당 <color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>을 일격에 즉사시키십시오.", 1.2f);
                    }
                    else if (Tools.TryGetNearestPlayer(traitor, out Player nearestPlayer, out float radius))
                        traitor.AddHint("TTT 생존자 거리 확인", $"<b>[ <color={nearestPlayer.Role.Color.ToHex()}>{( Trans.Role[nearestPlayer.Role.Type])}</color>, 거리: {radius.ToString("F1")}m ]</b>", 1.2f);

                    else
                        traitor.AddHint("TTT 배신자 임무 완수", "당신은 임무를 완수하였습니다.", 1.2f);
                }

                foreach (var mimic in mimics.Where(x => x.IsAlive))
                {
                    mimic.AddHint("TTT 전과자", $"당신은 <color=red>배신자</color>에게 같은 <color=red>배신자</color>로 보입니다.", 1.2f);
                }

                foreach (var soulMate in soulMates.Where(x => x.IsAlive))
                {
                    if (soulMates.Count() == 2)
                    {
                        Player s = soulMates.Where(x => x != soulMate).FirstOrDefault();
                        soulMate.AddHint("TTT 소울메이트", $"당신의 짝은 {s.DisplayNickname}({(int)Vector3.Distance(s.Position, soulMate.Position)}m)입니다.", 1.2f);
                    }
                    else
                        soulMate.AddHint("TTT 소울메이트", $"당신의 짝은 사망했습니다!", 1.2f);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var player in PlayerManager.List)
            {
                if (traitors.Contains(player))
                {
                    player.RankName = "배신자";
                    player.RankColor = "red";
                }
                else if (soulMates.Contains(player))
                {
                    player.RankName = "소울메이트";
                    player.RankColor = "pink";

                    soulMates.Remove(player);
                }
                else if (mimics.Contains(player))
                {
                    player.RankName = "전과자";
                    player.RankColor = "orange";
                }
                else if (player == O5)
                {
                    player.RankName = "O5 평의회";
                    player.RankColor = "brown";
                }
                else if (player != detective)
                {
                    player.RankName = "무죄인";
                    player.RankColor = "orange";
                }
            }
        }

        public void OnShot(ShotEventArgs ev)
        {
            ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
        }

        public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (!instantKillCooldown.Contains(ev.Player))
            {
                if (Tools.TryGetLookPlayer(ev.Player, 3, out Player t, out RaycastHit? hit))
                {
                    if (traitors.Contains(ev.Player) && !traitors.Contains(t))
                    {
                        if (GodModePlayers.Contains(t))
                            GodModePlayers.Remove(t);

                        t.Hit(ev.Player, ev.Player.MaxHealth);
                        ev.Player.ShowHitMarker();

                        instantKillCooldown.Add(ev.Player);

                        Timing.CallDelayed(10, () =>
                        {
                            instantKillCooldown.Remove(ev.Player);
                        });
                    }
                }
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (Round.IsEnded || !IsStarted)
                return;

            if (ev.Attacker != null)
            {
                if (ev.Attacker != O5)
                {
                    if (ev.Attacker != detective && !traitors.Contains(ev.Attacker))
                    {
                        if (ev.Player != detective && !traitors.Contains(ev.Player))
                        {
                            ev.Attacker.Hurt(50, "같은 무죄인을 죽이는 실수를 범해서는 안됐습니다.");
                        }
                    }
                }
            }

            if (traitors.Contains(ev.Player))
            {
                ev.Player.RankName = "배신자";
                ev.Player.RankColor = "red";
            }
            else if (soulMates.Contains(ev.Player))
            {
                ev.Player.RankName = "소울메이트";
                ev.Player.RankColor = "pink";

                soulMates.Remove(ev.Player);
            }
            else if (mimics.Contains(ev.Player))
            {
                ev.Player.RankName = "전과자";
                ev.Player.RankColor = "orange";
            }
            else if (ev.Player == O5)
            {
                ev.Player.RankName = "O5 평의회";
                ev.Player.RankColor = "brown";
            }
            else if (ev.Player != detective)
            {
                ev.Player.RankName = "무죄인";
                ev.Player.RankColor = "orange";
            }

            if (PlayerManager.List.Count(x => x.IsAlive) == 1 && PlayerManager.List.FirstOrDefault(x => x.IsAlive) == O5)
            {
                Round.IsLocked = false;

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<color=#000000>O5 평의회</color>의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x == O5).ToList(), 5));
            }
            else if (traitors.Where(x => x.IsAlive).Count() == 0 && !PlayerManager.List.Any(x => x == O5 && x.IsAlive))
            {
                Round.IsLocked = false;

                if (detective != null && detective.IsAlive)
                    detective.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<color=orange>무죄인</color> 팀의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => !traitors.Contains(x) && O5 != x).ToList(), 1));
            }
            else if (PlayerManager.List.Where(x => !traitors.Contains(x)).Where(x => x.IsAlive).Count() == 0)
            {
                Round.IsLocked = false;

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<color=red>배신자</color> 팀의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => traitors.Contains(x)).ToList(), 3));
            }
        }
    }
}
