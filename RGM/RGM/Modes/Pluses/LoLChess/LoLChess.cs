using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using DiscordInteraction.Discord;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using MultiBroadcast.API;
using PlayerRoles;
using RemoteAdmin;
using RGM.API.Features;
using RGM.Modes.Commands;
using static RGM.Variables.Variable;
using Random = UnityEngine.Random;

namespace RGM.Modes;

[Mode(ModeCategory.Private, ModeInfo.Plus, ModeType.LoLChess)]
public class LoLChess : Mode
{
    public override string Name => "전략적 팀 전투";
    public override string Description => "전략적인 빌드를 구성하여 팀원을 승리로 이끄십시오.";
    public override string Detail =>
"""

""";
    public override string Color => "5fdd74";

    public static ABattle Instance;

    CoroutineHandle _onModeStarted;

    public override void OnEnabled()
    {
        _onModeStarted = Timing.RunCoroutine(OnModeStarted());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_onModeStarted);
    }

    IEnumerator<float> OnModeStarted()
    {
        yield break;
    }
}
