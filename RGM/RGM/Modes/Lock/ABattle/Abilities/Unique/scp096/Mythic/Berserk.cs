using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp096.Mythic;

[Ability("광분", """
               공격 성공 시 폭발을 일으킵니다. 자신과 아군들은 해당 폭발에 피해를 받지 않습니다.
               추가로, 분노 조절문제, 천리안, 별자리 찢기 능력을 획득하며, 받는 데미지가 50%p 감소합니다.
               """, 
    AbilityCategory.Mythic,
    AbilityType.MYTHIC_SCP096_BERSERK, 
    RoleAbility.Scp096)]

public class Berserk : Ability
{
    bool _berserkExplosionActive;

    public override void OnEnabled()
    {
        Owner.AddAbility(AbilityType.NORMAL_SCP096_CANTMANAGEANGER);
        Owner.AddAbility(AbilityType.RARE_SCP096_SEER);
        Owner.AddAbility(AbilityType.EPIC_SCP096_STARTEARING);
        Owner.AddEffect(EffectType.DamageReduction, 100);
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        _berserkExplosionActive = false;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (_berserkExplosionActive &&
            ev.DamageHandler.Type == DamageType.Explosion &&
            ev.Attacker == Owner &&
            (ev.Player == Owner || ev.Player.Role.Team == Owner.Role.Team))
        {
            ev.IsAllowed = false;
            return;
        }

        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        if (ev.DamageHandler.Type != DamageType.Scp096)
            return;

        if (Owner.Role is not Scp096Role scp096 || !scp096.RageManager.IsEnraged)
            return;

        if (_berserkExplosionActive)
            return;

        _berserkExplosionActive = true;

        var grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Owner);
        grenade.FuseTime = 0.01f;
        grenade.SpawnActive(Owner.Position, Owner);

        Timing.CallDelayed(0.5f, () => _berserkExplosionActive = false);
    }
}
