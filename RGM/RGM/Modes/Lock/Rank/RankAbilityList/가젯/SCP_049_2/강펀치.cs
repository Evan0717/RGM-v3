using Exiled.Events.EventArgs.Player;
using RGM.Modes;

using static RGM.Variables.Variable;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("강펀치", "스킬 사용 후 다음 공격은 적을 즉사시킵니다.", RankAbilityType.강펀치, RankCategory.SCP_049_2, "👊", 120)]
    public class 강펀치 : RankGadgetAbility
    {
        bool isEnabled = false;

        protected override bool CanUseGadget()
        {
            return !isEnabled;
        }

        protected override void OnGadgetUsed()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            isEnabled = true;

            void OnHurting(HurtingEventArgs ev)
            {
                if (ev.Attacker == null || ev.Attacker != Owner)
                    return;

                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);

                ev.DamageHandler.Damage = ev.Player.MaxHealth + ev.Player.MaxArtificialHealth + ev.Player.MaxHumeShield;
                Exiled.Events.Handlers.Player.Hurting -= OnHurting;
                isEnabled = false;
            }
        }
    }
}
