using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;
using static RGM.Variables.Variable;
using RGM.API.Features;

namespace DAONTFT.Core.EventArgs
{
    public static class ServerEvents
    {
        public static void OnWaitingForPlayers()
        {
            GlobalPlayer = AudioPlayer.CreateOrGet($"Global AudioPlayer", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000);
            });
        }

        public static IEnumerator<float> OnRoundStarted()
        {
            foreach (var player in Player.List)
            {
                player.AddBroadcast(10, "");
            }

            // --------------------------------------------------

            GlobalPlayer.AddClip("게임 시작");

            Round.IsLocked = true;

            Dictionary<Player, RoleTypeId> role = new();

            var encounter = Encounters.GetRandomValue();
            Encounter = encounter.Value.Item1;

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
                    if (Encounter == RoleTypeId.ChaosRepressor)
                    {
                        foreach (var player in Player.List)
                            player.AddItem(Tools.EnumToList<ItemType>().Where(x => x.IsWeapon()).GetRandomValue());
                    }

                    if (Encounter == RoleTypeId.ChaosMarauder)
                    {
                        foreach (var player in Player.List)
                            player.AddItem(UnityEngine.Random.Range(1, 3) == 1 ? ItemType.GrenadeFlash : ItemType.GrenadeHE);
                    }

                    if (Encounter == RoleTypeId.ChaosConscript)
                    {
                        foreach (var player in Player.List)
                            player.AddItem(Tools.EnumToList<ItemType>().Where(x => x.ToString().Contains("SCP")).GetRandomValue());
                    }

                    if (Encounter == RoleTypeId.ChaosRifleman)
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
                DAONTFT.Core.TFT.TFTBattle.StartUpgrade();
            });

            int getTime()
            {
                if (Encounter == RoleTypeId.ClassD)
                    return 100;

                if (Encounter == RoleTypeId.Scientist)
                    return 60;

                if (Encounter == RoleTypeId.FacilityGuard)
                    return 180;

                else
                    return 300;
            }

            int waitTime = getTime();

            while (true)
            {
                yield return Timing.WaitForSeconds(waitTime);

                DAONTFT.Core.TFT.TFTBattle.StartUpgrade();
            }
        }

        public static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var player in Player.List)
            {
                Server.ExecuteCommand($"/speak {player.Id} 1");
            }

            Timing.CallDelayed(9, () =>
            {
                Server.ExecuteCommand($"/sr");
            });
        }
    }
}
