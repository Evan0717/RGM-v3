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

[Ability("괴성", """
               적을 보고 있을 때 마이크를 키면 모든 적군을 3초간 『기절』 상태로 만들고, 15초간 『무장 해제』 효과를 부여합니다. (쿨타임 60초)
               이후 영향을 받은 대상에게 30초간 출혈 효과를 부여합니다.
               """, AbilityCategory.Legend, AbilityType.LEGEND_SCREAM)]
public class GmanRoaringSound : Ability
{
    private const int RoaringSoundCooldownDuration = 60;
    private const float DisarmDuration = 15f;
    private int _roaringSoundCooldown;
    private readonly HashSet<Player> _disarmedPlayers = new();

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
        Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;
        Exiled.Events.Handlers.Player.ChangingItem -= OnChangingItem;
        _disarmedPlayers.Clear();
    }

    private void OnChangingItem(ChangingItemEventArgs ev)
    {
        if (_disarmedPlayers.Contains(ev.Player))
            ev.IsAllowed = false;
    }

    private IEnumerator<float> OnVoiceChatting(VoiceChattingEventArgs ev)
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

        _disarmedPlayers.Clear();

        foreach (var player in PlayerManager.List.Where(x => !x.IsNPC && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, x.ReferenceHub) && x.IsAlive))
        {
            player.EnableEffect(EffectType.Ensnared, 1, 1.5f);
            player.EnableEffect(EffectType.Flashed, 1, 3f);
            player.EnableEffect(EffectType.Blurred, 1, 15f);
            player.EnableEffect(EffectType.Deafened, 1, 15f);
            player.EnableEffect(EffectType.SinkHole, 1, 6f);
            player.EnableEffect(EffectType.Slowness, 120, 6f);
            player.EnableEffect(EffectType.Disabled, 1, 15f);
            player.EnableEffect(EffectType.Stained, 1, 5f);
            player.EnableEffect(EffectType.AmnesiaItems, 1, 15f);
            if (player.Role.Type != RoleTypeId.Scp079)
                player.EnableEffect(EffectType.Bleeding, 1, 30f);

            player.CurrentItem = null;
            _disarmedPlayers.Add(player);
        }

        Timing.CallDelayed(DisarmDuration, () => _disarmedPlayers.Clear());

        yield return Timing.WaitForSeconds(0.65f);

        PlayerManager.List.ToList().ForEach(x => x.AddHint("저주받은 괴성", "<b><color=#B08A03>저주받은 괴성</color></b>", 5));
    }
}
