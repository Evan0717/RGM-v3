using CustomPlayerEffects;
using Exiled.API.Features.Items;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RGM.API.Interfaces
{
    public class PlayerReport
    {
        public int Kill { get; set; }
        public int Death { get; set; }
        public int Revive { get; set; }
        public int KillScp { get; set; }
        public int KillHuman { get; set; }
    }
}
