using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Legend;

[Ability("괴성", "적을 보고 있을 때 마이크를 키면 시설 내의 적들을 일시적으로 둔해지게 만듭니다. (쿨타임 100초)", AbilityCategory.Legend, AbilityType.LEGEND_SCREAM)]
public class GmanRoaringSound : Ability
{
    int RoaringSoundCooldown = 0;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;
    }

    public IEnumerator<float> OnVoiceChatting(VoiceChattingEventArgs ev)
    {
        if (ev.Player != Owner)
            yield break;

        if (RoaringSoundCooldown <= 0)
        {
            if (Tools.TryGetLookPlayer(ev.Player, 10f, out Player target, out RaycastHit? hit) && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, target.ReferenceHub))
            {
                RoaringSoundCooldown = 180;

                Tools.PlayGlobalAudio("GmanRoaringSound");

                foreach (var player in PlayerManager.List.Where(x => !x.IsNPC && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, x.ReferenceHub) && x.IsAlive))
                {
                    player.EnableEffect(EffectType.Flashed, 1, 0.3f);
                    player.EnableEffect(EffectType.Blinded, 1, 7.5f);
                    player.EnableEffect(EffectType.SinkHole, 1, 12f);
                    player.EnableEffect(EffectType.Slowness, 150, 5.5f);
                }

                yield return Timing.WaitForSeconds(0.65f);

                PlayerManager.List.ToList().ForEach(x => x.AddHint("저주받은 괴성", "<b><i><color=#B08A03>저주받은 괴성</color></i></b>", 5));

                for (int i = 1; i < 71; i++)
                {
                    Warhead.Shake();

                    yield return Timing.WaitForOneFrame;
                }

                Timing.CallDelayed(100, () =>
                {
                    RoaringSoundCooldown = 0;
                });
            }
        }
    }
}
