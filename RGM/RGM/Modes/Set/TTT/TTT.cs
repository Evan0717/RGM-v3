using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using RGM.API.Features;

using PlayerRoles;
using RGM.API.DataBases;
using Exiled.API.Extensions;
using static RGM.Variables.Variable;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem.Items.Usables.Scp330;

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
• 가끔씩 진통제, 고폭 수류탄, 섬광탄이 추가로 지급될 수 있습니다.
• <b><color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>은 잘못된 유저를 죽이면 심각한 피해를 입습니다!</b>
• <color=#c753d9>소울메이트</color>들은 서로의 위치를 확인할 수 있습니다.
• <color=#000000>O5 평의회</color>는 혼자서 살아남아야 하는 대신, 많은 체력과 아이템들을 가지고 시작합니다.
• <color=#f178fc>광대</color>도 혼자서 살아남아야 하는 대신, <color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>에게 사망하면 1번 부활합니다.

<b>[Map Credit]</b>
@vasileii, @sleeplessbutter
""";
        bool IsStarted = false;
        Player detective;
        Player O5;
        Player jester;
        List<Player> traitors = new List<Player>();
        List<Player> soulMates = new List<Player>();
        List<Player> innocentRewardTargets = new List<Player>();
        List<Player> instantKillCooldown = new List<Player>();
        List<ItemType> main = new List<ItemType> 
        {
            ItemType.GunAK,
            ItemType.GunShotgun,
            ItemType.GunE11SR,
            ItemType.GunLogicer,
            ItemType.GunFRMG0
        };
        List<ItemType> secondary = new List<ItemType> 
        {
            ItemType.GunRevolver,
            ItemType.GunCOM18,
            ItemType.GunCrossvec,
            ItemType.GunFSP9
        };

        private CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Died += OnDied;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
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

        List<Player> GetInnocentRewardTargets()
        {
            return PlayerManager.List.Where(x =>
                innocentRewardTargets.Contains(x) ||
                x.RankName == "무죄인" ||
                x.RankName == "탐정" ||
                x.RankName == "소울메이트"
            ).ToList();
        }

        public IEnumerator<float> OnModeStarted()
        {
            Tools.LoadMap($"{Maps.GetRandomValue()}");

            foreach (var player in PlayerManager.List)
            {
                spawn(player);
            }

            for (int i = 0; i < 30; i++)
            {
                foreach (var player in PlayerManager.List)
                    player.AddHint("TTT 안내", $"""
<align=left><size=25>
{desc}
</size></align>

