using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using ProjectMER.Events.Arguments;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Set, ModeType.AddScp)]
    class AddScp : Mode
    {
        public override string Name => "추가 SCP";
        public override string Description => "새로운 SCP들이 추가됩니다.";
        public override string Detail =>
"""
새로운 SCP..?
""";
        public override string Color => "fd0101";

        public static AddScp Instance;

        public override void OnEnabled()
        {
            ProjectMER.Events.Handlers.Schematic.SchematicSpawned += OnSchematicSpawned;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield break;
        }

        public void OnSchematicSpawned(SchematicSpawnedEventArgs ev)
        {
            if (ev.Name == "SCP_999")
            {
                IEnumerator<float> enumerator()
                {
                    while (true)
                    {
                        ev.Schematic.AnimationController.Stop();
                        string name = $"Normal{Random.Range(1, 3)}";
                        ev.Schematic.AnimationController.Play(name);
                        Tools.PlaySound(ev.Schematic.transform, "scp-999", 2);

                        yield return Timing.WaitForSeconds(Random.Range(2, 11));
                    }
                }

                Timing.RunCoroutine(enumerator());
            }
        }
    }
}
