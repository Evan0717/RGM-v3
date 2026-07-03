using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Mythic;

[Ability("패왕색 패기", "누군가가 당신을 쳐다본다면, 그 사람은 이제 없는 존재가 되겠지요!", AbilityCategory.Mythic, AbilityType.MYTHIC_KINGSCOLOR)]
public class KingsColor : Ability
{
    CoroutineHandle king;
    int count = 0;
    LabApi.Features.Wrappers.LightSourceToy lightSource;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnDied;

        king = Timing.RunCoroutine(kingsColor());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;

        Timing.KillCoroutines(king);
    }

    public IEnumerator<float> kingsColor()
    {
        lightSource = LabApi.Features.Wrappers.LightSourceToy.Create();
        lightSource.Color = Color.red;
        lightSource.Intensity = 40;
        lightSource.Range = 10;

        while (Owner.IsAlive)
        {
            foreach (var player in PlayerManager.List)
            {
                if (Tools.TryGetLookPlayer(player, 90f, out Player target, out RaycastHit? hit))
                {
                    if (Owner == target && HitboxIdentity.IsEnemy(player.ReferenceHub, target.ReferenceHub))
                    {
                        lightSource.Position = Owner.Position;

                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 1f);
                        player.EnableEffect(EffectType.Slowness, 80, 0.8f);
                        player.CurrentItem = null;
                        player.Hit(Owner, target.IsScpRole() ? target.MaxHealth * 0.019f : target.MaxHealth * 0.113f);
                    }
                }
            }

            yield return Timing.WaitForOneFrame;
        }

        lightSource.Destroy();
    }

    public void OnDied(DiedEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        count++;

        switch (count)
        {
            case 5:
            {
                Tools.PlayGlobalAudio("시산혈해 (屍山血海)", 2.5f);

                Owner.RankName = "시산혈해 (屍山血海)";
                Owner.RankColor = "red";

                foreach (var player in PlayerManager.List.Where(x => Owner.LeadingTeam != x.LeadingTeam))
                {
                    player.EnableEffect(EffectType.SinkHole, 1, 3);
                    player.EnableEffect(EffectType.Blinded, 1, 3);
                }

                break;
            }
            case 30:
            {
                foreach (var player in PlayerManager.List.Where(x => x != Owner))
                {
                    player.Kill("패기에 의해 공중분해 되었습니다");
                }

                break;
            }
        }
    }
}