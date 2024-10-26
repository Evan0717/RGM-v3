using Exiled.API.Enums;
using Exiled.API.Extensions;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API.DataBases;
using RGM.API.Features;
using RGM.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;

using static RGM.Variables.Protocol;
using static RGM.Variables.ServerManagers;
using MEC;

namespace RGM.EventArgs
{
    public static class PlayerEvents
    {
        public static async void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (!PlayersReport.ContainsKey(ev.Player.UserId))
            {
                PlayersReport.Add(ev.Player.UserId, new PlayerReport()
                {
                    Kill = 0,
                    Death = 0,
                    Revive = 0,
                    KillScp = 0,
                    KillHuman = 0
                });
            }

            // --------------------------------------------------------------------

            List<string> DefaultValues = Enumerable.Repeat("0", 15).ToList();

            if (!UsersManager.UsersCache.ContainsKey(ev.Player.UserId))
            {
                UsersManager.AddUser(ev.Player.UserId, DefaultValues);

                UsersManager.SaveUsers();
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[ev.Player.UserId];

                try
                {
                    Tools.RemovePaint(ev.Player);
                    Tools.ChangePaint(ev.Player, uc[9]);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                if (uc.Count < DefaultValues.Count)
                {
                    int diff = DefaultValues.Count - uc.Count;

                    for (int i = 0; i < diff; i++)
                        uc.Add("0");

                    UsersManager.SaveUsers();
                }

                if (uc[5] != "0")
                    ev.Player.DisplayNickname = uc[5];

                if (uc[6] != "0")
                    ev.Player.CustomInfo = uc[6];
            }

            OnGround.Add(ev.Player, 5);

            if (Round.IsStarted)
            {
                string ModeColor = ModeList[CurrentMode][0];
                string ModeDescription = ModeList[CurrentMode][1];
                string ModeFileName = ModeList[CurrentMode][2];
                string ModeDescriptionDetail = ModeList[CurrentMode][5];

                string Message = Notions.LateJoinModeDescription
                .Replace("{ModeColor}", ModeColor)
                .Replace("{CurrentMode}", CurrentMode)
                .Replace("{CurrentSubMode}", CurrentSubMode != null ? $"<size=20>추가된 서브 모드 : <color=#{ModeList[CurrentSubMode][0]}>{CurrentSubMode}</color></size>\n" : "")
                .Replace("{ModeDescription}", ModeDescription);

                ev.Player.AddBroadcast(10, Message);

                ev.Player.SendConsoleMessage($"\n{Message.Replace("\n", "\n")}", "white");
                if (ModeDescriptionDetail == "")
                    ev.Player.SendConsoleMessage($"\n해당 모드에 대한 자세한 설명이 없습니다.", "white");

                else
                    ev.Player.SendConsoleMessage($"\n{ModeDescriptionDetail}", "white");
            }
            else
            {
                Server.ExecuteCommand($"/speak {ev.Player.Id} enable");

                List<RoleTypeId> Scps = new List<RoleTypeId>()
                {
                    RoleTypeId.Scp173,
                    RoleTypeId.Scp049,
                    RoleTypeId.Scp0492,
                    RoleTypeId.Scp106,
                    RoleTypeId.Scp939,
                    RoleTypeId.Scp3114
                };

                List<RoleTypeId> Humans = new List<RoleTypeId>()
                {
                    RoleTypeId.ClassD,
                    RoleTypeId.Scientist,
                    RoleTypeId.FacilityGuard,
                    RoleTypeId.ChaosConscript,
                    RoleTypeId.NtfSpecialist,
                    RoleTypeId.Tutorial
                };

                List<RoleTypeId> SelectedRole()
                {
                    if (UnityEngine.Random.Range(1, 11) == 1)
                        return Scps;

                    else
                        return Humans;
                }

                ev.Player.Role.Set(Tools.GetRandomValue(SelectedRole()));
                ev.Player.ClearInventory();
                ev.Player.Position = GameObject.Find("LobbyStartPoint").transform.position;
                ev.Player.AddBroadcast(10, Notions.WelcomeMessage);

                string iv(int num)
                {
                    return $"{ModeVote.Keys.ToList()[num - 1]}";
                }

                while (!Round.IsStarted)
                {
                    try
                    {
                        if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 5f, (LayerMask)1))
                        {
                            if (hit.transform.name == "Credit")
                            {
                                ev.Player.ShowHint(
    """
<size=50><b>[ ⭐ 랜덤게임모드(RGM) 크레딧 ⭐ ]</b></size>

<align=left><size=30>
<b><size=35><color=#F7FE2E>관리진</color></size></b>
Arloramäki(@alvar_noah) - 서버 소유자
Serendipity(@mercedes83) - 총괄 관리자 (베테랑)
Normalperson(@normal._.person) - 정규 관리자 (베테랑)
bluefox2322(@bluefox2322) - 수습 관리자
pey paid for it(@0735_) - 수습 관리자
leejihyuk(@leejihyuk) - 수습 관리자

<b><size=35><color=#C8FE2E>개발진</color></size></b>
GoldenPig1205(@GoldenPig1205) - 메인 개발자

<b><size=35><color=#F79F81>후원자</color></size></b>
<size=20>[D.I.]Dotory001(@dotory001), 류세(@milkyway_0119), Lu(@1__neeko__1), 뇨호호(@yeeeee222), ㅠㅅㅠ(@tampast)</size>

<b><size=35><color=#F8E0F7>도움 주신 분들</color></size><b>
<size=20>Cocoa(@cocoa_1.19), leejihyuk(@leejihyuk), MujishungPlay(@mujishungplay)</size>
</size></align>
\n\n\n\n\n\n\n\n
""", 1.2f);
                            }
                            else if (hit.transform.name == "Mode")
                            {
                                List<string> Modes = new List<string>();

                                foreach (var mode in ModeList)
                                {
                                    string modeName = mode.Key;
                                    string color = mode.Value[0];
                                    string flag = mode.Value[3];

                                    if (flag == "private")
                                        Modes.Add($"<s><color=#{color}>{modeName}</color></s>");

                                    else if (flag == "onlysub")
                                        Modes.Add($"<mark=#FFFF0080><color=#{color}>{modeName}</color></s></mark>");

                                    else
                                        Modes.Add($"<color=#{color}>{modeName}</color>");
                                }

                                ev.Player.ShowHint($"\n\n\n\n\n\n<size=40><b>[ ⭐ 랜덤게임모드(RGM) 모드 목록 ⭐ ]</b></size>\n\n<size=25>{string.Join(", ", Modes)}</size>");
                            }
                            else
                            {
                                string SelectedMode;
                                string ModeColor;
                                string ModeDescription;

                                for (int i = 0; i < 3; i++)
                                {
                                    if (ModeVote.ContainsKey(ModeVote.Keys.ToList()[i]) && ModeVote[ModeVote.Keys.ToList()[i]].Contains(ev.Player))
                                        ModeVote[ModeVote.Keys.ToList()[i]].Remove(ev.Player);
                                }

                                if (new List<string>() { "First", "Second", "Third" }.Contains(hit.collider.name))
                                {
                                    if (hit.collider.name == "First")
                                    {
                                        SelectedMode = ModeVote.Keys.ToList()[0];
                                        ModeVote[SelectedMode].Add(ev.Player);
                                    }
                                    else if (hit.collider.name == "Second")
                                    {
                                        SelectedMode = ModeVote.Keys.ToList()[1];
                                        ModeVote[SelectedMode].Add(ev.Player);
                                    }
                                    else
                                    {
                                        SelectedMode = ModeVote.Keys.ToList()[2];
                                        ModeVote[SelectedMode].Add(ev.Player);
                                    }

                                    ModeColor = ModeList[SelectedMode][0];
                                    ModeDescription = ModeList[SelectedMode][1];
                                }
                                else
                                {
                                    string FirstDesc()
                                    {
                                        if (SelectMode == "RandomSelect")
                                            return "<b>[선택 모드 : 임의의 손길]</b> <color=#F8E0E0>랜덤한 모드가 선택됩니다. 과연 어떤 모드가 걸릴까요?</color>";

                                        else if (SelectMode == "SimpleSelect")
                                            return "<b>[선택 모드 : 간이 셀렉트]</b> <color=#F5F6CE>투표한 유저 중에서 모드가 자동으로 결정됩니다.</color>";

                                        else if (SelectMode == "MostVote")
                                            return "<b>[선택 모드 : 다수 결정자]</b> <color=#E0F2F7>원하는 모드의 번호가 할당된 플랫폼을 밟아 투표하세요.</color>";

                                        else
                                            return "<b>[버그로 추정됨 : 문의 요망]</b> 어떤 선택 모드도 선택되지 않았습니다. 뭔가 이상합니다.";
                                    }

                                    SelectedMode = "<i>참고</i>";
                                    ModeColor = "ffffff";
                                    ModeDescription = $"{FirstDesc()}\n<size=25>콘솔(` 또는 ~)을 열고 .help를 입력하여 사용 가능한 [RGM] 명령어 리스트를 확인할 수 있습니다.</size>";
                                }

                                string IdeaBy()
                                {
                                    if (!ModeList.ContainsKey(SelectedMode) || ModeList[SelectedMode][4] == "")
                                        return "";
                                    else
                                        return $" <size=20><color=white>Idea by {ModeList[SelectedMode][4]}</color></size>";
                                }

                                List<string> uc = UsersManager.UsersCache[ev.Player.UserId];

                                ev.Player.ShowHint(Notions.LobbyMessage
                                    .Replace("{FirstMark}", ModeVote[iv(1)].Contains(ev.Player) ? "■" : "□")
                                    .Replace("{SecondMark}", ModeVote[iv(2)].Contains(ev.Player) ? "■" : "□")
                                    .Replace("{ThirdMark}", ModeVote[iv(3)].Contains(ev.Player) ? "■" : "□")
                                    .Replace("{First}", CurrentMode != null ? CurrentMode : $"<color=#{ModeList[iv(1)][0]}>{iv(1)}</color>" + (SubModeVote[0] != null ? $" + <b><i> <size=20><color=#{ModeList[SubModeVote[0]][0]}>{SubModeVote[0]}</color></size></i></b>" : ""))
                                    .Replace("{FirstVote}", ModeVote[iv(1)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(1)].Count()}</color>" : ModeVote[iv(1)].Count().ToString())
                                    .Replace("{Second}", CurrentMode != null ? CurrentMode : $"<color=#{ModeList[iv(2)][0]}>{iv(2)}</color>" + (SubModeVote[1] != null ? $" + <b><i><size=20><color=#{ModeList[SubModeVote[1]][0]}>{SubModeVote[1]}</color></size></i></b>" : ""))
                                    .Replace("{SecondVote}", ModeVote[iv(2)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(2)].Count()}</color>" : ModeVote[iv(2)].Count().ToString())
                                    .Replace("{Third}", CurrentMode != null ? CurrentMode : $"<color=#{ModeList[iv(3)][0]}>{iv(3)}</color>" + (SubModeVote[2] != null ? $" + <b><i> <size=20><color=#{ModeList[SubModeVote[2]][0]}>{SubModeVote[2]}</color></size></i></b>" : ""))
                                    .Replace("{ThirdVote}", ModeVote[iv(3)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(3)].Count()}</color>" : ModeVote[iv(3)].Count().ToString())
                                    .Replace("{ModeName}", $"{SelectedMode}{IdeaBy()}")
                                    .Replace("{ModeColor}", $"{ModeColor}").Replace("{ModeDescription}", $"{ModeDescription}")
                                    .Replace("{Lines}", $"{(ModeDescription.Contains("\n") ? "\n" : "\n\n")}")
                                    .Replace("{Exp}", $"{uc[0]}")
                                    .Replace("{RP}", $"{uc[1]}")
                                    .Replace("{Cash}", $"{int.Parse(uc[2]).ToString("N0")}")
                                    .Replace("{Tip}", Tip)
                                    .Replace("{Version}", $"{RGM.Instance.Version}")
                                    .Replace("{Logo}", $"{Logo}")
                                    , 1.2f);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }

                    await Task.Delay(500);
                }
            }
        }

