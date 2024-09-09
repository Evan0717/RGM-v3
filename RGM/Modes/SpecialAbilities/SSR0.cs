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
    public class SSR0 // 마술사
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Exiled.Events.Handlers.Player.Dying += OnDying;

            Timing.RunCoroutine(OnStarted());
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Dying -= OnDying;
        }

        public IEnumerator<float> OnStarted()
        {
            while (target.IsAlive)
                yield return Timing.WaitForSeconds(1f);

            OnDisabled();
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (ev.Player != null && ev.Player == target)
            {
                ev.Attacker.Kill("저런, 몸이 교체되는 마술에 당했네요!");
                ev.Player.Role.Set(ev.Attacker.Role);
                ev.Player.MaxHealth = ev.Attacker.MaxHealth;
                ev.Player.Health = ev.Attacker.Health;

                foreach (var Item in ev.Attacker.Items)
                {
                    ev.Player.AddItem(Item);
                }

                ev.Attacker.ShowHint("<u>짜잔, 마술이였습니다!</u>", 5);
                ev.Player.ShowHint("<u>짜잔, 마술이였습니다!</u>", 5);
            }
        }
    }
}
