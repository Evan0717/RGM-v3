using Exiled.API.Enums;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.API.DataBases
{
    public class Datas
    {
        public static List<string> ModeSets = new List<string>()
        {
            "더블업",
            "저거너트",
            "폭탄 파티",
            "개인전",
            "Gun Game",
            "HIDE",
            "GG 클럽",
            "미니게임",
            "해적 룰렛",
            "05 평의회 구출 작전",
            "스플리프",
            "무덤",
            "데드 라인",
            "폭탄 돌리기",
            "꼬리 잡기",
            "데스런",
            "고문",
            "숨바꼭질",
            "점프맵 라운지",
            "스피드런",
            "여긴 어디?",
            "나는 누구?"
        };

        public static Dictionary<string, string> Colors = new Dictionary<string, string>
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

        public static List<EffectType> FunnyEffects = new List<EffectType>()
        {
            EffectType.SugarRush,
            EffectType.SugarHigh,
            EffectType.Spicy,
            EffectType.OrangeCandy,
            EffectType.Metal,
            EffectType.Marshmallow,
            EffectType.Ghostly,
            EffectType.Prismatic,
            EffectType.OrangeWitness
        };

        public static List<DamageType> BlockDamageTypes = new List<DamageType>()
        {
            DamageType.Warhead,
            DamageType.Crushed
        };
    }
}
