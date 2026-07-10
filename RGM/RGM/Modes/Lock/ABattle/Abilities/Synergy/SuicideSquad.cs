using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.RARE_BOMBERMAN, AbilityType.RARE_MARTYRDOM, AbilityType.EPIC_SUICIDEBOMBER)]
[Ability("수어사이드 스쿼드", "<봄버맨, 순교, 수어사이드 봄버맨> 사망할 경우 SCP-018를 3개 떨어트립니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_SUICIDESQUAD)]
public class SuicideSquad : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        Vector3 pos = Owner.Position;

        Timing.CallDelayed(0.1f, () =>
        {
            if (Owner.IsDead)
            {
                for (int i = 0; i < 3; i++)
                {
                    Pickup item = Pickup.Create(ItemType.SCP018);
                    item.Spawn(pos, Quaternion.identity);
                }
            }
        });
    }
}
