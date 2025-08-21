using RGM.API.Features;
using RGM.API.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using PlayerRoles;
using Exiled.API.Extensions;
using static System.Net.Mime.MediaTypeNames;
using RGM.API.DataBases;
using MultiBroadcast.API;
using UserSettings.ServerSpecific;
using Exiled.API.Features.Core.UserSettings;
using System.IO;

namespace RGM.Modes
{
    public static class ABattleVar
    {
        public static Dictionary<Player, int> CASSIE = new();
    }
}
