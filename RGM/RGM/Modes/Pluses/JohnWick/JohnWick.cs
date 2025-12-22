using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.JohnWick)]
    public class JohnWick : Mode
    {
        public override string Name => "존 윅";
        public override string Description => "권총류 무기의 데미지가 400% 상승합니다.";
        public override string Detail =>
"""
COM-15
COM-18
COM-45
.44 리볼버
""";
        public override string Color => "2EFEF7";

        public static JohnWick Instance;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                List<ItemType> Pistols = new List<ItemType>()
                {
                    ItemType.GunCOM15,
                    ItemType.GunCOM18,
                    ItemType.GunCom45,
                    ItemType.GunRevolver
                };

                if (Pistols.Contains(ev.Attacker.CurrentItem.Type))
                {
                    ev.DamageHandler.Damage = 4 * ev.DamageHandler.Damage;
                }
            }
        }
    }
}
