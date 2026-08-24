using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using RGM.API.DataBases;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.RARE_SALAMANDRA, AbilityType.RARE_UNDINE, AbilityType.RARE_GNOME, AbilityType.RARE_SYLPH)]
[Ability("드루이드",
    """
    <살라만드라, 운디네, 노움, 실프> 4대 정령의 가호가 당신과 함께합니다.
    76% 확률(<color=red>SCP</color>의 경우 49%)로 상대방의 공격을 반사합니다.
    추가로, 4대 정령에 특수 능력이 부여됩니다.
    """,
    AbilityCategory.Synergy,
    AbilityType.SYNERGY_DRUID)]
public class Druid : Ability
{
    private static bool _isReflecting;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (_isReflecting ||
            ev.Player != Owner ||
            ev.Attacker == null ||
            !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub) ||
            Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type) ||
            WeakPointAttack.ShouldIgnoreDefenses(ev.Attacker))
            return;

        float reflectChance = ev.Player.IsScpRole() ? 49 : 76;

        if (!(UnityEngine.Random.Range(1, 101) <= reflectChance)) return;
        ev.IsAllowed = false;

        _isReflecting = true;
        try
        {
            ev.Attacker.Hit(ev.Player, ev.Amount);
            ev.Attacker.AddHint("드루이드", "당신의 공격이 반사되었습니다.");
            ev.Player.AddHint("드루이드", $"상대의 공격이 반사되었습니다.");
        }
        finally
        {
            _isReflecting = false;
        }
    }
}