        public static async void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (OnGround.ContainsKey(ev.Player))
                OnGround.Remove(ev.Player);

            if (!Round.IsStarted)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (ModeVote.ContainsKey(ModeVote.Keys.ToList()[i]) && ModeVote[ModeVote.Keys.ToList()[i]].Contains(ev.Player))
                        ModeVote[ModeVote.Keys.ToList()[i]].Remove(ev.Player);
                }
            }

            if (PlayersInfo.ContainsKey(ev.Player.UserId))
            {
                string UserId = ev.Player.UserId;

                await Task.Delay(1000);

                for (int i = 1; i < 181; i++)
                {
                    foreach (var player in Player.List.Where(x => !x.IsNPC))
                    {
                        if (UserId == player.UserId)
                        {
                            player.Role.Set(PlayersInfo[UserId].RoleType);
                            player.MaxHealth = PlayersInfo[UserId].MaxHealth;
                            player.Health = PlayersInfo[UserId].Health;

                            foreach (var effect in PlayersInfo[UserId].ActiveEffects)
                                player.EnableEffect(effect, effect.Intensity, effect.Duration);

                            player.ClearItems();

                            foreach (var item in PlayersInfo[UserId].Items)
                                player.AddItem(item.Type);

                            player.CurrentItem = player.Items.ToList().Find(x => x.Type == PlayersInfo[UserId].CurrentItem.Type);

                            player.Position = new Vector3(PlayersInfo[UserId].Position.x, PlayersInfo[UserId].Position.y, PlayersInfo[UserId].Position.z);

                            if (PlayersInfo.ContainsKey(UserId))
                                PlayersInfo.Remove(UserId);

                            Player.List.Where(x => x.IsDead).ToList().ForEach(x => x.AddBroadcast(10, $"<size=20>❤️ SCP 재접속 -> {player.DisplayNickname}(<color={player.Role.Color.ToHex()}>{Trans.Role[player.Role.Type]}</color>)</size>"));

                            PlayersInfo.Remove(player.UserId);
                            return;
                        }
                    }

                    await Task.Delay(1000);
                }
            }
        }

        public static void OnSpawningRagdoll(Exiled.Events.EventArgs.Player.SpawningRagdollEventArgs ev)
        {
            if (!Round.IsStarted)
                ev.IsAllowed = false;
        }

        public static void OnSpawnedRagdoll(Exiled.Events.EventArgs.Player.SpawnedRagdollEventArgs ev)
        {
            Timing.CallDelayed(5 * 60, () =>
            {
                if (ev.Ragdoll != null)
                    ev.Ragdoll.Destroy();
            });
        }

        public static async void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                ev.Player.Scale = new Vector3(1, 1, 1);
                ev.Player.EnableEffect(EffectType.FogControl);
                ev.Player.DisableEffect(EffectType.Ghostly);

                if (Round.IsLobby || ev.Reason == SpawnReason.RoundStart)
                {

                }
                else
                    PlayersReport[ev.Player.UserId].Revive += 1;
            }

            if (ev.Reason == SpawnReason.RoundStart)
            {
                if (ev.Player.IsScp)
                {
                    if (UnityEngine.Random.Range(1, 21) == 1 && !IsScp3114Enabled)
                    {
                        ev.Player.Role.Set(RoleTypeId.Scp3114);

                        ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>SCP-3114(5%, 정규)</color> 기믹이 적용되었습니다.");
                        IsScp3114Enabled = true;
                    }

                    if (!Tools.GetGameSetsList().Contains(CurrentMode))
                    {
                        PlayersInfo.Add(ev.Player.UserId, new PlayerInfo
                        {
                            RoleType = ev.Player.Role.Type,
                            MaxHealth = ev.Player.MaxHealth,
                            Health = ev.Player.Health,
                            ActiveEffects = ev.Player.ActiveEffects.ToList(),
                            Items = ev.Player.Items.ToList(),
                            CurrentItem = ev.Player.CurrentItem,
                            Position = new Vector3(ev.Player.Position.x, ev.Player.Position.y, ev.Player.Position.z)
                        });
                    }
                }
                else if (ev.Player.IsHuman)
                {
                    if (StartupRandom == 1) // 시작 카오스
                    {
                        if (ev.Player.Role.Type == RoleTypeId.FacilityGuard)
                        {
                            ev.Player.Role.Set(RoleTypeId.ChaosConscript);

                            ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>시작 카오스(5%, 정규)</color> 기믹이 적용되었습니다.");
                        }
                    }
                    if (StartupRandom == 2) // 시작 NTF
                    {
                        if (ev.Player.Role.Type == RoleTypeId.FacilityGuard)
                        {
                            ev.Player.Role.Set(RoleTypeId.NtfPrivate);

                            ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>시작 NTF(5%, 정규)</color> 기믹이 적용되었습니다.");
                        }
                    }

                    int rand = UnityEngine.Random.Range(1, 101); // 시작 좀?비
                    if (rand == 1)
                    {
                        ev.Player.Role.Set(RoleTypeId.Scp0492);
                        ev.Player.MaxHealth = 1000;
                        ev.Player.Health = ev.Player.MaxHealth;

                        ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>시작 좀비(1%, 이스터에그)</color> 기믹이 적용되었습니다.");
                    }
                    else if (rand == 2)
                    {
                        ev.Player.Scale = new Vector3(-1, -1, -1);

                        ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>뒤집기(1%, 이스터에그)</color> 기믹이 적용되었습니다.");
                    }
                }
            }

            if (ev.Player.Role.Type == RoleTypeId.Scp079)
            {
                ev.Player.MaxHealth = 12050;
                ev.Player.Health = ev.Player.MaxHealth;
            }

            if (ev.Player.IsAlive && Round.IsStarted && (ev.Reason == SpawnReason.RoundStart || ev.Reason == SpawnReason.Respawn))
            {
                GodModePlayers.Add(ev.Player);

                await Task.Delay(5000);

                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);
            }
        }

        public static void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player.IsScp && ev.Player.CurrentItem != null && ev.Door.Name.Contains("CHECKPOINT"))
                ev.Door.IsOpen = true;
        }

        public static void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (!Round.IsStarted)
                ev.IsAllowed = false;

            else
            {
                if (GodModePlayers.Contains(ev.Player))
                {
                    if (ev.Attacker != null && ev.Attacker != ev.Player)
                        ev.IsAllowed = false;
                }

                if (ev.Player.LeadingTeam == ev.Attacker.LeadingTeam && ev.DamageHandler.Type == DamageType.Marshmallow)
                    ev.IsAllowed = false;
            }
        }

        public static void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (!Round.IsStarted)
                ev.IsAllowed = false;

            else
            {
                if (GodModePlayers.Contains(ev.Player))
                {
                    if (ev.Attacker != null && ev.Attacker != ev.Player && ev.DamageHandler.Type != DamageType.Warhead)
                        ev.IsAllowed = false;
                }
            }
        }

        public static void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            string ColorFormat(string cn)
            {
                if (ColorUtility.TryParseHtmlString(cn, out Color color))
                    return color.ToHex();

                else
                {
                    var cd = Tools.GetColorsDictionary();

                    if (cd.ContainsKey(cn))
                        return cd[cn];

                    else
                        return "#FFFFFF";
                }
            }

            string BadgeFormat(Player player)
            {
                if (player.Group != null && !player.BadgeHidden)
                    return $"[<color={ColorFormat(player.Group.BadgeColor)}>{player.Group.BadgeText}</color>] ";

                else
                    return "";
            }

            string MessageFormat()
            {
                if (ev.Attacker == null)
                    return $"{(PlayersInfo.ContainsKey(ev.Player.UserId) && ev.DamageHandler.Type == DamageType.Unknown ? "⏳ <color=#FF0000><b>SCP 탈주</b></color>(3분 내로 재접속 가능)" : "💀 <color=#A4A4A4>자살</color>")}ㅣ{BadgeFormat(ev.Player)}<color=#F2F5A9>{ev.Player.DisplayNickname}</color>(<color={ev.TargetOldRole.GetColor().ToHex()}>{Trans.Role[ev.TargetOldRole]}</color>) - {ev.DamageHandler.Type}";

                else
                    return $"💔 <color=#FAAC58>{(ev.Player.IsCuffed ? "<b>체포킬</b>(신고 가능 여부는 규칙 확인)" : "사살")}</color>ㅣ{BadgeFormat(ev.Attacker)}<color=#F2F5A9>{ev.Attacker.DisplayNickname}</color>(<color={ev.Attacker.Role.Color.ToHex()}>{Trans.Role[ev.Attacker.Role.Type]}</color>) -> {BadgeFormat(ev.Player)}<color=#F2F5A9>{ev.Player.DisplayNickname}</color>(<color={ev.TargetOldRole.GetColor().ToHex()}>{Trans.Role[ev.TargetOldRole]}</color>) - {ev.DamageHandler.Type}";
            }

            foreach (var player in Player.List.Where(x => x.IsDead))
                player.AddBroadcast(10, $"<size=20>{MessageFormat()}</size>");

            if (ev.Attacker != null && !ev.Attacker.IsNPC)
            {
                PlayersReport[ev.Attacker.UserId].Kill += 1;

                if (ev.Player.IsScp)
                    PlayersReport[ev.Attacker.UserId].KillScp += 1;

                if (!ev.Player.IsScp)
                    PlayersReport[ev.Attacker.UserId].KillHuman += 1;
            }

            if (!ev.Player.IsNPC)
                PlayersReport[ev.Player.UserId].Death += 1;
        }

        public static void OnDroppedItem(Exiled.Events.EventArgs.Player.DroppedItemEventArgs ev)
        {
            Timing.CallDelayed(5 * 60, () =>
            {
                if (ev.Pickup != null)
                    ev.Pickup.Destroy();
            });
        }

        public static void OnDroppedAmmo(Exiled.Events.EventArgs.Player.DroppedAmmoEventArgs ev)
        {
            Timing.CallDelayed(5 * 60, () =>
            {
                foreach (var ammo in ev.AmmoPickups)
                {
                    if (ammo != null)
                        ammo.Destroy();
                }
            });
        }
    }
}
