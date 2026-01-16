using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using InventorySystem.Configs;
using MEC;
using Mirror;

using PlayerRoles;
using ProjectMER.Commands.Modifying.Position;
using ProjectMER.Events.Arguments;
using RGM.API.Components;
using RGM.API.DataBases;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using static RGM.Variables.Variable;

namespace RGM.EventArgs
{
    public static class MEREvents
    {
        private static List<ShootingTargetToy> targets = new();

        public static void OnSchematicSpawned(SchematicSpawnedEventArgs ev)
        {
            if (ev.Schematic.Name == "Cafeteria")
            {
                foreach (var targetBlock in ev.Schematic.AttachedBlocks.Where(x => x.name.StartsWith("Target/")))
                {
                    int num = int.Parse(targetBlock.name.Split('/')[1]);

                    PrefabType get()
                    {
                        if (num == 0)
                            return PrefabType.BinaryTarget;

                        if (num == 1)
                            return PrefabType.DBoyTarget;

                        return PrefabType.SportTarget;
                    }

                    ShootingTargetToy target = ShootingTargetToy.Get(PrefabHelper.Spawn(get()).GetComponent<ShootingTarget>());

                    target.Rotation = new Quaternion(0, 180, 0, 0);

                    targets.Add(target);

                    IEnumerator<float> work()
                    {
                        while (targetBlock != null)
                        {
                            Vector3 pos = targetBlock.transform.position;
                            target.Position = pos;

                            yield return Timing.WaitForOneFrame;
                        }
                    }

                    Timing.RunCoroutine(work());
                }
            }
        }

        public static void OnSchematicDestroyed(SchematicDestroyedEventArgs ev) 
        { 
            if (ev.Schematic.Name == "Cafeteria")
            {
                foreach (var target in targets.ToList())
                {
                    target.Destroy();

                    targets.Remove(target);
                }
            }
        }
    }
}
