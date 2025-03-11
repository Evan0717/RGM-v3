using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.API.Interfaces
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public Action<Player, string> Script { get; set; }
        public Func<Player, bool> Check { get; set; } = (player) => true;
    }
}
