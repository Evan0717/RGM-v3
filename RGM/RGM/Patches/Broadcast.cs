using Exiled.API.Features;
using MultiBroadcast.API;

namespace RGM.Patches
{
    public class BroadcastPostfix
    {
        public static void Postfix(ushort duration, string message, Broadcast.BroadcastFlags type, bool shouldClearPrevious)
        {
            foreach (var player in Player.List)
            {
                player.AddBroadcast(duration, message);
            }
        }
    }
}
