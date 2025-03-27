using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Exiled.API.Features;
using Exiled.Events.Commands.Reload;
using MEC;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API.Components;
using RGM.API.DataBases;
using UnityEngine;
using DiscordInteraction.Discord;

using static RGM.Variables.ServerManagers;
using RGM.API.Interfaces;
using AdminToys;
using MapEditorReborn.API.Features.Objects;
using Exiled.API.Enums;
using RGM.UserSettings;

namespace RGM.API.Features
{
    public class Tools
    {
        public static T GetRandomValue<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int index = random.Next(0, list.Count);
            return list[index];
        }

        public static List<T> EnumToList<T>()
        {
            Array items = Enum.GetValues(typeof(T));
            List<T> itemList = new List<T>();

            foreach (T item in items)
            {
                List<string> list = new List<string>() 
                {
                    "None",
                    "Unknown",
                    "Destroyed"
                };

                if (!list.Contains(item.ToString()))
                    itemList.Add(item);
            }

            return itemList;
        }

        public static void TeleportToLobby(Player player)
        {
            List<RoleTypeId> Scps = new List<RoleTypeId>()
            {
                RoleTypeId.Scp173,
                RoleTypeId.Scp049,
                RoleTypeId.Scp0492,
                RoleTypeId.Scp106,
                RoleTypeId.Scp939,
                // RoleTypeId.Scp3114,
                RoleTypeId.Flamingo,
                RoleTypeId.AlphaFlamingo,
                RoleTypeId.ZombieFlamingo,
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

            player.Role.Set(Tools.GetRandomValue(Humans));
            player.ClearInventory();
            player.Position = GameObject.Find("LobbyStartPoint").transform.position;

            if (SelectMode == "FightVote")
            {
                switch (UnityEngine.Random.Range(1, 16)) 
                {
                    case 1:
                        player.AddItem(ItemType.GunRevolver);
                        player.AddAmmo(AmmoType.Ammo44Cal, 1205);
                        break;

                    case 2:
                        player.AddItem(ItemType.GrenadeHE);
                        break;
                }
            }
        }

        public static List<Transform> GetObjectList(string Name)
        {
            return GameObject.FindObjectsOfType<Transform>().Where(t => t.name == Name).ToList();
        }

        public static List<string> GetModeDesc(ModeType ModeType, ModeType SubModeType)
        {
            string Color = ModeList[ModeType].Color;
            string Name = ModeList[ModeType].Name;
            string Description = ModeList[ModeType].Description;
            string Detail = ModeList[ModeType].Detail;

            string Message = Notions.StartModeDescription
                .Replace("{ModeColor}", Color)
                .Replace("{CurrentMode}", Name)
                .Replace("{CurrentSubMode}", SubModeType != ModeType.None ? $"<size=20>추가된 서브 모드 : <color=#{ModeList[SubModeType].Color}>{ModeList[SubModeType].Name}</color></size>\n" : "")
                .Replace("{ModeDescription}", Description)
                .Replace("{ModeInfo}", ModeType.GetModeData().Info.ToString());

            return new List<string>() 
            { 
                Message,
                "성공적으로 모드 설명을 불러왔습니다.",
                "해당 모드에 대한 자세한 설명이 없습니다.", 
                Detail 
            };
        }

        public static Color GetRandomColor(bool Transparency = false)
        {
            if (!Transparency)
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);

            else
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }

