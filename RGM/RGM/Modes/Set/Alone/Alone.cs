using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;
using MultiBroadcast;

using PlayerRoles;
using Respawning;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Alone)]
    class Alone : Mode
    {
        public override string Name => "나 홀로 집에";
        public override string Description => "한명을 제외한 나머지는 모두 SCP가 됩니다.";
        public override string Detail =>
$"""
한명은 반드시 <color={RoleTypeId.ClassD.GetColor().ToHex()}>D계급</color>으로 스폰하며, 탈출하는 것이 목표입니다.
나머지는 모두 <color=red>SCP</color>가 됩니다. (<color={RoleTypeId.Scp079.GetColor().ToHex()}>SCP-079</color> 제외)
""";
        public override string Color => "F781BE";

        public static Alone Instance;

        Player alone;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Respawn.PauseWaves();

            Exiled.Events.Handlers.Player.Escaped += OnEscaped;

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Respawn.ResumeWaves();

            Exiled.Events.Handlers.Player.Escaped -= OnEscaped;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            alone = PlayerManager.List.Where(x => !x.IsNPC).GetRandomValue();
            List<RoleTypeId> ignoredRoles = new List<RoleTypeId> 
            { 
                RoleTypeId.Scp079,
                RoleTypeId.Scp3114,
                RoleTypeId.Scp0492
            };
            List<RoleTypeId> scpRoles = Tools.EnumToList<RoleTypeId>().Where(x => x.IsScp() && !ignoredRoles.Contains(x)).ToList();
            List<ItemType> items = new List<ItemType>() 
            {
                ItemType.KeycardO5,
                ItemType.SCP500,
                ItemType.SCP268,
                ItemType.AntiSCP207,
                ItemType.SCP2176,
                ItemType.Jailbird,
                ItemType.MicroHID,
                ItemType.SCP207
            };

            alone.Role.Set(RoleTypeId.ClassD);
            alone.EnableEffect(EffectType.MovementBoost, 255);

            foreach (var item in items)
                alone.AddItem(item);

            foreach (var player in PlayerManager.List.Where(x => x != alone))
                player.Role.Set(scpRoles.GetRandomValue());

            yield break;
        }

        public void OnEscaped(EscapedEventArgs ev)
        {
            if (ev.Player == alone)
            {
                Timing.RunCoroutine(Tools.SetWinner(new List<Player> { alone }, 5));

                foreach (var player in PlayerManager.List.Where(x => x != alone && x.IsAlive))
                    player.Role.Set(RoleTypeId.Tutorial);
            } 
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }
    }
}
