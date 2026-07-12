using Exiled.API.Enums;
using Exiled.Events.EventArgs.Scp049;
using MEC;
using RGM.Modes;
using System.Collections.Generic;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("사신", "의사의 감각이 이동 속도를 15% 추가로 올려줍니다.", RankAbilityType.사신, RankCategory.SCP_049, RankAbilityCategory.변칙성, "👿")]
    public class 사신 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp049.ActivatingSense += OnActivatingSense;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp049.ActivatingSense -= OnActivatingSense;
        }

        void OnActivatingSense(ActivatingSenseEventArgs ev)
        {
            if (ev.Target != null && ev.Player == Owner)
            {
                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    IEnumerator<float> enumerator()
                    {
                        Owner.AddEffect(EffectType.MovementBoost, 15);

                        while (ev.Scp049.SenseAbility.HasTarget)
                            yield return Timing.WaitForSeconds(1);

                        Owner.RemoveEffect(EffectType.MovementBoost, 15);
                    };

                    Timing.RunCoroutine(enumerator());
                });
            }
        }
    }
}
