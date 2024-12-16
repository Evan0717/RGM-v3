using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Extensions;
using RGM.API.Features;
using PlayerRoles;
using RGM.API.DataBases;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Disguise)]
    public class Disguise : Mode
    {
        public override string Name => "변장";
        public override string Description => "모든 유저의 모습이 랜덤하게 변경됩니다.";
        public override string Detail =>
"""
* 근접하면 실제 역할군이 표시됩니다.

<b>변장 역할군 제외 목록</b>
SCP-079
관전자
오버워치
필름메이커
""";
        public override string Color => "F1F8E0";

        public static Disguise Instance;

        static List<RoleTypeId> _blockedRoles = new List<RoleTypeId>()
        {
            RoleTypeId.Spectator,
            RoleTypeId.Scp079,
            RoleTypeId.Overwatch,
            RoleTypeId.Filmmaker
        };
        List<RoleTypeId> _roleList = Tools.EnumToList<RoleTypeId>().Where(x => !_blockedRoles.Contains(x)).ToList();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
                Spawned(player);

            while (true)
            {
                foreach (var p in Player.List)
                {
                    if (Tools.TryGetLookPlayer(p, 3, out Player t))
                        p.ShowHint($"<size=25><b>진실의 눈은 그를 <color={t.Role.Color.ToHex()}>{Trans.Role[t.Role.Type]}</color>(으)로 판별했습니다.</b></size>", 1);

                    else
                        p.ShowHint($"<size=25><b>당신의 정체는 <color={p.Role.Color.ToHex()}>{Trans.Role[p.Role.Type]}</color>입니다.</b></size>", 1);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (_blockedRoles.Contains(player.Role))
                return;

            Timing.CallDelayed(1f, () =>
            {
                player.ChangeAppearance(Tools.GetRandomValue(_roleList));
            });
        }
    }
}
