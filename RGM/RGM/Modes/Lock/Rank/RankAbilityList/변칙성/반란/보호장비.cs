using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("보호 장비", "부정적 특성 효과를 받을 경우 4초 뒤 강제 해제합니다.", RankAbilityType.보호장비, RankCategory.반란, RankAbilityCategory.변칙성, "😷")]
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
        
        public void OnReceivingEffect(ReceivingEffectEventArgs ev)
        {
            if (ev.Player != Owner) return;
            
            var effectType = ev.Effect.GetEffectType();

            if (!EffectManager.IsKeptBuff(effectType))
            {
                Timing.CallDelayed(4f, () =>
                {
                    ev.Player.DisableEffect(effectType);
                });
            }
        }
    }
}