        public static string GetPlayerInfo(Player player)
        {
            List<string> uc = UsersManager.UsersCache[player.UserId];

            string GetJoinedInfo(int num)
            {
                if (uc[num] == "0")
                    return "-";

                else
                    return string.Join(", ", uc[num].Split('/'));
            }

            return
$"""


<size=20><b>{player.Nickname}</b>님의 정보</size>

<size=15>SteamID: {player.UserId}</size>
<size=15>Exp: {uc[0]}</size>
<size=15>RP: {uc[1]}</size>
<size=15><i>Cash</i>: ₩{int.Parse(uc[2]).ToString("N0")}</size>

<size=15>보유한 킬 이펙트: {GetJoinedInfo(3)}</size>
<size=15>장착한 킬 이펙트: {(uc[4] == "0" ? "-" : uc[4])}</size>
<size=10>{(uc[4] == "0" ? "'.킬이펙트 <킬이펙트 이름>' 명령어를 사용하여 킬 이펙트를 장착할 수 있습니다." : KillEffects[uc[4]])}</size>

<size=15>보유한 커스텀: {GetJoinedInfo(7)}</size>
<size=15>커스텀 닉네임: {(uc[5] == "0" ? "-" : uc[5])}</size>
<size=10>{(uc[5] == "0" ? "'.닉네임 <텍스트>' 명령어를 사용하여 커스텀 닉네임을 설정할 수 있습니다." : $"미리 보기: {Tools.CustomFormatter(player, uc[5]).Replace("\n", "\\n")}")}</size>
<size=15>커스텀 인포: {(uc[6] == "0" ? "-" : uc[6])}</size>
<size=10>{(uc[6] == "0" ? "'.인포 <텍스트>' 명령어를 사용하여 커스텀 인포를 설정할 수 있습니다." : $"미리 보기: {Tools.CustomFormatter(player, uc[6]).Replace("\n", "\\n")}")}</size>

<size=15>보유한 페인트: {GetJoinedInfo(8)}</size>
<size=15>장착한 페인트: {(uc[9] == "0" ? "-" : uc[9])}</size>
<size=10>{(uc[9] == "0" ? "'.페인트 <페인트 이름>' 명령어를 사용하여 페인트를 장착할 수 있습니다." : Paints[uc[9]])}</size>

<size=15>보유한 칭호: {GetJoinedInfo(10)}</size>
<size=15>장착한 칭호: {(uc[11] == "0" ? "-" : uc[11])}</size>
<size=10>{(uc[11] == "0" ? "'.칭호 <칭호 이름>' 명령어를 사용하여 칭호를 장착할 수 있습니다." : Badges[uc[11]])}</size>
""";
        }

        public static void ChangePaint(Player player, string Color)
        {
            if (UsersManager.UsersCache[player.UserId][9] != "0")
            {
                Dictionary<string, string[]> ColorDictionary = new Dictionary<string, string[]>()
                {
                    {"블랙골드", new string[] { "brown", "yellow" } },
                    {"핫핑크", new string[] { "magenta", "pink" } },
                    {"레인보우", Datas.Colors.Keys.ToArray() },
                    {"분홍색", new string[] { "pink" } },
                    {"빨간색", new string[] { "red" } },
                    {"흰색", new string[] { "default" } },
                    {"갈색", new string[] { "brown" } },
                    {"은색", new string[] { "silver" } },
                    {"밝은 녹색", new string[] { "light_green" } },
                    {"진홍색", new string[] { "crimson" } },
                    {"청록색", new string[] { "cyan" } },
                    {"옥색", new string[] { "aqua" } },
                    {"진한 분홍색", new string[] { "deep_pink" } },
                    {"토마토색", new string[] { "tomato" } },
                    {"노란색", new string[] { "yellow" } },
                    {"짙은 홍색", new string[] { "magenta" } },
                    {"푸른 녹색", new string[] { "blue_green" } },
                    {"주황색", new string[] { "orange" } },
                    {"라임색", new string[] { "lime" } },
                    {"초록색", new string[] { "green" } },
                    {"에메랄드색", new string[] { "emerald" } },
                    {"카민색", new string[] { "carmine" } },
                    {"니켈색", new string[] { "nickel" } },
                    {"박하색", new string[] { "mint" } },
                    {"군대 녹색", new string[] { "army_green" } },
                    {"호박색", new string[] { "pumpkin" } }
                };

                TagController rtController = player.GameObject.AddComponent<TagController>();
                rtController.Colors = ColorDictionary[Color];
                rtController.Interval = 1;
            }
        }

        public static void RemovePaint(Player player)
        {
            if (player.GameObject.TryGetComponent<TagController>(out TagController rtc))
                UnityEngine.Object.Destroy(rtc);

            player.RankColor = null;
        }

