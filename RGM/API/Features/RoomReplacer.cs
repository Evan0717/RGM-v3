using System.Linq;
using Exiled.API.Features;
using UnityEngine;

namespace RGM.API.Features;

public static class RoomReplacer
{
    public static void DestroyRoom(Room room)
    {
        foreach (var component in room.gameObject.GetComponentsInChildren<Component>())
            try
            {
                if (component.name.Contains("SCP-079") || component.name.Contains("CCTV"))
                {
                    Log.Debug(
                        $"Prevent from destroying: {component.name} {component.tag} {component.GetType().FullName}");
                    continue;
                }

                if (component.GetComponentsInParent<Component>()
                    .Any(c => c.name.Contains("SCP-079") || c.name.Contains("CCTV")))
                {
                    Log.Debug(
                        $"Prevent from destroying: {component.name} {component.tag} {component.GetType().FullName}");
                    continue;
                }

                Log.Debug($"Destroying component: {component.name} {component.tag} {component.GetType().FullName}");

                Object.Destroy(component);
            }
            catch
            {
            }
    }
}