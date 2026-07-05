using System;
using System.Linq;
using AdminToys;
using Exiled.API.Features;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("노움", "웅장한 대지의 기운! 불, 물, 바람의 정령을 모으면..?", AbilityCategory.Rare, AbilityType.RARE_GNOME)]
public class Gnome : Ability
{
    public override void OnEnabled()
    {
        Light(Owner, new Color(0.588f, 0.294f, 0));
    }

    public override void OnDisabled()
    {
    }
    
    public void Light(Player player, Color color)
    {
        try
        {
            SchematicObject schematic = ObjectSpawner.SpawnSchematic("Light", Vector3.zero);
            LightSourceToy light = schematic.GetComponentsInChildren<LightSourceToy>().First();

            schematic.transform.parent = player.Transform;
            schematic.transform.localPosition = Vector3.zero;

            light.NetworkLightColor = color;
            light.NetworkLightRange = 50;
            light.NetworkLightIntensity = 20;

            Timing.CallDelayed(5, schematic.Destroy);
        }
        catch (NullReferenceException e)
        {
            Log.Warn("Failure to fetch object 'light'.");
        }
    }
}