{30 - i}초 후 게임이 시작됩니다.



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
            innocentRewardTargets.Clear();

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

                traitors.Add(traitor);
            }

            if (playerCount >= 20)
            {
                jester = PlayerManager.List.Where(x => !traitors.Contains(x)).GetRandomValue();
            }

            if (playerCount >= 15)
            {
                for (int i = 0; i < 2; i++)
                {
                    Player soulMate = PlayerManager.List.Where(x => !traitors.Contains(x) && !soulMates.Contains(x) && jester != x).GetRandomValue();

                    soulMates.Add(soulMate);
                }

                O5 = PlayerManager.List.Where(x => !traitors.Contains(x) && !soulMates.Contains(x) && jester != x).GetRandomValue();
            }

            detective = PlayerManager.List.Where(x => !traitors.Contains(x) && !soulMates.Contains(x) && jester != x && O5 != x).GetRandomValue();
            detective.Role.Set(RoleTypeId.FacilityGuard, RoleSpawnFlags.None);
            detective.RankName = "탐정";
            detective.RankColor = "cyan";
            innocentRewardTargets.Add(detective);
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
                else if (soulMates.Contains(player))
                {
                    innocentRewardTargets.Add(player);
                    player.AddHint("TTT 소울메이트", $"당신은 <color=#c753d9>소울메이트</color>입니다. 당신의 짝이 어디있는지 실시간으로 확인할 수 있습니다.", 20);
                }
                else if (player == detective)
                {
                    player.AddHint("TTT 탐정", $"당신은 <color=#2ECCFA>탐정</color>입니다. <color=red>배신자</color>들을 처단하세요.", 20);
                }
                else if (player == O5)
                {
                    player.AddHint("TTT O5", $"당신은 <color=#000000>O5 평의회</color>입니다. 끝까지 생존하거나, 나머지를 전부 죽이세요!", 20);
                    player.MaxHealth = 250;
                    player.Health = player.MaxHealth;
                    player.AddItem(ItemType.Painkillers);
                }
                else if (player == jester)
                {
                    player.AddHint("TTT 광대", $"당신은 <color=#f178fc>광대</color>입니다. 사망하기 전까지 공격할 수 없으며,\n<color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>에게 사망할 경우 1번 부활하고, 모두가 당신의 정체에 대해 알게 됩니다.", 20);
                }
                else
                {
                    innocentRewardTargets.Add(player);
                    player.AddHint("TTT 무죄인", $"당신은 <color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>입니다. <color=#2ECCFA>탐정</color>과 함께 <color=red>배신자</color>들을 처단하세요.", 20);
                }
            }

            IsStarted = true;
        }

        public IEnumerator<float> Timer()
        {
            for (int i = 1; i < 480; i++)
            {
                if (Round.IsEnded)
                    yield break;

                PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(1, $"<size=25>게임 종료까지 {480 - i}초</size>"));

                yield return Timing.WaitForSeconds(1f);
            }

            // 게임 종료 시점: 무죄인이 살아있으면 무죄인 승리
            var innocents = PlayerManager.List.Where(x =>
                !traitors.Contains(x) &&
                x != O5 &&
                x != jester &&
                x.IsAlive
            ).ToList();

            if (innocents.Count > 0)
            {
                foreach (var player in PlayerManager.List.Where(x => x.IsAlive && !innocents.Contains(x)))
                {
                    if (GodModePlayers.Contains(player))
                        GodModePlayers.Remove(player);

                    player.Kill("무죄인 팀이 승리하였습니다!");
                }

                foreach (var player in innocents)
                {
                    player.AddBroadcast(20, $"<color=orange>무죄인</color> 팀의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(GetInnocentRewardTargets(), 1));
                yield break;
            }

            // 무죄인 없으면 광대 또는 O5 평의회 중에서 승리자 결정
            if (jester != null && jester.IsAlive)
            {
                foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x != jester))
                {
                    if (GodModePlayers.Contains(player))
                        GodModePlayers.Remove(player);

                    player.Kill("광대가 승리를 탈취해갔습니다!");
                }
                jester.AddBroadcast(20, $"<color=#f178fc>광대</color>의 승리입니다!");
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x == jester).ToList(), 13));
            }
            else if (O5 != null && O5.IsAlive)
            {
                foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x != O5))
                {
                    if (GodModePlayers.Contains(player))
                        GodModePlayers.Remove(player);

                    player.Kill("아뿔싸! O5 평의회가 살아있었군요!");
                }
                O5.AddBroadcast(20, $"<color=#000000>O5 평의회</color>의 승리입니다!");
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x == O5).ToList(), 8));
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
                        if (traitors.Contains(t))
                            traitor.AddHint("TTT 배신자 확인", $"그는 당신의 동료, 같은 <color=red>배신자</color>입니다.", 1.2f);

                        else if (t == jester)
                            traitor.AddHint("TTT 광대 확인", $"그는 <color=#f178fc>광대</color>입니다.", 1.2f);

                        else if (t == O5)
                            traitor.AddHint("TTT O5 평의회 확인", $"그는 <color=#000000>O5 평의회</color>입니다.", 1.2f);

                        else
                            traitor.AddHint("TTT 무죄인 확인", $"[ALT]ㅣ해당 <color={RoleTypeId.ClassD.GetColor().ToHex()}>무죄인</color>을 일격에 즉사시키십시오.", 1.2f);
                    }
                    else if (Tools.TryGetNearestPlayer(traitor, out Player nearestPlayer, out float radius, Player.List.Where(traitors.Contains).ToList()))
                        traitor.AddHint("TTT 생존자 거리 확인", $"<b>[ <color={nearestPlayer.Role.Color.ToHex()}>{Trans.Role[nearestPlayer.Role.Type]}</color>, 거리: {radius.ToString("F1")}m ]</b>", 1.2f);

                    else
                        traitor.AddHint("TTT 배신자 임무 완수", "당신은 임무를 완수하였습니다.", 1.2f);
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
                }
                else if (player == O5)
                {
                    player.RankName = "O5 평의회";
                    player.RankColor = "brown";
                }
                else if (player == jester)
                {
                    player.RankName = "광대";
                    player.RankColor = "pink";
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
            if (ev.Player == jester && ev.Player.RankName != "광대")
            {
                ev.IsAllowed = false;
            }
        }

        public void OnShot(ShotEventArgs ev)
        {
            ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
        }

        public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            ev.Player.Grab();

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
                if (!(ev.Attacker == O5 || ev.Attacker == jester))
                {
                    if (ev.Attacker != detective && !traitors.Contains(ev.Attacker))
                    {
                        if (ev.Player != detective && !traitors.Contains(ev.Player) && ev.Player != jester && ev.Player != O5)
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
            else if (ev.Player == O5)
            {
                ev.Player.RankName = "O5 평의회";
                ev.Player.RankColor = "brown";
            }
            else if (ev.Player == jester && ev.Player.RankName != "광대")
            {
                ev.Player.RankName = "광대";
                ev.Player.RankColor = "pink";

                if (ev.Attacker != null && !(traitors.Contains(ev.Attacker) || ev.Attacker == O5))
                {
                    Timing.CallDelayed(3, () =>
                    {
                        ev.Player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
                        ev.Player.AddItem(main.GetRandomValue());
                        ev.Player.AddItem(secondary.GetRandomValue());
                        ev.Player.Position = Tools.GetObjectList("Spot Random").GetRandomValue().position;
                        switch (Random.Range(1, 16))
                        {
                            case 1:
                                ev.Player.AddItem(ItemType.Flashlight);
                                break;

                            case 2:
                                ev.Player.AddItem(ItemType.GrenadeHE);
                                break;

                            case 3:
                                ev.Player.AddItem(ItemType.Painkillers);
                                break;
                        }
                    });
                }
            }
            else if (ev.Player != detective)
            {
                ev.Player.RankName = "무죄인";
                ev.Player.RankColor = "orange";
            }

            if (traitors.Where(x => x.IsAlive).Count() == 0 && !PlayerManager.List.Any(x => (x == O5 || x == jester) && x.IsAlive))
            {
                Round.IsLocked = false;

                if (detective != null && detective.IsAlive)
                    detective.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<color=orange>무죄인</color> 팀의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(GetInnocentRewardTargets(), 1));
            }
            else if (PlayerManager.List.Count(x => x.IsAlive) == 1 && PlayerManager.List.FirstOrDefault(x => x.IsAlive) == jester)
            {
                Round.IsLocked = false;

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<color=#f178fc>광대</color>의 승리입니다!");
                }

                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x == jester).ToList(), 13));
            }
            else if (PlayerManager.List.Count(x => x.IsAlive) == 1 && PlayerManager.List.FirstOrDefault(x => x.IsAlive) == O5)
            {
                Round.IsLocked = false;

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<color=#000000>O5 평의회</color>의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x == O5).ToList(), 8));
            }
            else if (PlayerManager.List.Where(x => !traitors.Contains(x)).Where(x => x.IsAlive).Count() == 0)
            {
                Round.IsLocked = false;

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<color=red>배신자</color> 팀의 승리입니다!");
                }
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(traitors.Contains).ToList(), 4));
            }
        }
    }
}
