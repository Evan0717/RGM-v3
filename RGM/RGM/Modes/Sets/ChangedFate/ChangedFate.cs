using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using UnityEngine;
using Exiled.API.Enums;
using RGM.API.Features;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.ChangedFate)]
    public class ChangedFate : Mode
    {
        public override string Name => "뒤바뀐 운명";
        public override string Description => "각 진영의 운명이 뒤바뀐다는 예언이 있었습니다.";
        public override string Detail =>
$"""
<b>각 진영의 스폰 위치는 같지만, 역할군이 변경됩니다.</b>

<color={RoleTypeId.ClassD.GetColor().ToHex()}>D계급</color> <-> <color={RoleTypeId.Scientist.GetColor().ToHex()}>과학자</color>
<color={RoleTypeId.FacilityGuard.GetColor().ToHex()}>시설 경비</color> <-> <color={RoleTypeId.Scp079.GetColor().ToHex()}>SCP</color>
<color={RoleTypeId.NtfSergeant.GetColor().ToHex()}>NTF</color> <-> <color={RoleTypeId.ChaosMarauder.GetColor().ToHex()}>혼돈의 반란</color>
""";
        public override string Color => "B40486";
        public override string Suggester => "idea by A(@idk_9936)";

        public static ChangedFate Instance;

        static List<RoleTypeId> ignoredRoles = new List<RoleTypeId>
        {
            RoleTypeId.Scp3114,
            RoleTypeId.Scp0492
        };
        Dictionary<RoleTypeId, List<RoleTypeId>> roleTypeIds = new Dictionary<RoleTypeId, List<RoleTypeId>>()
        {
            { RoleTypeId.ClassD, new List<RoleTypeId> { RoleTypeId.Scientist } },
            { RoleTypeId.Scientist, new List<RoleTypeId> { RoleTypeId.ClassD } },
            { RoleTypeId.FacilityGuard, Tools.EnumToList<RoleTypeId>().Where(x => x.IsScpRole() && !ignoredRoles.Contains(x)).ToList() }
        };

        RoleTypeId selectRole(Player player)
        {
            if (roleTypeIds.ContainsKey(player.Role.Type))
            {
                List<RoleTypeId> roles = roleTypeIds[player.Role.Type];
                return roles.GetRandomValue();
            }
            else
            {
                if (player.IsScpRole())
                    return RoleTypeId.FacilityGuard;

                if (player.IsNTF)
                    return Tools.EnumToList<RoleTypeId>().Where(x => x.IsChaos()).GetRandomValue();

                if (player.IsCHI)
                    return Tools.EnumToList<RoleTypeId>().Where(x => x.IsNtf()).GetRandomValue();

                return RoleTypeId.Tutorial;
            }
        }

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
                RoleTypeId roleType = selectRole(player);
                player.Role.Set(roleType, SpawnReason.ItemUsage, RoleSpawnFlags.AssignInventory);
            }

            yield break;
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive && ev.Reason != SpawnReason.ItemUsage)
            {
                RoleTypeId roleType = selectRole(ev.Player);
                ev.Player.Role.Set(roleType, SpawnReason.ItemUsage, RoleSpawnFlags.AssignInventory);
            }
        }
    }
}
