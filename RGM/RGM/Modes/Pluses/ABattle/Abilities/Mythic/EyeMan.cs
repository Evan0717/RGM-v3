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
using ProjectMER.Features.Objects;
using ProjectMER.Features;
using Mirror;
using LabApi.Features.Wrappers;

namespace RGM.Modes.Abilities.Mythic;

[Ability("눈빛맨", "상대는 눈에 띄는 것만으로도 압도당할 것입니다!", AbilityCategory.Mythic, AbilityType.MYTHIC_EYEMAN)]
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
        SchematicObject beam = ObjectSpawner.SpawnSchematic("눈빛맨", new Vector3(1205, 1205, 1205));
        beam.GetComponentsInChildren<PrimitiveObjectToy>().ToList().ForEach(x => x.MovementSmoothing = 0);

        while (Owner.IsAlive)
        {
            try
            {
                //foreach (var near in PlayerManager.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, Owner.Position) < 11))
                //{
                //    if (Owner != near && HitboxIdentity.IsEnemy(Owner.ReferenceHub, near.ReferenceHub))
                //    {
                //        near.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                //        near.EnableEffect(EffectType.Blinded, 1, 0.2f);
                //        near.Hurt(near.MaxHealth / 240, "눈빛의 힘에 압도당했습니다.");
                //        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 1f);
                //    }
                //}

                if (Tools.TryGetLookPlayer(Owner, 100f, out Exiled.API.Features.Player target, out RaycastHit? hit))
                {
                    if (Owner != target && HitboxIdentity.IsEnemy(Owner.ReferenceHub, target.ReferenceHub))
                    {
                        beam.Position = Owner.CameraTransform.position + Owner.CameraTransform.forward * 0.3f;
                        beam.Rotation = Quaternion.LookRotation(Owner.CameraTransform.forward);
                        target.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                        target.EnableEffect(EffectType.Blinded, 1, 0.2f);
                        target.CurrentItem = null;
                        target.Hit(Owner, target.IsScpRole() ? target.MaxHealth / 240 : target.MaxHealth / 60);
                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 0.5f);
                    }
                    else
                        beam.Position = new Vector3(1205, 1205, 1205);
                }
                else
                    beam.Position = new Vector3(1205, 1205, 1205);
            }
            catch (Exception e)
            {
                Log.Error($"눈빛맨 오류: {e}");
            }

            yield return Timing.WaitForOneFrame;
        }

        NetworkServer.Destroy(beam.gameObject);
        beam.Destroy();
    }
}