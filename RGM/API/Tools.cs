using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using RGM.Features;
using UnityEngine;

namespace RGM.API
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

        public static List<string> GetMiniGamesList()
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
                "꼬리 잡기"
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
                {"gold", "#EFC01A"},
                {"blue", "#005EBC"},
                {"purple", "#8137CE"},
                {"light_red", "#FD8272"},
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
                {"silver_blue", "#666699"},
                {"orange", "#FF9966"},
                {"police_blue", "#002DB3"},
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

            string KillEffects()
            {
                if (uc[3] == "0")
                    return "-";

                else
                    return string.Join(", ", uc[3].Split('/'));
            }

            return
$"""


<size=40><b>{player.Nickname}</b>님의 정보</size>

SteamID: {player.UserId}
Exp: {uc[0]}
RP: {uc[1]}
<i>Cash</i>: ₩{int.Parse(uc[2]).ToString("N0")}
보유한 킬 이펙트: {KillEffects()}
장착한 킬 이펙트: {(uc[4] == "0" ? "-" : uc[4])}
<size=15>{(uc[4] == "0" ? "'.킬이펙트 <킬이펙트 이름>' 명령어를 사용하여 킬 이펙트를 장착할 수 있습니다." : RGM.Instance.KillEffects[uc[4]])}</size>
커스텀 닉네임: {(uc[5] == "0" ? "-" : uc[5])}
<size=15>{(uc[5] == "0" ? "'.닉네임 <텍스트>' 명령어를 사용하여 커스텀 닉네임을 설정할 수 있습니다." : "")}</size>
커스텀 인포: {(uc[6] == "0" ? "-" : uc[6])}
<size=15>{(uc[6] == "0" ? "'.인포 <텍스트>' 명령어를 사용하여 커스텀 인포를 설정할 수 있습니다." : "")}</size>
""";
        }
    }
}
