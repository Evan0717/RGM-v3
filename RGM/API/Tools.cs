using System;
using System.Collections.Generic;
using System.Linq;
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

        public static Color GetRandomColor(bool Transparency = false)
        {
            if (!Transparency)
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            else
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
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
