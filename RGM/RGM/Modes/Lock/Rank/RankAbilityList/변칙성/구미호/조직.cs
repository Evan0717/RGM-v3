using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("조직", "주변에 있는 아군 수만큼 공격력이 2.36% 증가합니다.", RankAbilityType.조직, RankCategory.구미호, RankAbilityCategory.변칙성, "🏢")]
    public class 조직 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker == Owner)
            {
                int count = 0;

                foreach (var player in Player.List.Where(x => x.IsNTF && Vector3.Distance(Owner.Position, x.Position) < 10))
                    count++;

                ev.DamageHandler.Damage *= 1 + (0.0236f * count);
            }
        }
    }
}
