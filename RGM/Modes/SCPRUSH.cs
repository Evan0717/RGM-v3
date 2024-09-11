using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles;

namespace RGM.Modes
{
    public class SCPRUSH
    {
        public static SCPRUSH Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            List<RoleTypeId> ScpRoles = new List<RoleTypeId>
            {
                RoleTypeId.Scp049,
                RoleTypeId.Scp096,
                RoleTypeId.Scp106,
                RoleTypeId.Scp173,
                RoleTypeId.Scp939,
                RoleTypeId.Scp079,
                RoleTypeId.Scp3114,
            };
            RoleTypeId RandomScpRole = ScpRoles[UnityEngine.Random.Range(0, ScpRoles.Count)];

            foreach (var player in Player.List.Where(x => x.IsScp))
            {
                player.Role.Set(RandomScpRole);
            }

            yield break;
        }
    }
}
