using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("보호 장비", "부정적 특성 효과를 받을 경우 4초 뒤 강제 해제합니다.", RankAbilityType.보호장비, RankCategory.반란, RankAbilityCategory.변칙성, "😷")]
    public class 보호장비 : RankAbility
    {
        private static readonly HashSet<EffectType> KeptBuffs =
        [
            EffectType.Scp1853,
            EffectType.Invigorated,
            EffectType.Invisible,
            EffectType.RainbowTaste,
            EffectType.BodyshotReduction,
            EffectType.DamageReduction,
            EffectType.MovementBoost,
            EffectType.Vitality,
            EffectType.SpawnProtected,
            EffectType.Ghostly,
            EffectType.SilentWalk,
            EffectType.Fade,
            EffectType.FocusedVision,
            EffectType.AnomalousRegeneration,
            EffectType.Scp1344,
            EffectType.Scp207,
            EffectType.AntiScp207,
            EffectType.Lightweight,
            EffectType.NightVision,
            EffectType.FogControl,
            EffectType.PitDeath
        ];
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

            if (!KeptBuffs.Contains(effectType))
            {
                Timing.CallDelayed(4f, () =>
                {
                    ev.Player.DisableEffect(effectType);
                });
            }
        }
    }
}
