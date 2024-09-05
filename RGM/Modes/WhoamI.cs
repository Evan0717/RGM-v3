using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API;

namespace RGM.Modes
{
    public class WhoamI
    {
        public static WhoamI Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                List<RoleTypeId> Roles = Tools.EnumToList<RoleTypeId>();

                foreach (var player in Player.List)
                {
                    RoleTypeId SelectedRole = Roles[UnityEngine.Random.Range(0, Roles.Count())];
                    player.Role.Set(SelectedRole, Exiled.API.Enums.SpawnReason.ForceClass, RoleSpawnFlags.None);
                }

                yield return Timing.WaitForSeconds(60);
            }
        }
    }
}
