using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("보호 장비", "섬광 및 화상 효과에 면역을 가집니다.", RankAbilityType.방독면, RankCategory.반란, RankAbilityCategory.변칙성, "😷")]
    public class 보호장비 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
        }
        List<EffectType> _effects =
        [
            EffectType.Blurred,
            EffectType.Deafened,
            EffectType.Flashed,
            EffectType.Burned
        ];
        public void OnReceivingEffect(ReceivingEffectEventArgs ev)
        {
            if (ev.Player != Owner) return;
        
            if (ev.Effect.GetEffectType() == EffectType.Flashed || ev.Effect.GetEffectType() == EffectType.Burned)
            {
                Timing.CallDelayed(Timing.WaitForOneFrame, () => {
                    foreach (var effect in _effects) {
                        ev.Player.DisableEffect(effect);
                    }
                });
            }
        }
    }
}
