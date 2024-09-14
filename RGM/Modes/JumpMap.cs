using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace RGM.Modes
{
    class JumpMap
    {
        site02.site02 site02 = new site02.site02();

        public void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Timing.CallDelayed(1f, () =>
            {
                Timing.RunCoroutine(OnModeStarted());

                site02.OnEnabled();
                site02.OnRoundStarted();

                foreach (var player in Player.List)
                    site02.Verified(player);
            });
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            var modeType = Type.GetType($"RGM.Modes.FriendlyFire");
            if (modeType != null)
            {
                var modeInstance = Activator.CreateInstance(modeType);
                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                onEnabledMethod?.Invoke(modeInstance, null);
            }

            for (int i = 0; i < 300; i++)
            {
                Player.List.ToList().ForEach(x => x.ClearBroadcasts());
                Player.List.ToList().ForEach(x => x.Broadcast(2, $"<b><size=25><color=green>{300 - i}초 후</color> 게임이 종료됩니다.</size></b>"));
                yield return Timing.WaitForSeconds(1f);
            }

            Player first = Player.List.OrderByDescending(x => int.Parse(site02.Stage[x.UserId])).ToList()[0];
            List<Player> farthestPlayers = Player.List.Where(x => int.Parse(site02.Stage[x.UserId]) == int.Parse(site02.Stage[first.UserId])).ToList();
            string playerNames = string.Join(", ", farthestPlayers.Select(x => $"<color=#ffd700>{x.DisplayNickname}</color>(Stage {site02.Stage[x.UserId]})"));

            Player.List.ToList().ForEach(x => x.Broadcast(15, $"<b><size=35>가장 멀리 간 유저는 {playerNames}입니다!</size></b>"));
            Round.IsLocked = false;
        }
    }
}
