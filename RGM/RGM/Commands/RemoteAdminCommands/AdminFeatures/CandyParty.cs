using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CandyParty : ICommand, IUsageProvider
    {
        public static void Create(CandyKindID candy, float size, Vector3 pos)
        {
            var scp330Pickup = (Exiled.API.Features.Pickups.Scp330Pickup)Exiled.API.Features.Pickups.Scp330Pickup.CreateAndSpawn(ItemType.SCP330, pos);
            scp330Pickup.Candies.Add(candy);
            scp330Pickup.ExposedCandy = scp330Pickup.Candies.First();

            NetworkServer.UnSpawn(scp330Pickup.GameObject);
            scp330Pickup.GameObject.transform.localScale *= size;
            NetworkServer.Spawn(scp330Pickup.GameObject);
        }
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));
            string candyArg = arguments.At(1);
            bool isRandom = candyArg.Equals("random", StringComparison.OrdinalIgnoreCase);
            CandyKindID candyKindID = CandyKindID.None;
            if (!isRandom)
                candyKindID = (CandyKindID)Enum.Parse(typeof(CandyKindID), candyArg, true);
            int count = int.Parse(arguments.At(2));
            float size = float.Parse(arguments.At(3));

            CandyKindID[] allCandyKinds = Enum.GetValues(typeof(CandyKindID))
                .Cast<CandyKindID>()
                .Where(x => x != CandyKindID.None)
                .ToArray();
            System.Random rng = new System.Random();

            IEnumerator<float> candyParty()
            {
                for (int i = 0; i < count; i++)
                {
                    CandyKindID currentCandy = isRandom
                        ? allCandyKinds[rng.Next(allCandyKinds.Length)]
                        : candyKindID;

                    Create(currentCandy, size, player.Position);
                }

                yield break;
            }

            Timing.RunCoroutine(candyParty());

            response = $"This is party for {player.DisplayNickname}!!";
            return true;
        }

        public string Command { get; } = "candyparty";

        public string[] Aliases { get; } = { "cp" };

        public string Description { get; } = "[RGM] 원하는 캔디를 소환합니다.";

        public bool SanitizeResponse { get; } = true;
        public string[] Usage { get; } = { "%player%", "CandyKindID", "Count", "Size" };
    }
}
