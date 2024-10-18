using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;

namespace RGM.Modes.ABattleVariables
{
    public static class Cooldowns
    {
        public static List<Player> MeleeCooldown = new List<Player>();
        public static List<Player> RoaringSoundCooldown = new List<Player>();
        public static List<Player> TeleportCooldown = new List<Player>();
        public static List<Player> PickPocketCooldown = new List<Player>();
    }
}
