using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
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
    public class SettingInfo
    {
        public List<SettingBase> SettingBases { get; set; }
        public HeaderSetting Header { get; set; }
        public Action<Player> Activate { get; set; }
    }
}