        public static IEnumerator<float> SetWinner(List<Player> playerList, int amount)
        {
            if (IsWinnerSelected)
                yield break;

            IsWinnerSelected = true;

            if (Server.PlayerCount >= 15)
            {
                foreach (var player in playerList)
                {
                    UsersManager.UsersCache[player.UserId][0] = (int.Parse(UsersManager.UsersCache[player.UserId][0]) + amount).ToString();
                    UsersManager.UsersCache[player.UserId][1] = (int.Parse(UsersManager.UsersCache[player.UserId][1]) + amount).ToString();
                }

                UsersManager.SaveUsers();

                while (true)
                {
                    foreach (var player in Player.List)
                        player.AddBroadcast(1, $"<size={(30 - Math.Round(playerList.Count() * 0.5f))}><color=yellow><b>✨</b></color> <b>{string.Join($", ", playerList.Select(x => $"<color={x.Role.Color.ToHex()}>{x.DisplayNickname}</color>"))}</b>(이)가 <b>{amount}</b> EXP, RP를 획득하였습니다.</size>");

                    yield return Timing.WaitForSeconds(1);
                }
            }
            else
            {
                while (true)
                {
                    foreach (var player in Player.List)
                        player.AddBroadcast(1, $"<size=25>서버 인원이 15명 이하이므로 우승 보상은 지급되지 않습니다.</size>");

                    yield return Timing.WaitForSeconds(1);
                }
            }
        }

        public static bool TryGetNearestPlayer(Player player, out Player nearestPlayer, out float radius, List<Player> exceptPlayers = null)
        {
            nearestPlayer = null;
            radius = 99999;

            if (exceptPlayers == null)
                exceptPlayers = new List<Player>();

            foreach (var near in Player.List.Where(x => x.IsAlive && x != player && !exceptPlayers.Contains(x)))
            {
                float Distance = Vector3.Distance(near.Position, player.Position);

                if (Distance < radius)
                {
                    nearestPlayer = near;
                    radius = Distance;
                }
            }

            if (nearestPlayer != null)
                return true;

            else
                return false;
        }

