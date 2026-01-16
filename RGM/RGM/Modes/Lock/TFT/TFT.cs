using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;

using NetworkManagerUtils.Dummies;
using PlayerRoles;
using RemoteAdmin;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes;

[Mode(ModeCategory.Public, ModeInfo.Lock, ModeType.TFT)]
public class TFT : Mode
{
    public override string Name => "전략적 팀 전투";
    public override string Description => "전략적인 빌드를 구성하여 팀원을 승리로 이끄십시오.";
    public override string Detail =>
"""
증강은 한 사람당 총 3개를 확보할 수 있으며,

처음 라운드 시작시 30초 후에,

그 다음 300초마다 지급됩니다.
""";
    public override string Color => "ffd700";

    public static ABattle Instance;

    CoroutineHandle _onModeStarted;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;

        _onModeStarted = Timing.RunCoroutine(OnModeStarted());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

        Exiled.Events.Handlers.Player.Verified -= OnVerified;
        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;

        Timing.KillCoroutines(_onModeStarted);
    }

    IEnumerator<float> OnModeStarted()
    {
        DAONTFT.Main.Init();

        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.SelectFirst());
        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.SelectSecond());
        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.SelectThird());
        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.RerollFirst());
        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.RerollSecond());
        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.RerollThird());

        foreach (var player in PlayerManager.List)
            DAONTFT.Core.EventArgs.PlayerEvents.Verified(player);

        // --------------------------------------------------

        MultiBroadcast.API.MultiBroadcast.ClearAllBroadcasts();

        GlobalPlayer.AddClip("게임 시작", 2);

        Round.IsLocked = true;

        Dictionary<Player, RoleTypeId> role = new();

        var encounter = DAONTFT.Core.Variables.Base.Encounters.GetRandomValue();
        DAONTFT.Core.Variables.Base.Encounter = encounter.Value.Item1;

        if (encounter.Value.Item1 != RoleTypeId.None)
        {
            Player dummy = Player.Get(DummyUtils.SpawnDummy(encounter.Key));
            dummy.Role.Set(encounter.Value.Item1);
            dummy.Health = 99999;
            dummy.Scale = new Vector3(5, 5, 5);
            dummy.Position = new Vector3(139.8427f, 335.6814f, 67.04181f);

            Timing.CallDelayed(11, () =>
            {
                NetworkServer.Destroy(dummy.GameObject);
            });
        }

        foreach (var player in Player.List)
        {
            role.Add(player, player.Role.Type);

            player.Role.Set(RoleTypeId.Tutorial);
            player.Position = new Vector3(137.8167f, 304.3213f, 71.88593f);
            player.AddEffect(EffectType.NightVision, 50);

            Timing.CallDelayed(1, () =>
            {
                player.AddBroadcast(10, $"<size=25>{encounter.Value.Item2}</size>");
            });
        }

        Timing.CallDelayed(11, () =>
        {
            foreach (var player in Player.List)
            {
                if (role.ContainsKey(player))
                    player.Role.Set(role[player]);

                else
                    player.Role.Set(RoleTypeId.ClassD);
            }

            try
            {
                if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ChaosRepressor)
                {
                    foreach (var player in Player.List)
                        player.AddItem(Tools.EnumToList<ItemType>().Where(x => x.IsWeapon()).GetRandomValue());
                }

                if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ChaosMarauder)
                {
                    foreach (var player in Player.List)
                        player.AddItem(Random.Range(1, 3) == 1 ? ItemType.GrenadeFlash : ItemType.GrenadeHE);
                }

                if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ChaosConscript)
                {
                    foreach (var player in Player.List)
                        player.AddItem(Tools.EnumToList<ItemType>().Where(x => x.ToString().Contains("SCP")).GetRandomValue());
                }

                if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ChaosRifleman)
                {
                    foreach (var player in Player.List)
                        player.AddItem(Tools.EnumToList<ItemType>().GetRandomValue());
                }
            }
            catch { }

            Round.IsLocked = false;
        });

        // --------------------------------------------------

        Timing.CallDelayed(30, () =>
        {
            DAONTFT.Core.TFT.ABattle.StartUpgrade();
        });

        int getTime()
        {
            if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ClassD)
                return 100;

            if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.Scientist)
                return 60;

            if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.FacilityGuard)
                return 180;

            else
                return 300;
        }

        int waitTime = getTime();

        while (true)
        {
            yield return Timing.WaitForSeconds(waitTime);

            DAONTFT.Core.TFT.ABattle.StartUpgrade();
        }
    }

    void OnVerified(VerifiedEventArgs ev)
    {
        DAONTFT.Core.EventArgs.PlayerEvents.Verified(ev.Player);
    }

    void OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (ev.Player.IsDead || ev.NewRole.IsDead() || ev.Player.GetAbilities().Count() == 0)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                DAONTFT.Core.TFT.ABattle.Reset(ev.Player);
            });
        }
    }

    void OnRoundEnded(RoundEndedEventArgs ev)
    {
        IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

        if (players.Count() == 1)
            Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

        else if (players.Count() > 1)
            Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
    }
}
