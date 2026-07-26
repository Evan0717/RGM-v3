using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using HarmonyLib;

namespace RGM.Patches;

public static partial class UserSettingsPatch
{
     [Obsolete("현재 제작중, 완성되지 않았으므로 사용되지 않습니다.")]
     public static partial void Apply(Harmony instance);
     
     public static partial void Apply(Harmony instance)
     {
          // TODO: 사용되지 않는 로직
          /*
          instance.Patch(TargetMethod("ProximityChat").GetMethod,
               new HarmonyMethod(AccessTools.Method(typeof(UserSettingsPatch), nameof(HeaderSettingsPrefix))));

          instance.Patch(TargetMethod("Personalization").GetMethod,
               new HarmonyMethod(AccessTools.Method(typeof(UserSettingsPatch), nameof(PersonalizationSettingsPrefix))));
          */
     }
     
     private static PropertyInfo TargetMethod(string name)
     {
          try
          {
               Assembly targetAssembly = Assembly.Load("ScpProximityChat.SecretAPI");
               Type internalClassType = targetAssembly.GetType("ScpProximityChat.SecretAPI.Settings.Headers");

               return AccessTools.Property(internalClassType, name);
          }
          catch (Exception e)
          {
               Log.Error("Cannot to load the TargetMethod \"ScpProximityChat\". Aborting.");
               throw new Exception(e.StackTrace);
          }
     }
     
     public static void HeaderSettingsPrefix(string __result)
     {
          __result = "SCP 근접 채팅";
     }

     public static void PersonalizationSettingsPrefix(string __result)
     {
          __result = "근접 채팅 볼륨 개인화";
     }
     
}
