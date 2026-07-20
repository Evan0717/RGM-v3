using Exiled.Events.EventArgs.Scp049;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp049;

[Ability("유능한 의사", "소생된 좀비의 체력이 50% 추가됩니다.", AbilityCategory.Common, AbilityType.COMMON_SCP049_COMPETENTDOCTOR, RoleAbility.Scp049)]
public class CompetentDoctor : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp049.FinishingRecall += OnFinishingRecall;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp049.FinishingRecall -= OnFinishingRecall;
    }

    public void OnFinishingRecall(FinishingRecallEventArgs ev)
    {
        if (ev.Player == Owner)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Target.MaxHealth *= 1.5f;
                ev.Target.Health = ev.Target.MaxHealth;
            });
        }
    }
}