        public static bool TryGetLookPlayer(Player player, float Distance, out Player target, out RaycastHit? raycastHit)
        {
            target = null;
            raycastHit = null;

            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, Distance) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                if (Player.TryGet(hit.collider.GetComponentInParent<ReferenceHub>(), out Player t) && player != t)
                {
                    target = t;
                    raycastHit = hit;

                    return true;
                }
            }

            return false;
        }

        public static bool TryInstallMode(ModeType ModeType)
        {
            var modeType = Type.GetType($"RGM.Modes.{ModeType}");

            if (modeType == null)
            {
                if (ModeList.ContainsKey(ModeType.GetModeData().Type))
                    modeType = Type.GetType($"RGM.Modes.{modeType}");
            }

            if (modeType != null)
            {
                var modeInstance = Activator.CreateInstance(modeType);
                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                onEnabledMethod?.Invoke(modeInstance, null);

                EnabledModeList.Add(ModeType);

                foreach (var player in Player.List)
                {
                    ServerSpecificSettings.Refresh(player);
                }

                return true;
            }
            else
                return false;
        }

        public static string TryGetUserId(string Name)
        {
            if (Name.Contains("@steam"))
                return Name;

            else if (Player.TryGet(Name, out Player player))
                return player.UserId;

            else
                return null;
        }

        public static Player SpawnDJ(string name, RoleTypeId roleTypeId, Vector3 position, string sn = null)
        {
            ReferenceHub dj = GGUtils.Gtool.Spawn(roleTypeId, position);

            if (sn == null)
                sn = $"{UnityEngine.Random.value}";

            Dictionary<ReferenceHub, string> register = new Dictionary<ReferenceHub, string>()
            {
                { dj, sn }
            };

            foreach (var reg in register)
            {
                try
                {
                    GGUtils.Gtool.Register(reg.Key, reg.Value);
                }
                catch
                {
                }
            }

            GGUtils.Gtool.PlayerGet(sn).DisplayNickname = name;

            return Player.Get(dj);
        }


        public static List<Vector3> GetCirclePoints(Vector3 center, float radius, int pointCount)
        {
            List<Vector3> points = new List<Vector3>();
            float angleStep = 2 * Mathf.PI / pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                float angle = i * angleStep;
                float x = center.x + radius * Mathf.Cos(angle);
                float z = center.z + radius * Mathf.Sin(angle);
                points.Add(new Vector3(x, center.y, z));
            }

            return points;
        }

        public static IEnumerator<float> BugVote(Player host, string reason)
        {
            int RequiredCount = Player.List.Count / 2;
            bool IsSuccess = false;

            Webhook.Send($"🗳️ **게임 진행 불가 투표**ㅣ{host.Nickname}에 의해 시작됨 ({reason})");

            for (int i = 1; i<21; i++)
            {
                if (BugVotePlayers.Count() >= RequiredCount)
                {
                    IsSuccess = true;
                    break;
                }

                foreach (var player in Player.List)
                    player.ShowHint($"<size=25>{host.DisplayNickname}(이)가 <b><color=#FFBF00>게임 진행 불가 투표</color></b>를 개설하였습니다.\n라운드를 강제로 종료해야 한다면 <b>.찬성</b> 명령어를 입력하세요.</size>\n이유 : {reason}\n<size=20>투표 종료까지 {21 - i}초 남음 ({BugVotePlayers.Count}/{RequiredCount})</size>", 1.2f);

                yield return Timing.WaitForSeconds(1);
            }

            if (IsSuccess)
            {
                foreach (var player in Player.List)
                    player.AddBroadcast(5, $"게임 진행 불가 투표가 <b><color=#9AFE2E>가결</color></b>되었습니다. 곧 서버가 재시작됩니다.");

                Webhook.Send($"🗳️ **게임 진행 불가 투표**ㅣ✅ 가결됨 (투표자: {string.Join(", ", BugVotePlayers.Select(x => x.Nickname))})");

                yield return Timing.WaitForSeconds(5);

                Server.ExecuteCommand($"sr");
            }
            else
            {
                foreach (var player in Player.List)
                    player.AddBroadcast(5, $"게임 진행 불가 투표가 <b><color=#FE2E2E>부결</color></b>되었습니다.");

                Webhook.Send($"🗳️ **게임 진행 불가 투표**ㅣ❌ 부결됨 (투표자: {string.Join(", ", BugVotePlayers.Select(x => x.Nickname))})");
            }

            IsBugVoteProcessing = false;
            BugVotePlayers.Clear();
        }

        public static IEnumerator<float> Suggest(Player host, string reason)
        {
            int RequiredCount = Player.List.Count / 2;
            bool IsSuccess = false;

            Webhook.Send($"🔐 **의문의 제안**ㅣ{host.Nickname}에 의해 시작됨 ({reason})");

            for (int i = 1; i < 31; i++)
            {
                if (SuggestPlayers.Count >= RequiredCount)
                {
                    IsSuccess = true;
                    break;
                }

                foreach (var player in Player.List)
                    player.ShowHint($"<size=25><b><color=#DA81F5>의문의 제안</color></b>이 개설되었습니다.\n제안을 수락하시려면 <b>.수락</b> 명령어를 입력하세요.</size>\n{reason}\n<size=20>투표 종료까지 {31 - i}초 남음 ({SuggestPlayers.Count}/{RequiredCount})</size>", 1.2f);

                yield return Timing.WaitForSeconds(1);
            }

            if (IsSuccess)
            {
                foreach (var player in Player.List)
                    player.AddBroadcast(5, $"의문의 제안이 <b><color=#9AFE2E>가결</color></b>되었습니다.");

                Webhook.Send($"🔐 **의문의 제안**ㅣ✅ 가결됨 (투표자: {string.Join(", ", SuggestPlayers.Select(x => x.Nickname))})");
            }
            else
            {
                foreach (var player in Player.List)
                    player.AddBroadcast(5, $"의문의 제안이 <b><color=#FE2E2E>부결</color></b>되었습니다.");

                Webhook.Send($"🔐 **의문의 제안**ㅣ❌ 부결됨 (투표자: {string.Join(", ", SuggestPlayers.Select(x => x.Nickname))})");
            }

            IsSuggestProcessing = false;
            SuggestPlayers.Clear();
        }

        public static void CallSnakeHand(Player Convener, List<Player> PlayerList)
        {
            List<Player> SnakeHands = PlayerList;

            List<ItemType> Items = new List<ItemType>
                {
                    ItemType.KeycardFacilityManager,
                    ItemType.GunFSP9,
                    ItemType.GunRevolver,
                    ItemType.Adrenaline,
                    ItemType.AntiSCP207
                };

            List<ItemType> Ammos = new List<ItemType>
                {
                    ItemType.Ammo44cal,
                    ItemType.Ammo9x19
                };

            foreach (var p in SnakeHands)
            {
                p.Role.Set(RoleTypeId.Tutorial);
                p.Position = new Vector3(-0.08203125f, 1000.96f, 6.828125f);

                foreach (ItemType Item in Items)
                    p.AddItem(Item);

                for (int i = 1; i < 3; i++)
                {
                    foreach (var Ammo in Ammos)
                        p.AddItem(Ammo);
                }
            }

            if (Convener != null)
                Convener.ShowHint($"<i>{SnakeHands.Count()}명의 <color=#FE2EF7>동료</color>들이 당신과 함께합니다..</i>", 5f);
        }

        public static string ColorFormat(string cn)
        {
            if (ColorUtility.TryParseHtmlString(cn, out Color color))
                return color.ToHex();

            else
            {
                var cd = Datas.Colors;

                if (cd.ContainsKey(cn))
                    return cd[cn];

                else
                    return "#FFFFFF";
            }
        }

        public static string BadgeFormat(Player player)
        {
            if (player.RankName != null && !player.BadgeHidden)
                return $"[<color={ColorFormat(player.RankColor)}>{player.RankName}</color>] ";

            else
                return "";
        }

        public static string CustomFormatter(Player player, string str)
        {
            Dictionary<string, PlayerReport> pr = PlayersReport;

            return str
                .Replace("\\n", "\n")
                .Replace("{name}", player.Nickname)
                .Replace("{kill}", $"{pr[player.UserId].Kill}")
                .Replace("{death}", $"{pr[player.UserId].Death}")
                .Replace("{revive}", $"{pr[player.UserId].Revive}")
                .Replace("{kill_scp}", $"{pr[player.UserId].KillScp}")
                .Replace("{kill_human}", $"{pr[player.UserId].KillHuman}")
                .Replace("{max_health}", $"{player.MaxHealth}")
                .Replace("{health}", $"{player.Health}")
                .Replace("{items_count}", $"{player.Items.Count}")
                .Replace("{role}", $"{Trans.Role[player.Role]}")
                .Replace("{damage}", $"{pr[player.UserId].Damage}")
                ;
        }

        public static bool TryGetRaycastPoint(Player player, float _distance, out Vector3 _point)
        {
            Vector3 forward = player.CameraTransform.forward;
            RaycastHit raycastHit;

            if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out raycastHit, _distance))
            {
                _point = Vector3.zero;
                return false;
            }
            else
            {
                _point = raycastHit.point;
                return true;
            }
        }

        public static void GetAllChildren(Transform parentTransform)
        {
            foreach (Transform childTransform in parentTransform)
            {
                Debug.Log(childTransform.name);

                PrimitiveObject primitiveObject = childTransform.GetComponent<PrimitiveObject>();

                if (primitiveObject != null)
                {
                    primitiveObject.Base.PrimitiveFlags = PrimitiveFlags.Visible;
                }

                GetAllChildren(childTransform);
            }
        }

        public static void PlayGlobalAudio(string clipName, float volume = 1, bool loop = false, bool destroyOnEnd = true)
        {
            foreach (var player in Player.List)
            {
                player.AddBroadcast(10, $"<size=20>로드된 오디오: {clipName}</size>");
            }

            GlobalPlayer.AddClip(clipName, volume, loop, destroyOnEnd);
        }
    }
}
