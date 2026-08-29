using System;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("살라만드라", "뜨거운 열정의 기운! 물, 흙, 바람의 정령을 모으면..?", AbilityCategory.Rare, AbilityType.RARE_SALAMANDRA)]
public class Salamandra : Ability
{
    public override void OnEnabled()
    {
        Light(Owner, Color.red);
        Exiled.Events.Handlers.Player.Hurt += OnHurt;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurt -= OnHurt;
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

        ev.Player.EnableEffect(EffectType.Burned, 1, 5f);
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
        catch (NullReferenceException)
        {
            Log.Warn("Failure to fetch object 'light'.");
        }
    }
}
