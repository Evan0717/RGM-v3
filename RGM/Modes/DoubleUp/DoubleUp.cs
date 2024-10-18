using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using MultiBroadcast;
using MultiBroadcast.API;
using RGM.API.Features;

namespace RGM.Modes
{
    class DoubleUp
    {
        public static DoubleUp Instance;

        public static List<string> BlackListMods = Tools.GetMiniGamesList();
        public static Dictionary<string, List<string>> Mods = RGM.Instance.ModeList;

        public static List<string> ModeKeys = RGM.Instance.ModeList.Keys.Where(x => Mods[x][3] != "private" && !BlackListMods.Contains(x)).ToList();
        public static string mod1 = Tools.GetRandomValue(ModeKeys);
        public static string mod2 = Tools.GetRandomValue(ModeKeys.Where(x => x != mod1).ToList());

        public List<string> Modes = new List<string>() { mod1, mod2 };

        public List<string> pl = new List<string>();

        public static string Description = $"<size=25><b>[<color=#{Mods[mod1][0]}>{mod1}</color> + <color=#{Mods[mod2][0]}>{mod2}</color>]</b></size>";

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            for (int i=0; i<2; i++)
                Tools.TryInstallMode(Mods[Modes[i]][2]);

            foreach (var player in Player.List.Where(x => !x.IsNPC))
            {
                player.AddBroadcast(10, Description);
                player.SendConsoleMessage($"\n{Description}", "white");
            }
        }

        public void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            ev.Player.AddBroadcast(10, Description);
            ev.Player.SendConsoleMessage($"\n{Description}", "white");
        }
    }
}
