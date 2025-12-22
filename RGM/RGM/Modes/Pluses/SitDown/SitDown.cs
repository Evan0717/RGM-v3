using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using RGM.API.DataBases;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.OnlySub, ModeInfo.Plus, ModeType.SitDown)]
    public class SitDown : Mode
    {
        public override string Name => "뜀박질";
        public override string Description => "일어났다가 앉았다가";
        public override string Detail =>
"""
인간은 ALT키를 통해 일어났다가 앉을 수 있습니다.
""";
        public override string Color => "d7dfbd";

        public static SitDown Instance;

        static Dictionary<Player, PlayerStatus> playerStatuses = new Dictionary<Player, PlayerStatus>();

        CoroutineHandle _onModeStarted;

        class PlayerStatus
        {
            public bool IsSitDown { get; set; } = false;
            public bool IsChangingSitDownState { get; set; } = false;
        }

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                playerStatuses.Add(player, new PlayerStatus());
            }

            yield break;
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (!playerStatuses.ContainsKey(ev.Player))
            {
                playerStatuses.Add(ev.Player, new PlayerStatus());
            }
        }

        public static IEnumerator<float> OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (!playerStatuses[ev.Player].IsChangingSitDownState && !ev.Player.IsJumping && !ev.Player.IsNoclipPermitted && ev.Player.IsHuman)
            {
                playerStatuses[ev.Player].IsChangingSitDownState = true;

                AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {ev.Player.Nickname}", onIntialCreation: (p) =>
                {
                    Speaker speaker = p.AddSpeaker("Main", maxDistance: 3);

                    p.transform.parent = ev.Player.GameObject.transform;

                    speaker.transform.parent = ev.Player.GameObject.transform;

                    speaker.transform.localPosition = Vector3.zero;
                });

                if (playerStatuses[ev.Player].IsSitDown)
                {
                    audioPlayer.TryPlay($"standing");

                    while (ev.Player.Scale.y >= 0.65f)
                    {
                        ev.Player.Scale = new Vector3(1, ev.Player.Scale.y - 0.01f, 1);

                        yield return Timing.WaitForOneFrame;
                    }

                    ev.Player.EnableEffect(EffectType.Slowness, 50);
                    ev.Player.EnableEffect(EffectType.SilentWalk, 10);

                    playerStatuses[ev.Player].IsSitDown = false;
                }
                else
                {
                    if (!Physics.Raycast(ev.Player.Position, Vector3.up, out RaycastHit hit, 1, (LayerMask)1))
                    {
                        audioPlayer.TryPlay($"sitting");

                        while (ev.Player.Scale.y <= 1)
                        {
                            ev.Player.Scale = new Vector3(1, ev.Player.Scale.y + 0.01f, 1);

                            yield return Timing.WaitForOneFrame;
                        }

                        ev.Player.DisableEffect(EffectType.Slowness);
                        ev.Player.DisableEffect(EffectType.SilentWalk);

                        playerStatuses[ev.Player].IsSitDown = true;
                    }
                }

                playerStatuses[ev.Player].IsChangingSitDownState = false;
            }
        }
    }
}
