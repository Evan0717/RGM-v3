using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("23분 카레", "체력이 1200이 되는 대신 탄약을 사용하는 총기 관련 데미지에 85% 저항을 가지며, 흄 쉴드가 90% 감소합니다.", RankAbilityType.이십삼분_카레, RankCategory.SCP_106, RankAbilityCategory.변칙성, "🍛")]
    public class 이십삼분_카레 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.MaxHealth = 1200;
            Owner.Health = Owner.MaxHealth;
            Owner.MaxHumeShield *= 0.1f;
            Owner.HumeShield = Owner.MaxHumeShield;

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player == Owner && ev.DamageHandler.Type.IsWeapon(false))
                ev.DamageHandler.Damage *= 0.15f;
        }
    }
}
