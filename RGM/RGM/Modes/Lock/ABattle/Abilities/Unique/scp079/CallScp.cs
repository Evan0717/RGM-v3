using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using RGM.API.DataBases;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp079;

// [Ability("SCP 지원 호출기", "A.I. 지능이 탑재된 SCP를 1개체 부릅니다. 이 개체는 모든 데미지를 50%만 받습니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_CALLSCP, RoleAbility.Scp079)]
public class CallScp : Ability
{
    public override void OnEnabled()
    {
        RoleTypeId _role = Tools.GetRandomValue(Datas.AIRoles.Where(x => !PlayerManager.List.ToList().Where(x => x.IsNPC).Select(x1 => x1.Role.Type).ToList().Contains(x)).ToList());

        Server.ExecuteCommand($"/spawnai {_role.ToString()}");

        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player.IsScpRole() && ev.Player.IsNPC)
            ev.DamageHandler.Damage /= 2;
    }
}
