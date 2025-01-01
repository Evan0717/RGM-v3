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

using static RGM.Variables.ServerManagers;
using MEC;
using Exiled.API.Features.Toys;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using System.Runtime.Remoting.Messaging;

namespace RGM.EventArgs
{
    public static class PlayerEvents
    {
        public static IEnumerator<float> OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (!PlayersReport.ContainsKey(ev.Player.UserId))
            {
                PlayersReport.Add(ev.Player.UserId, new PlayerReport()
                {
                    Kill = 0,
                    Death = 0,
                    Revive = 0,
                    KillScp = 0,
                    KillHuman = 0,
                    Damage = 0
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
                    ev.Player.Group = null;
                    ev.Player.RankName = null;
                    ev.Player.BadgeHidden = false;

                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        ev.Player.RankName = $"{(BadgeIcons.ContainsKey(uc[11]) ? $"{BadgeIcons[uc[11]]} " : "")}{(uc[11] != "0" ? uc[11] : "")}";

                        Tools.RemovePaint(ev.Player);
                        Tools.ChangePaint(ev.Player, uc[9]);
                    });
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

            ev.Player.AddBroadcast(10, Notions.WelcomeMessage);

            if (Round.IsStarted)
            {
                string Name = ModeList[CurrentMode].Name;
                string Color = ModeList[CurrentMode].Color;
                string Description = ModeList[CurrentMode].Description;
                string FileName = ModeList[CurrentMode].ToString();
                string Detail = ModeList[CurrentMode].Detail;

                string Message = Notions.LateJoinModeDescription
                .Replace("{ModeColor}", Color)
                .Replace("{CurrentMode}", Name)
                .Replace("{CurrentSubMode}", CurrentSubMode != ModeType.None ? $"<size=20>추가된 서브 모드 : <color=#{ModeList[CurrentSubMode].Color}>{CurrentSubMode.GetModeData().Name}</color></size>\n" : "")
                .Replace("{ModeDescription}", Description)
                .Replace("{ModeInfo}", CurrentMode.GetModeData().Info.ToString());

                ev.Player.AddBroadcast(10, Message);

                ev.Player.SendConsoleMessage($"\n{Message.Replace("\n", "\n")}", "white");
                if (Detail == "")
                    ev.Player.SendConsoleMessage($"\n해당 모드에 대한 자세한 설명이 없습니다.", "white");

                else
                    ev.Player.SendConsoleMessage($"\n{Detail}", "white");
            }
            else
            {
                Server.ExecuteCommand($"/speak {ev.Player.Id} enable");

                Tools.TeleportToLobby(ev.Player);

                if (SelectMode == "SecretVote")
                    ev.Player.EnableEffect(EffectType.Invisible);

                ModeType iv(int num)
                {
                    return ModeVote.Keys.ToList()[num - 1];
                }

                while (!Round.IsStarted && ev.Player.IsConnected)
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
@alvar_noah - 서버 소유자
@mercedes83 - 총괄 관리자 (베테랑)
@normal._.person - 정규 관리자 (베테랑)
@bluefox2322 - 관리자 신입
@0735_ - 관리자 신입

<b><size=35><color=#C8FE2E>개발진</color></size></b>
@GoldenPig1205 - 메인 개발자
@cocoa_1.19 - 서브? 개발자

<b><size=35><color=#F79F81>후원자</color></size></b>
<size=20>@dotory001, @milkyway_0119, @1__neeko__1, @yeeeee222, @tampast, @decoding_, @hs_bini, @suabwy_1059</size>

<b><size=35><color=#F8E0F7>도움 주신 분들</color></size><b>
<size=20>@cocoa_1.19, @leejihyuk, @mujishungplay</size>
</size></align>
\n\n\n\n\n\n\n\n
""", 1.2f);
                            }
                            else if (hit.transform.name == "Mode")
                            {
                                List<string> Modes = new List<string>();

                                foreach (var mode in ModeList)
                                {
                                    ModeData modeData = mode.Key.GetModeData();

                                    string Name = modeData.Name;
                                    string Color = modeData.Color;
                                    ModeCategory flag = modeData.Category;

                                    if (flag == ModeCategory.Private)
                                        Modes.Add($"<s><color=#{Color}>{Name}</color></s>");

                                    else if (flag == ModeCategory.OnlySub)
                                        Modes.Add($"<mark=#FFFF000D><color=#{Color}>{Name}</color></s></mark>");

                                    else
                                        Modes.Add($"<color=#{Color}>{Name}</color>");
                                }

                                ev.Player.ShowHint($"\n\n\n\n\n\n<size=40><b>[ ⭐ 랜덤게임모드(RGM) 모드 목록 ⭐ ]</b></size>\n\n<size=25>{string.Join(", ", Modes)}</size>");
                            }
                            else
                            {
                                ModeType SelectedMode = ModeType.None;
                                string Color;
                                string Description;

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

                                    Color = ModeList[SelectedMode].Color;
                                    Description = ModeList[SelectedMode].Description;
                                }
                                else
                                {
                                    string FirstDesc()
                                    {
                                        if (SelectMode == "RandomSelect")
                                            return "<b>[선택 모드 : 무작위]</b> <color=#F8E0E0>랜덤한 모드가 선택됩니다. 과연 어떤 모드가 걸릴까요?</color>";

                                        else if (SelectMode == "SimpleSelect")
                                            return "<b>[선택 모드 : 롤토체스]</b> <color=#F5F6CE>투표한 유저 중에서 모드가 자동으로 결정됩니다.</color>";

                                        else if (SelectMode == "MostVote")
                                            return "<b>[선택 모드 : 다수결]</b> <color=#E0F2F7>원하는 모드의 번호가 할당된 플랫폼을 밟아 투표하세요.</color>";

                                        else if (SelectMode == "SecretVote")
                                            return "<b>[선택 모드 : 비밀 선거]</b> <color=#E1F5A9>누가 어떤 모드를 투표했는지 알 수 없습니다.</color>";

                                        else if (SelectMode == "FightVote")
                                            return "<b>[선택 모드 : 공포 정치]</b> <color=#E1F5A9>누가 어떤 모드를 투표했는지 알 수 없습니다.</color>";

                                        else
                                            return "<b>[버그로 추정됨 : 문의 요망]</b> 어떤 선택 모드도 선택되지 않았습니다. 뭔가 이상합니다.";
                                    }

                                    Color = "ffffff";
                                    Description = $"{FirstDesc()}\n<size=25>콘솔(` 또는 ~)을 열고 .help를 입력하여 사용 가능한 [RGM] 명령어 리스트를 확인할 수 있습니다.</size>";
                                }

                                string IdeaBy()
                                {
                                    if (!ModeList.ContainsKey(SelectedMode) || ModeList[SelectedMode].Suggester == "")
                                        return "";
                                    else
                                        return $" <size=20><color=white>Idea by {ModeList[SelectedMode].Suggester}</color></size>";
                                }

                                string s(int num)
                                {
                                    if (SelectMode == "SecretVote")
                                        return "?";

                                    else
                                        return ModeVote[iv(num)].Count().ToString();
                                }

                                List<string> uc = UsersManager.UsersCache[ev.Player.UserId];

                                ev.Player.ShowHint(Notions.LobbyMessage
                                    .Replace("{FirstMark}", ModeVote[iv(1)].Contains(ev.Player) ? "■" : "□")
                                    .Replace("{SecondMark}", ModeVote[iv(2)].Contains(ev.Player) ? "■" : "□")
                                    .Replace("{ThirdMark}", ModeVote[iv(3)].Contains(ev.Player) ? "■" : "□")
                                    .Replace("{First}", (CurrentMode != ModeType.None ? CurrentMode.GetModeData().Name : $"<color=#{ModeList[iv(1)].Color}>{iv(1).GetModeData().Name}</color>") + (SubModeVote[0] != ModeType.None ? $" + <b><i> <size=20><color=#{ModeList[SubModeVote[0]].Color}>{SubModeVote[0].GetModeData().Name}</color></size></i></b>" : ""))
                                    .Replace("{FirstVote}", ModeVote[iv(1)].Contains(ev.Player) ? $"<color=yellow>{s(1)}</color>" : s(1))
                                    .Replace("{Second}", (CurrentMode != ModeType.None ? CurrentMode.GetModeData().Name : $"<color=#{ModeList[iv(2)].Color}>{iv(2).GetModeData().Name}</color>") + (SubModeVote[1] != ModeType.None ? $" + <b><i><size=20><color=#{ModeList[SubModeVote[1]].Color}>{SubModeVote[1].GetModeData().Name}</color></size></i></b>" : ""))
                                    .Replace("{SecondVote}", ModeVote[iv(2)].Contains(ev.Player) ? $"<color=yellow>{s(2)}</color>" : s(2))
                                    .Replace("{Third}", (CurrentMode != ModeType.None ? CurrentMode.GetModeData().Name : $"<color=#{ModeList[iv(3)].Color}>{iv(3).GetModeData().Name}</color>") + (SubModeVote[2] != ModeType.None ? $" + <b><i> <size=20><color=#{ModeList[SubModeVote[2]].Color}>{SubModeVote[2].GetModeData().Name}</color></size></i></b>" : ""))
                                    .Replace("{ThirdVote}", ModeVote[iv(3)].Contains(ev.Player) ? $"<color=yellow>{s(3)}</color>" : s(3))
                                    .Replace("{ModeName}", $"{(SelectedMode == ModeType.None ? "<i>참고</i>" : SelectedMode.GetModeData().Name)}{IdeaBy()}")
                                    .Replace("{ModeColor}", $"{Color}").Replace("{ModeDescription}", $"{Description}")
                                    .Replace("{Lines}", $"{(Description.Contains("\n") ? "\n" : "\n\n")}")
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

                    yield return Timing.WaitForSeconds(0.5f);
                }
            }
        }

        public static IEnumerator<float> OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
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
            else
            {
                if (PlayersInfo.ContainsKey(ev.Player.UserId))
                {
                    string UserId = ev.Player.UserId;

                    yield return Timing.WaitForSeconds(1f);

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
                                yield break;
                            }
                        }

                        yield return Timing.WaitForSeconds(1f);
                    }
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

        public static IEnumerator<float> OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                ev.Player.Scale = new Vector3(1, 1, 1);
                Server.ExecuteCommand($"/pfx FogControl 1 0 {ev.Player.Id}");
                Server.ExecuteCommand($"/pfx Ghostly 0 0 {ev.Player.Id}");

                if (Round.IsLobby || ev.Reason == SpawnReason.RoundStart)
                {

                }
                else
                    PlayersReport[ev.Player.UserId].Revive += 1;
            }

            if (ev.Reason == SpawnReason.RoundStart && ev.SpawnFlags == RoleSpawnFlags.All)
            {
                if (ev.Player.IsScp)
                {
                    if (ev.Player.Role.Type == RoleTypeId.Scp079)
                    {
                        ev.Player.MaxHealth = 12050;
                        ev.Player.Health = ev.Player.MaxHealth;
                    }

                    /*
                    if (UnityEngine.Random.Range(1, 21) == 1 && !IsScp3114Enabled)
                    {
                        ev.Player.Role.Set(RoleTypeId.Scp3114);

                        IsScp3114Enabled = true;
                    }
                    */

                    if (CurrentMode.GetModeData().Info == ModeInfo.Plus)
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

                        }
                    }
                    if (StartupRandom == 2) // 시작 NTF
                    {
                        if (ev.Player.Role.Type == RoleTypeId.FacilityGuard)
                        {
                            ev.Player.Role.Set(RoleTypeId.NtfPrivate);

                        }
                    }

                    int rand = UnityEngine.Random.Range(1, 101); // 시작 좀?비

                    if (rand == 1)
                    {
                        ev.Player.Role.Set(RoleTypeId.Scp0492);
                        ev.Player.MaxHealth = 1000;
                        ev.Player.Health = ev.Player.MaxHealth;

                    }
                    else if (rand == 2)
                    {
                        ev.Player.Scale = new Vector3(-1, -1, -1);
                    }
                }
            }

            if (ev.Player.IsAlive && Round.IsStarted && 
                (ev.Reason == SpawnReason.RoundStart || ev.Reason == SpawnReason.Respawn) && 
                CurrentMode.GetModeData().Info == ModeInfo.Plus)
            {
                GodModePlayers.Add(ev.Player);

                yield return Timing.WaitForSeconds(5);

                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);
            }
        }

        public static void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player.IsScp)
            {
                if (ev.Door.Type.ToString().Contains("Checkpoint"))
                {
                    if (ev.Player.CurrentItem != null)
                        ev.Door.IsOpen = false;

                    if (ev.Door.IsFullyClosed)
                    {
                        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                        {
                            ev.Door.IsOpen = true;
                        });
                    }
                }

                else if (ev.Player.Role.Type != RoleTypeId.Scp079 && !ev.Door.IsOpen && !ev.Door.Type.ToString().Contains("Scp079"))
                {
                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (!ev.Door.IsOpen)
                        {
                            if (!InteractedDoors.ContainsKey(ev.Door))
                                InteractedDoors.Add(ev.Door, 0);

                            InteractedDoors[ev.Door] += 1;

                            if (InteractedDoors[ev.Door] >= 500)
                            {
                                ev.Door.IsOpen = true;

                                InteractedDoors.Remove(ev.Door);
                            }
                            else
                                ev.Player.ShowHint($"앞으로 {500 - InteractedDoors[ev.Door]}번 상호작용하면 문이 강제로 열립니다.");
                        }
                    });
                }
            }
        }

        public static void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (Round.IsLobby)
                return;

            if (GodModePlayers.Contains(ev.Player))
            {
                if (!Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
                    ev.IsAllowed = false;

                else
                {
                    GodModePlayers.Remove(ev.Player);
                    ev.Player.Kill(ev.DamageHandler);
                }
            }
            else if (ev.Attacker != null)
            {
                if (HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub) && ev.Attacker != ev.Player)
                    PlayersReport[ev.Attacker.UserId].Damage += (int)ev.DamageHandler.Damage;
            }
        }

        public static void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (Round.IsLobby)
                return;

            if (GodModePlayers.Contains(ev.Player))
            {
                if (!Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
                    ev.IsAllowed = false;

                else
                {
                    GodModePlayers.Remove(ev.Player);
                    ev.Player.Kill(ev.DamageHandler);
                }
            }
        }

        public static void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (Round.IsLobby)
            {
                Tools.TeleportToLobby(ev.Player);
            }
            else
            {
                string MessageFormat()
                {
                    if (ev.Attacker == null)
                        return $"{(PlayersInfo.ContainsKey(ev.Player.UserId) && ev.DamageHandler.Type == DamageType.Unknown ? "⏳ <color=#FF0000><b>SCP 탈주</b></color>(3분 내로 재접속 가능)" : "💀 <color=#A4A4A4>자살</color>")}ㅣ{Tools.BadgeFormat(ev.Player)}<color=#F2F5A9>{ev.Player.DisplayNickname}</color>(<color={ev.TargetOldRole.GetColor().ToHex()}>{Trans.Role[ev.TargetOldRole]}</color>) - {ev.DamageHandler.Type}";

                    else
                        return $"💔 <color=#FAAC58>{(ev.Player.IsCuffed ? "<b>체포킬</b>(신고 가능 여부는 규칙 확인)" : "사살")}</color>ㅣ{Tools.BadgeFormat(ev.Attacker)}<color=#F2F5A9>{ev.Attacker.DisplayNickname}</color>(<color={ev.Attacker.Role.Color.ToHex()}>{Trans.Role[ev.Attacker.Role.Type]}</color>) -> {Tools.BadgeFormat(ev.Player)}<color=#F2F5A9>{ev.Player.DisplayNickname}</color>(<color={ev.TargetOldRole.GetColor().ToHex()}>{Trans.Role[ev.TargetOldRole]}</color>) - {ev.DamageHandler.Type}";
                }

                foreach (var player in Player.List.Where(x => x.IsDead || x == ev.Attacker))
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

        public static void OnItemAdded(Exiled.Events.EventArgs.Player.ItemAddedEventArgs ev)
        {
            if (ev.Player.IsScp || ev.Player.Role.Type.ToString().Contains("Flamingo"))
            {
                if (!ev.Item.IsAmmo)
                {
                    ev.Player.CurrentItem = ev.Item;

                    if (IsDropScpItemAllowed)
                    {
                        foreach (var item in ev.Player.Items.Where(x => x != ev.Item).ToList())
                            ev.Player.DropItem(item);
                    }
                }
            }
        }

        public static void OnKicking(Exiled.Events.EventArgs.Player.KickingEventArgs ev)
        {
            if (ev.Player.IsNPC)
                return;

            foreach (var player in Player.List)
                player.AddBroadcast(10, $"<size=20>{ev.Target.Nickname}(이)가 서버에서 <color=red>추방</color>되었습니다. (사유: {ev.Reason})</size>");
        }

        public static void OnBanning(Exiled.Events.EventArgs.Player.BanningEventArgs ev)
        {
            if (ev.Player.IsNPC)
                return;

            foreach (var player in Player.List)
                player.AddBroadcast(10, $"<size=20>{ev.Target.Nickname}(이)가 서버에서 <color=red>차단</color>되었습니다. (사유: {ev.Reason})</size>");
        }

        public static void OnChangingGroup(Exiled.Events.EventArgs.Player.ChangingGroupEventArgs ev)
        {
            ulong permission = ev.Player.Group.Permissions;

            Timing.CallDelayed(1, () =>
            {
                ev.Player.Group.Permissions = permission;
            });
        }

        public static void OnChangedEmotion(Exiled.Events.EventArgs.Player.ChangedEmotionEventArgs ev)
        {
            EmotionPresetType type = ev.EmotionPresetType;

            if (type == EmotionPresetType.Neutral)
                return;

            string emotion()
            {
                if (type == EmotionPresetType.Happy)
                    return "행복한 표정을 짓고 있습니다";

                else if (type == EmotionPresetType.AwkwardSmile)
                    return "뒤틀린 미소를 짓고 있습니다";

                else if (type == EmotionPresetType.Scared)
                    return "두려운 표정을 짓고 있습니다";

                else if (type == EmotionPresetType.Angry)
                    return "화가난 표정을 짓고 있습니다";

                else if (type == EmotionPresetType.Chad)
                    return "꼭 채드처럼 보이는군요";

                else
                    return "꼭 오우거같이 보이는군요";
            }

            foreach (var player in Player.List.Where(x => x.IsDead || Vector3.Distance(x.Position, ev.Player.Position) < 11))
                player.AddBroadcast(5, $"<size=20>{Tools.BadgeFormat(ev.Player)}<color={ev.Player.Role.Color.ToHex()}>{ev.Player.DisplayNickname}</color>(은)는 {emotion()}.</size>");
        }
    }
}
