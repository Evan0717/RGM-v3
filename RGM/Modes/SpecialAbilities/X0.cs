using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;

namespace RGM.Modes.SpecialAbilities
{
    public class X0 // 눈빛맨
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Timing.RunCoroutine(OnStarted());
        }

        public void OnDisabled()
        {
        }

        public IEnumerator<float> OnStarted()
        {
            while (target.IsAlive)
            {
                if (Physics.Raycast(target.ReferenceHub.PlayerCameraReference.position + target.ReferenceHub.PlayerCameraReference.forward * 0.2f, target.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 1000f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                    Server.ExecuteCommand($"/rocket {player.Id} 1");
                }

                yield return Timing.WaitForSeconds(1f);
            }

            OnDisabled();
        }
    }
}
