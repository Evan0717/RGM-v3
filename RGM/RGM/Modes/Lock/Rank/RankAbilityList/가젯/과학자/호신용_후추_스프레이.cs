using Exiled.API.Features;
using Exiled.API.Enums;
using RGM.Modes;
using System.Linq;
using RGM.API.Features;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("호신용_후추_스프레이", "근접해 있는 적에게 일시적으로 부식과 흐릿함, 감속 효과를 부여합니다.", RankAbilityType.호신용_후추_스프레이, RankCategory.과학자, "🎢", 60)]
    public class 호신용_후추_스프레이 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            foreach (var enemy in Player.List.Where(x => Vector3.Distance(x.Position, Owner.Position) < 4 && HitboxIdentity.IsEnemy(x.ReferenceHub, Owner.ReferenceHub)))
            {
                enemy.AddEffect(EffectType.Blinded, 10, 3);
                enemy.AddEffect(EffectType.Corroding, 1, 3);
                enemy.AddEffect(EffectType.Slowness, 40, 3);
            }
        }
    }
}
