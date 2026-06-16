using MEC;

namespace RGM.Modes.Abilities.Unique.Scp049;

[Ability("능수능란", "좀비 소생 시간이 50% 줄어듭니다.", AbilityCategory.Scp049, AbilityType.SCP049_PROFICIENCY)]
public class Proficiency : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp049.StartingRecall += OnStartingRecall;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp049.StartingRecall -= OnStartingRecall;
    }

    public void OnStartingRecall(Exiled.Events.EventArgs.Scp049.StartingRecallEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        Timing.CallDelayed(0.1f, () =>
        {
            ev.Scp049.RemainingCallDuration *= 0.5f;
        });
    }
}
