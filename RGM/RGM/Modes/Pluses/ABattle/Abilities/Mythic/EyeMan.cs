using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Exiled.API.Enums;

namespace RGM.Modes.Abilities.Mythic;

[Ability("눈빛맨", "상대는 눈에 띄거나 근처에 있는 것만으로도 압도당할 것입니다!", AbilityCategory.Mythic, AbilityType.MYTHIC_EYEMAN)]
public class EyeMan : Ability
{
    CoroutineHandle _twinkle;

    public override void OnEnabled()
    {
        _twinkle = Timing.RunCoroutine(Twinkle());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_twinkle);
    }

    public IEnumerator<float> Twinkle()
    {
        while (true)
        {
            try
            {
                foreach (var near in Player.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, Owner.Position) < 11))
                {
                    if (Owner != near && HitboxIdentity.IsEnemy(Owner.ReferenceHub, near.ReferenceHub))
                    {
                        near.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                        near.EnableEffect(EffectType.Blinded, 1, 0.2f);
                        near.Hurt(near.MaxHealth / 120, "눈빛의 힘에 압도당했습니다.");
                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 1f);
                    }
                }

                if (Tools.TryGetLookPlayer(Owner, 100f, out Player target, out RaycastHit? hit))
                {
                    if (Owner != target && HitboxIdentity.IsEnemy(Owner.ReferenceHub, target.ReferenceHub))
                    {
                        target.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                        target.EnableEffect(EffectType.Blinded, 1, 0.2f);
                        target.Hurt(target.MaxHealth / 120, "눈빛의 힘에 압도당했습니다.");
                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 0.5f);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"눈빛맨 오류: {e}");
            }

            yield return Timing.WaitForSeconds(0.1f);
        }
    }
}