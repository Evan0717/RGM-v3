using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Siberia)]
    public class Siberia : Mode
    {
        public override string Name => "시베리아";
        public override string Description => "최대한 다른 자들과 붙어 온기를 나눠 가지세요!";
        public override string Detail =>
"""
SCP-079가 시설 내 냉각 장치와 에어컨을 풀로 틀어버렸습니다.

3m 이상 떨어지지 마세요!
대상이 누구든 절대로 떨어지지 마십시오.
""";
        public override string Color => "FAFAFA";

        public static Siberia Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                Spawned(player);
            }

            yield return Timing.WaitForSeconds(1f);

            while (true)
            {
                List<Player> Players = PlayerManager.List.ToList();
                List<Player> PassPlayers = new List<Player>();

                foreach (var p1 in Players)
                {
                    foreach (var p2 in PlayerManager.List.Where(x => !PassPlayers.Contains(x)))
                    {
                        if (p1 != p2 && Vector3.Distance(p1.Position, p2.Position) <= 3f)
                        {
                            if (!PassPlayers.Contains(p1))
                                PassPlayers.Add(p1);

                            if (!PassPlayers.Contains(p2))
                                PassPlayers.Add(p2);
                        }
                    }
                }

                foreach (var player in PlayerManager.List.Where(x => !PassPlayers.Contains(x) && x.IsAlive))
                    player.EnableEffect(EffectType.Hypothermia, 255, 1.5f);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public IEnumerator<float> Spawned(Player player)
        {
            yield return Timing.WaitForSeconds(1f);

            player.EnableEffect(EffectType.FogControl, 7);

            if (player.Role.Type == RoleTypeId.Scp079)
                player.Health += 100000;
        }
    }
}
