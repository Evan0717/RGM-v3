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
using Achievements.Handlers;
using Exiled.API.Enums;

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
                yield return Timing.WaitForSeconds(60f);

                List<RoleTypeId> BlackList = new List<RoleTypeId>()
                {
                    RoleTypeId.Filmmaker,
                    RoleTypeId.Spectator,
                    RoleTypeId.Overwatch,
                    RoleTypeId.Scp079
                };

                List<RoleTypeId> Roles = Tools.EnumToList<RoleTypeId>().Where(role => !BlackList.Contains(role)).ToList();

                foreach (var player in Player.List.Where(x => !BlackList.Contains(x.Role.Type)))
                {
                    RoleTypeId SelectedRole = Tools.GetRandomValue(Roles);
                    player.Role.Set(SelectedRole, SpawnReason.ForceClass, RoleSpawnFlags.None);
                }
            }
        }
    }
}
