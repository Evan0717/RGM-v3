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

namespace RGM.Modes.Abilities.Legend;

[Ability("괴성", "적을 보고 있을 때 마이크를 키면 시설 내의 적들을 일시적으로 둔해지게 만듭니다. (쿨타임 100초)", AbilityCategory.Legend, AbilityType.LEGEND_SCREAM)]
public class Scream : Ability
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
        if (RoaringSoundCooldown <= 0)
        {
            if (Tools.TryGetLookPlayer(ev.Player, 10f, out Player target) && target.LeadingTeam != ev.Player.LeadingTeam)
            {
                RoaringSoundCooldown = 180;

                string sn = $"{UnityEngine.Random.Range(-9999999.9f, 9999999.9f)}";
                Player dj = Tools.SpawnDJ($"{ev.Player.Nickname}의 괴성", RoleTypeId.Spectator, Vector3.zero, sn);

                GGUtils.Gtool.PlaySound(sn, "GmanRoaringSound", VoiceChat.VoiceChatChannel.Intercom);

                foreach (var player in Player.List.Where(x => !x.IsNPC && x.LeadingTeam != ev.Player.LeadingTeam && x.IsAlive))
                {
                    player.EnableEffect(EffectType.Flashed, 1, 0.3f);
                    player.EnableEffect(EffectType.Blinded, 1, 7.5f);
                    player.EnableEffect(EffectType.SinkHole, 1, 12f);
                    player.EnableEffect(EffectType.Slowness, 150, 5.5f);
                }

                yield return Timing.WaitForSeconds(0.65f);

                Player.List.ToList().ForEach(x => x.ShowHint("<b><i><color=#B08A03>저</color><color=#9C7A02>?</color><color=#886B02>!</color><color=#755C01>주</color><color=#614C01>받</color><color=#4E3D01>은</color> <color=#271E00>괴</color><color=#130F00>성</color></i></b>", 5));

                for (int i = 1; i < 71; i++)
                {
                    Warhead.Shake();

                    yield return Timing.WaitForSeconds(0.1f);
                }

                Timing.CallDelayed(5f, () =>
                {
                    NetworkServer.Destroy(dj.ReferenceHub.gameObject);
                });

                Timing.CallDelayed(100, () =>
                {
                    RoaringSoundCooldown = 0;
                });
            }
        }
    }
}
