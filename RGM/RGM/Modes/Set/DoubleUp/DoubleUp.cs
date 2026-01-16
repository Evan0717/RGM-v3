using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using MultiBroadcast;

using RGM.API.DataBases;
using RGM.API.Features;

using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.DoubleUp)]
    class DoubleUp : Mode
    {
        public override string Name => "더블업";
        public override string Description => "모드 2개가 동시에 적용됩니다. <size=20>(Set은 최대 하나까지만 적용됩니다.)</size>";
        public override string Detail =>
"""
<b>Set은 Plus와 상반되는 모드 종류입니다.</b>

Set - 기존 SCP:SL의 게임 방식의 궤가 크게 달라짐. (ex. 나는 누구?, 스플리프)
Plus - 기존 SCP:SL의 게임 방식에 새로운 요소를 더함. (ex. 도박, 존 윅)
Plus(Sub) - 서브로만 등장하는 모드입니다. (ex. 한국인이 좋아하는 속도, 시베리아)

<i>* 서브 모드는 하나만 적용될 경우 게임의 재미가 떨어질 수도 있는 모드 집합입니다.</i>

모드 조합은 다음과 같이 등장할 수 있습니다.
- [Set, Plus]
- [Plus, Plus]
- [Set, Plus(Sub)]
- [Plus, Plus(Sub)]
- [Plus(Sub), Plus(Sub)]
""";
        public override string Color => "5882FA";

        public static DoubleUp Instance;

        static Dictionary<ModeType, ModeData> Mods = ModeList;

        static List<ModeType> ModeKeys = ModeList.Keys.Where(x => Mods[x].Category == ModeCategory.Public && Mods[x].Info != ModeInfo.Lock).ToList();
        static ModeType mod1 = Tools.GetRandomValue(ModeKeys);
        static ModeType mod2 = Tools.GetRandomValue(ModeKeys.Where(x => (mod1.GetModeData().Info == ModeInfo.Set ? x != mod1 : true) && ModeList.Keys.Where(x => x.GetModeData().Info != ModeInfo.Set).Contains(x)).ToList());

        List<ModeType> Modes = new List<ModeType>() { mod1, mod2 };

        static string Desc = $"<size=25><b>[<color=#{Mods[mod1].Color}>{mod1.GetModeData().Name}</color> + <color=#{Mods[mod2].Color}>{mod2.GetModeData().Name}</color>]</b></size>";

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            for (int i = 0; i < 2; i++)
                Tools.TryInstallMode(Mods[Modes[i]].Type);

            foreach (var player in PlayerManager.List.Where(x => !x.IsNPC))
            {
                player.AddBroadcast(10, Desc);
                player.SendConsoleMessage($"\n{Desc}", "white");
            }

            yield break;
        }

        public void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            ev.Player.AddBroadcast(10, Desc);
            ev.Player.SendConsoleMessage($"\n{Desc}", "white");
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }
    }
}
