using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("실프", "싱그러운 바람의 기운! 물, 불, 흙의 정령을 모으면..?", AbilityCategory.Rare, AbilityType.RARE_SYLPH)]
public class Sylph : Ability
{
    private const byte MovementBoostPerStack = 8;
    private const byte MaxStacks = 8;
    private const float CombatTimeout = 5f;

    private CoroutineHandle _stackingCoroutine;
    private byte _stacks;
    private bool _isInCombat;
    private float _lastHitTime;

    public override void OnEnabled()
    {
        Light(Owner, Color.green);

        // 드루이드는 실프가 추가된 뒤 시너지 처리 과정에서 부여되므로,
        // 활성화 시점이 아닌 피격/스택 처리 시점에 보유 여부를 확인한다.
        Exiled.Events.Handlers.Player.Hurt += OnHurt;
        _stackingCoroutine = Timing.RunCoroutine(ManageStacks());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurt -= OnHurt;
        Timing.KillCoroutines(_stackingCoroutine);
        ResetStacks();
    }

    private void OnHurt(HurtEventArgs ev)
    {
        if (ev.Attacker != Owner ||
            ev.DamageHandler.Damage <= 0f ||
            !Owner.HasAbility(AbilityType.SYNERGY_DRUID) ||
            !HitboxIdentity.IsEnemy(Owner.ReferenceHub, ev.Player.ReferenceHub))
        {
            return;
        }

        _isInCombat = true;
        _lastHitTime = Time.time;
    }

    private IEnumerator<float> ManageStacks()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(1f);

            if (!Owner.IsAlive || !Owner.HasAbility(AbilityType.SYNERGY_DRUID))
            {
                ResetStacks();
                continue;
            }

            if (!_isInCombat)
                continue;

            if (Time.time - _lastHitTime >= CombatTimeout)
            {
                ResetStacks();
                continue;
            }

            if (_stacks >= MaxStacks)
                continue;

            _stacks++;
            Owner.AddEffect(EffectType.MovementBoost, MovementBoostPerStack);
        }
    }

    private void ResetStacks()
    {
        if (_stacks > 0)
            Owner.RemoveEffect(EffectType.MovementBoost, (byte)(_stacks * MovementBoostPerStack));

        _stacks = 0;
        _isInCombat = false;
        _lastHitTime = 0f;
    }
   
    private static void Light(Player player, Color color)
    {
        try
        {
            SchematicObject schematic = ObjectSpawner.SpawnSchematic("Light", Vector3.zero);
            LightSourceToy light = schematic.GetComponentsInChildren<LightSourceToy>().First();

            schematic.transform.parent = player.Transform;
            schematic.transform.localPosition = Vector3.zero;

            light.NetworkLightColor = color;
            light.NetworkLightRange = 50;
            light.NetworkLightIntensity = 8;

            Timing.CallDelayed(5, schematic.Destroy);
        }
        catch (NullReferenceException e)
        {
            Log.Warn("Failure to fetch object 'light'.");
        }
    }
}
