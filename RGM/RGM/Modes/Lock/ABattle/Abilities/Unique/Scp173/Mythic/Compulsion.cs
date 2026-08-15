using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles.PlayableScps;
using RGM.API.Features;
using RGM.Modes.Abilities.Legend;
using RGM.Modes.Abilities.Synergy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RGM.Modes.Abilities.Unique.Scp173.Mythic;

[Ability("강박증",
    "50m 이내의 플레이어가 자신을 강제로 쳐다보게 되지만, 모든 데미지를 1로 받습니다.",
    AbilityCategory.Mythic,
    AbilityType.MYTHIC_SCP173_COMPULSION,
    RoleAbility.Scp173)]

public class Compulsion : Ability
{
    private const float MaxDamage = 1f;
    private int visionMask = VisionInformation.VisionLayerMask;
    private CoroutineHandle Handle;
    public override void OnEnabled()
    {
        Handle = Timing.RunCoroutine(WeMustLookAt());
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }
    public override void OnDisabled()
    {
        Timing.KillCoroutines(Handle);
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }
    
    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner ||
            ev.DamageHandler.Type == DamageType.Crushed ||
            WeakPointAttack.ShouldIgnoreDefenses(ev.Attacker) ||
            ZeroRule.ShouldIgnoreDefenses(ev))
            return;

        if (ev.IsInstantKill)
        {
            ev.IsAllowed = false;
            ev.Player.Hurt(MaxDamage, ev.DamageHandler.Type);

            return;
        }

        if (ev.DamageHandler.Damage > MaxDamage)
        {
            ev.DamageHandler.Damage = MaxDamage;
        }
    }
    private IEnumerator<float> WeMustLookAt()
    {
        while (Owner != null && Owner.IsAlive)
        {
            foreach (var target in PlayerManager.List.Where(target => target != Owner && target.IsAlive && Vector3.Distance(target.Position, Owner.Position) <= 50f))
            {
                if (Physics.Linecast(target.CameraTransform.position, Owner.CameraTransform.position, visionMask)) continue;
                target.ForceLookAt(Owner.CameraTransform.position);
                target.AddHint("강박증", "<b><color=#FF0000>눈을 떼지 않으면 안돼...</color></b>");
            }
            yield return Timing.WaitForOneFrame;
        }
    }
}
