using Exiled.API.Enums;
using MEC;
using RGM.Modes;
using RGM.API.Features;
using static RGM.Variables.Variable;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("비브라늄", "0.5초간 어떠한 피해도 받지 않는 대신, 움직일 수 없습니다.", RankAbilityType.비브라늄, RankCategory.과학자, "💍")]
    public class 비브라늄 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            if (!GodModePlayers.Contains(Owner))
                GodModePlayers.Add(Owner);

            Owner.AddEffect(EffectType.Ensnared, 1, 0.5f);
            Owner.AddEffect(EffectType.HeavyFooted, 100, 0.5f);

            Timing.CallDelayed(0.5f, () =>
            {
                if (GodModePlayers.Contains(Owner))
                    GodModePlayers.Remove(Owner);
            });
        }
    }
}
