using Exiled.Events.EventArgs.Scp049;
using MEC;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("걸작", "다음에 살리는 SCP-049-2의 체력이 50% 추가됩니다.", RankAbilityType.걸작, RankCategory.SCP_049, "💉", 90)]
    public class 걸작 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            Exiled.Events.Handlers.Scp049.FinishingRecall += OnFinishingRecall;

            void OnFinishingRecall(FinishingRecallEventArgs ev)
            {
                if (ev.Player == Owner)
                {
                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        ev.Target.MaxHealth *= 1.5f;
                        ev.Target.Health = ev.Target.MaxHealth;
                    });
                }

                Exiled.Events.Handlers.Scp049.FinishingRecall -= OnFinishingRecall;
            }
        }
    }
}
