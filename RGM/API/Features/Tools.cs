using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.Commands.Reload;
using PlayerRoles;
using RGM.API.Components;
using UnityEngine;

using static RGM.Variables.Protocol;
using static RGM.Variables.ServerManagers;

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
                if (!item.ToString().Contains("None"))
                    itemList.Add(item);
            }

            return itemList;
        }

        public static List<Transform> GetObjectList(string Name)
        {
            return GameObject.FindObjectsOfType<Transform>().Where(t => t.name == Name).ToList();
        }

        public static List<string> GetModeDesc(string ModeName, string SubModeName = null)
        {
            string ModeColor = ModeList[ModeName][0];
            string ModeDescription = ModeList[ModeName][1];
            string ModeFileName = ModeList[ModeName][2];
            string ModeDescriptionDetail = ModeList[ModeName][5];

            string Message = Notions.StartModeDescription
                .Replace("{ModeColor}", ModeColor)
                .Replace("{CurrentMode}", ModeName)
                .Replace("{CurrentSubMode}", SubModeName != null ? $"<size=20>추가된 서브 모드 : <color=#{ModeList[SubModeName][0]}>{SubModeName}</color></size>\n" : "")
                .Replace("{ModeDescription}", ModeDescription);

            return new List<string>() 
            { 
                Message,
                "성공적으로 모드 설명을 불러왔습니다.",
                "해당 모드에 대한 자세한 설명이 없습니다.", 
                ModeDescriptionDetail 
            };
        }

        public static List<string> GetGameSetsList()
        {
            List<string> Mods = new List<string>()
            {
                "더블업",
                "저거너트",
                "폭탄 파티",
                "개인전",
                "Gun Game",
                "HIDE",
                "GG 클럽",
                "미니 게임",
                "해적 룰렛",
                "스플리프",
                "무덤",
                "데드 라인",
                "폭탄 돌리기",
                "꼬리 잡기",
                "데스런",
                "고문",
                "숨바꼭질",
                "점프맵 라운지",
                "스피드런"
            };

            return Mods;
        }

        public static Color GetRandomColor(bool Transparency = false)
        {
            if (!Transparency)
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            else
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }

        public static Dictionary<string, string> GetColorsDictionary()
        {
            var colors = new Dictionary<string, string>
            {
                // {"gold", "#EFC01A"},
                // {"teal", "#008080"},
                // {"blue", "#005EBC"},
                // {"purple", "#8137CE"},
                // {"light_red", "#FD8272"},
                {"pink", "#FF96DE"},
                {"red", "#C50000"},
                {"default", "#FFFFFF"},
                {"brown", "#944710"},
                {"silver", "#A0A0A0"},
                {"light_green", "#32CD32"},
                {"crimson", "#DC143C"},
                {"cyan", "#00B7EB"},
                {"aqua", "#00FFFF"},
                {"deep_pink", "#FF1493"},
                {"tomato", "#FF6448"},
                {"yellow", "#FAFF86"},
                {"magenta", "#FF0090"},
                {"blue_green", "#4DFFB8"},
                // {"silver_blue", "#666699"},
                {"orange", "#FF9966"},
                // {"police_blue", "#002DB3"},
                {"lime", "#BFFF00"},
                {"green", "#228B22"},
                {"emerald", "#50C878"},
                {"carmine", "#960018"},
                {"nickel", "#727472"},
                {"mint", "#98FB98"},
                {"army_green", "#4B5320"},
                {"pumpkin", "#EE7600"}
            };

            return colors;
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


<size=30><b>{player.Nickname}</b>님의 정보</size>

SteamID: {player.UserId}
Exp: {uc[0]}
RP: {uc[1]}
<i>Cash</i>: ₩{int.Parse(uc[2]).ToString("N0")}

보유한 킬 이펙트: {GetJoinedInfo(3)}
장착한 킬 이펙트: {(uc[4] == "0" ? "-" : uc[4])}
<size=15>{(uc[4] == "0" ? "'.킬이펙트 <킬이펙트 이름>' 명령어를 사용하여 킬 이펙트를 장착할 수 있습니다." : KillEffects[uc[4]])}</size>

보유한 커스텀: {GetJoinedInfo(7)}
커스텀 닉네임: {(uc[5] == "0" ? "-" : uc[5])}
<size=15>{(uc[5] == "0" ? "'.닉네임 <텍스트>' 명령어를 사용하여 커스텀 닉네임을 설정할 수 있습니다." : "이 멋진 닉네임이 모두에게 보입니다!")}</size>
커스텀 인포: {(uc[6] == "0" ? "-" : uc[6])}
<size=15>{(uc[6] == "0" ? "'.인포 <텍스트>' 명령어를 사용하여 커스텀 인포를 설정할 수 있습니다." : "이 아름다운 언포가 모두에게 보입니다!")}</size>

보유한 페인트: {GetJoinedInfo(8)}
장착한 페인트: {(uc[9] == "0" ? "-" : uc[9])}
<size=15>{(uc[9] == "0" ? "'.페인트 <페인트 이름>' 명령어를 사용하여 페인트를 장착할 수 있습니다." : Paints[uc[9]])}</size>
""";
        }

        public static void ChangePaint(Player player, string Color)
        {
            if (player.Group != null && UsersManager.UsersCache[player.UserId][9] != "0")
            {
                Dictionary<string, string[]> ColorDictionary = new Dictionary<string, string[]>()
                {
                    {"블랙골드", new string[] { "brown", "yellow" } },
                    {"핫핑크", new string[] { "magenta", "pink" } },
                    {"레인보우", Tools.GetColorsDictionary().Keys.ToArray() }
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
        }

        public static bool TryGetNearestPlayer(Player player, out Player nearestPlayer, out float radius)
        {
            nearestPlayer = null;
            radius = 99999;

            foreach (var near in Player.List.Where(x => x.IsAlive && x != player))
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

        public static bool TryGetLookPlayer(Player player, float Distance, out Player target)
        {
            target = null;

            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, Distance, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                if (Player.TryGet(hit.collider.GetComponentInParent<ReferenceHub>(), out Player t) && player != t)
                {
                    target = t;

                    return true;
                }
            }

            return false;
        }

        public static bool TryInstallMode(string ModeName)
        {
            var modeType = Type.GetType($"RGM.Modes.{ModeName}");

            if (modeType == null)
            {
                if (ModeManager.Modes.ContainsKey(ModeName))
                    modeType = Type.GetType($"RGM.Modes.{ModeManager.Modes[ModeName][2]}");
            }

            if (modeType != null)
            {
                var modeInstance = Activator.CreateInstance(modeType);
                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                onEnabledMethod?.Invoke(modeInstance, null);

                EnabledModeList.Add(ModeName);

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
    }
}
