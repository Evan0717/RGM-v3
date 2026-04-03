using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.Patches.Events.Scp049;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using RGM.Modes;
using SLPlayerRotation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("본능", "주변에 있는 인간들이 점점 위를 올려다보게 됩니다.", RankAbilityType.본능, RankCategory.SCP_096, RankAbilityCategory.변칙성, "❓")]
    public class 본능 : RankAbility
    {
        CoroutineHandle handle;

        public override void OnEnabled()
        {
            handle = Timing.RunCoroutine(enumerator());
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(handle);
        }

        IEnumerator<float> enumerator()
        {
            while (true)
            {
                if (Owner.Role is Scp096Role scp096)
                {
                    foreach (var human in PlayerManager.List.Where(x => !x.IsScpRole() && !scp096.Targets.Contains(x)))
                    {
                        if (Vector3.Distance(Owner.Position, human.Position) < 2)
                        {
                            var currentEuler = human.CameraTransform.rotation.eulerAngles;
                            currentEuler.x -= 0.3f;
                            human.SetHubRotation(Quaternion.Euler(currentEuler));
                        }
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
