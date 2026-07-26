using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Scp079;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProjectMER.Features.Objects;
using ProjectMER.Features;

namespace RGM.Modes.Abilities.Unique.Scp079.Mythic;


[Ability("융단 폭격", "핑을 찍은 지점 20m 이내에서 10초간 0.25초 간격으로 미사일이 쏟아집니다. (쿨타임 5초)", AbilityCategory.Mythic, AbilityType.MYTHIC_SCP079_FUSIONBOMB, RoleAbility.Scp079)]
public class FusionBomb : Ability
{
    bool isScp079Cooldown = false;
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player == null && ev.Player != Owner) return;

        try
        {
            if (!isScp079Cooldown)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    Vector3 centerPos = ev.Position + new Vector3(0, 0.1f, 0);
                    Timing.RunCoroutine(StartBombardment(centerPos));

                    isScp079Cooldown = true;
                    Timing.CallDelayed(5f, () =>
                    {
                        isScp079Cooldown = false;
                    });
                });
            }


        }
        catch (Exception e)
        {
            Log.Error($"융단 폭격 오류: {e}");
        }
    }

    public IEnumerator<float> StartBombardment(Vector3 centerPos)
    {
        float duration = 10f;
        float interval = 0.25f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * 20f;
            Vector3 randomTargetPos = centerPos + new Vector3(randomCircle.x, 0f, randomCircle.y);

            Timing.RunCoroutine(Boom(randomTargetPos));

            yield return Timing.WaitForSeconds(interval);
            elapsedTime += interval;
        }
    }

    public IEnumerator<float> Boom(Vector3 pos)
    {
        Vector3 RealPosition = pos;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hitDown, 100f, (LayerMask)1))
        {
            RealPosition = hitDown.point;
        }

        else if (Physics.Raycast(pos, Vector3.up, out RaycastHit hitUp, 100f, (LayerMask)1))
        {
            RealPosition = hitUp.point;
        }

        SchematicObject Missile = ObjectSpawner.SpawnSchematic("Missile", RealPosition);
        yield return Timing.WaitForSeconds(0.15f);
        if (Missile != null) 
            Missile.Destroy();

        SchematicObject Effect = ObjectSpawner.SpawnSchematic("Explosion", RealPosition);

        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Owner);
        if (g != null)
        {
            g.FuseTime = 0.1f;
            g.SpawnActive(RealPosition, Owner);
        }

        Timing.CallDelayed(6f, () => 
        {
            if (Effect != null) Effect.Destroy();
        });

        yield break;
    }
}