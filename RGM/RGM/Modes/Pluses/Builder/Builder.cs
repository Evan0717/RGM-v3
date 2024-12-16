using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using RGM.API.Features;
using MultiBroadcast.API;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Item;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.BomberMan)]
    public class Builder : Mode
    {
        public override string Name => "건축가";
        public override string Description => "건축하세요. 재료는 오직 건축가의 체력 뿐입니다.";
        public override string Detail =>
"""
건축!
""";
        public override string Color => "2EFEC8";
        public override string Suggester => "몬키키";

        public static Builder Instance;

        public override void OnEnabled()
        {

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield break;
        }

        public void OnSwinging(SwingingEventArgs ev)
        {

        }

        public void OnChargingJailbird(ChargingJailbirdEventArgs ev)
        {

        }
    }
}
