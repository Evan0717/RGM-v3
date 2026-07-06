using Exiled.API.Features.Roles;
using RGM.Modes;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("발톱 강화", "발톱 공격의 데미지가 12 증가하고, 공격 시 20 HS를 회복합니다.(최대 1000)", RankAbilityType.흉내쟁이, RankCategory.SCP_939, RankAbilityCategory.변칙성, "☺")]
    public class 발톱강화 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }
        
        private void OnHurting(HurtingEventArgs ev)
        {
            if (Owner.Role is not Scp939Role scp939) return;
            if (ev.DamageHandler.Type != DamageType.Scp939) return;
            if (ev.Attacker.ReferenceHub == scp939.Owner.ReferenceHub && scp939.Owner.HumeShield < 1000) {
                scp939.Owner.HumeShield += 20;
            }
            ev.DamageHandler.Damage += 12;
        } 
    }
}
