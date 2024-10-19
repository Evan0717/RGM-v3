using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;

namespace RGM.Modes.ABattleVariables
{
    public static class AbiltiyManagers
    {
        public static Dictionary<string, string> RatingColor = new Dictionary<string, string>()
        {
            {"일반", "#A4A4A4"},
            {"희귀", "#2ECCFA"},
            {"영웅", "#FF00FF"},
            {"전설", "#ffd700"},
            {"신화", "#DF0101"},
            {"전용", "#F7819F"},
            {"시너지", "#DEEFED"}
        };
    }
}
