using Exiled.API.Features.Items;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Server;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Outlaw)]
    public class Outlaw : Mode
    {
        public override string Name => "무법자";
        public override string Description => "모두가 총기 하나를 가지고 시작합니다.";
        public override string Detail =>
"""
무기 아이템 중에서 랜덤으로 지급받습니다.

SCP는 매 지원마다 새로운 무기를 받습니다.

* 게임 시작 14분 뒤 <color=red>자동핵</color>이 작동됩니다.
""";
        public override string Color => "9F81F7";

        public static Outlaw Instance;

        CoroutineHandle _onModeStarted;
        CoroutineHandle _autoWarhead;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _autoWarhead = Timing.RunCoroutine(AutoWarhead());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;

            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_autoWarhead);
        }

        public IEnumerator<float> OnModeStarted()
        {
            if (Random.Range(1, 101) <= 10) { //10% 확률로 워크스테이션 업그레이드 시작
                Tools.TryInstallMode(ModeType.ABattle);
            }
            yield return Timing.WaitForSeconds(1f);

            foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x.Role.Type != RoleTypeId.Scp079))
                Spawned(player);

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                if (player.IsAlive && player.Role.Type != RoleTypeId.Scp079)
                {
                    Item weapon = player.AddItem(Tools.EnumToList<ItemType>().Where(x => x.ToString().Contains("Gun")).ToList().GetRandomValue());

                    if (weapon.Type == ItemType.GrenadeHE)
                        player.AddItem(ItemType.GrenadeHE, 2);

                    if (weapon is Firearm firearm)
                    {
                        if (firearm.AmmoType != AmmoType.None)
                        {
                            for (int i = 0; i < 3; i++)
                                player.AddItem(firearm.AmmoType.GetItemType());
                        }
                    }
                }
            });
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
                Spawned(player);
        }
        
        public IEnumerator<float> AutoWarhead()
        {
            yield return Timing.WaitForSeconds(13 * 60);

            if (Warhead.IsDetonated)
                yield break;

            Tools.MessageTranslated("", $"1분 뒤 <color=red>자동핵</color>이 작동됩니다.");

            if (Warhead.IsDetonated)
                yield break;

            yield return Timing.WaitForSeconds(1 * 60);

            DeadmanSwitch.StartWarhead();
        }
    }
}
