using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace RGM.Modes.Abilities.Legend;

[Ability("킬스트릭", "적을 처치할 때마다 새로운 능력을 얻습니다. (인간 진영의 경우 능력 5개를 추가로 선택합니다.)", AbilityCategory.Legend, AbilityType.LEGEND_KILLSTREAK)]
public class KillStreak : Ability
{
    CoroutineHandle coroutine;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnDied;

        coroutine = Timing.RunCoroutine(onStarted());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;

        Timing.KillCoroutines(coroutine);
    }

    public IEnumerator<float> onStarted()
    {
        for (int i = 1; i < 6; i++)
        {
            ABattle.Instance.StartSelect(Owner);

            while (ABattle.Instance.IsSelecting[Owner])
                yield return Timing.WaitForOneFrame;
        }
    }

    public void OnDied(DiedEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        ABattle.Instance.StartSelect(ev.Attacker);
    }
}
