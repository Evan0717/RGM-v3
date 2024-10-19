using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;

namespace RGM.Modes.ABattleVariables
{
    public static class MainManagers
    {
        public static bool IsFeverModeEnabled = false;
        public static Dictionary<Player, List<Vector3>> PlayerWorkstation = new Dictionary<Player, List<Vector3>>();
        public static Dictionary<Player, List<string>> PlayerAbilities = new Dictionary<Player, List<string>>();
        public static Dictionary<Player, string> PlayerVotes = new Dictionary<Player, string>();
    }
}
