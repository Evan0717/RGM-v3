using Exiled.API.Features;

using RGM.API.Features;

namespace RGM.Patches
{
    public class BroadcastPatch
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
