using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;
using static RGM.Variables.Variable;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("베테랑", "최초 1회에 한하여 사망 판정을 받을 경우 한번 버티며, 1.5초간 무적 효과를 얻습니다.", RankAbilityType.베테랑, RankCategory.구미호, RankAbilityCategory.변칙성, "🔋")]
    public class 베테랑 : RankAbility
    {
        int count = 1;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Dying -= OnDying;
        }

        void OnDying(DyingEventArgs ev)
        {
            if (ev.Player != Owner) return;
            if (count <= 0) return;
            ev.IsAllowed = false;
            if (!GodModePlayers.Contains(Owner)) GodModePlayers.Add(Owner);
            Timing.CallDelayed(1.5f, () => {
                if (GodModePlayers.Contains(Owner)) GodModePlayers.Remove(Owner);
            });
            count--;
        }
    }
}
