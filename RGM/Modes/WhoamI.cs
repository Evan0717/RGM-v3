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
            yield return Timing.WaitForSeconds(10f);

            List<RoleTypeId> BlackList = new List<RoleTypeId>() 
            { 
                RoleTypeId.Filmmaker, 
                RoleTypeId.Spectator, 
                RoleTypeId.Overwatch
            };

            List<RoleTypeId> Roles = Tools.EnumToList<RoleTypeId>().Where(role => !BlackList.Contains(role)).ToList();

            foreach (var player in Player.List.Where(x => !BlackList.Contains(x.Role.Type)))
            {
                RoleTypeId SelectedRole = Roles[UnityEngine.Random.Range(1, Roles.Count())];
                player.Role.Set(SelectedRole, Exiled.API.Enums.SpawnReason.ForceClass, RoleSpawnFlags.None);
            }
        }
    }
}
