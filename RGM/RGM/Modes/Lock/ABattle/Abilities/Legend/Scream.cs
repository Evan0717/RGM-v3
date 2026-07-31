using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

[Ability("괴성", "적을 보고 있을 때 마이크를 키면 모든 적군을 일정 시간동안 행동 불가 상태로 만들고, 무장을 해제합니다. (쿨타임 60초)\n이후 영향을 받은 대상에게 30초간 출혈 효과를 부여합니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCREAM)]
public class GmanRoaringSound : Ability
{
    private const int RoaringSoundCooldownDuration = 60;
    private int _roaringSoundCooldown;

    public override void OnEnabled()
        => Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;

    public override void OnDisabled() 
        => Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;

    public IEnumerator<float> OnVoiceChatting(VoiceChattingEventArgs ev)
    {
        if (ev.Player != Owner)
            yield break;

        if (_roaringSoundCooldown > 0)
            yield break;

        if (!ev.Player.TryGetLookPlayer(16f, out Player target, out RaycastHit? hit) ||
            !HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, target.ReferenceHub)) yield break;
        
        _roaringSoundCooldown = RoaringSoundCooldownDuration;
        Timing.CallDelayed(RoaringSoundCooldownDuration, () => _roaringSoundCooldown = 0);

        Tools.PlayGlobalAudio("GmanRoaringSound");

        foreach (var player in PlayerManager.List.Where(x => !x.IsNPC && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, x.ReferenceHub) && x.IsAlive))
        {
            player.EnableEffect(EffectType.Ensnared, 1, 1.5f);
            player.EnableEffect(EffectType.Flashed, 1, 3f);
            player.EnableEffect(EffectType.Blurred, 1, 15f);
            player.EnableEffect(EffectType.Deafened, 1, 15f);
            player.EnableEffect(EffectType.SinkHole, 1, 10f);
            player.EnableEffect(EffectType.Slowness, 120, 6f);
            player.EnableEffect(EffectType.Disabled, 1, 15f);
            player.EnableEffect(EffectType.Stained, 1, 5f);
            player.EnableEffect(EffectType.AmnesiaItems, 1, 15f);
            player.EnableEffect(EffectType.Bleeding, 1, 30f);

            player.DropItems();
        }

        yield return Timing.WaitForSeconds(0.65f);

        PlayerManager.List.ToList().ForEach(x => x.AddHint("저주받은 괴성", "<b><color=#B08A03>저주받은 괴성</color></b>", 5));
    }
}
