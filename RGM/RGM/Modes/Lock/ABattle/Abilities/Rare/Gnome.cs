using System;
using System.Linq;
using AdminToys;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using RGM.API.DataBases;
using RGM.Modes.Abilities.Synergy;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("노움", "웅장한 대지의 기운! 불, 물, 바람의 정령을 모으면..?", AbilityCategory.Rare, AbilityType.RARE_GNOME)]
public class Gnome : Ability
{
    public override void OnEnabled()
    {
        Light(Owner, new Color(0.588f, 0.294f, 0));
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner ||
            !Owner.HasAbility(AbilityType.SYNERGY_DRUID) ||
            Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type) ||
            WeakPointAttack.ShouldIgnoreDefenses(ev.Attacker))
        {
            return;
        }

        // 동일 능력을 여러 개 보유해도 드루이드 연계 효과는 한 번만 적용한다.
        if (ABattle.Instance.GetAbility(Owner, AbilityType.RARE_GNOME) != this)
            return;

        ev.DamageHandler.Damage *= 0.75f;
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
