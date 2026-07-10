using System.Linq;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Scp079;
using RGM.API.Features;
using RGM.Modes.SubClass;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("핑 갈고리", "다음 핑의 위치에 랜덤한 플레이어를 소환시킵니다.", AbilityCategory.Scp079, AbilityType.SCP079_PINGHOOK)]
public class PingHook : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        OnDisabled();

        Vector3 pos = ev.Position;

        PlayerManager.List.Where(x => x.IsAlive && !NonePlayer.Players.Contains(x)).GetRandomValue().Position = new Vector3(pos.x, pos.y + 2, pos.z);
    }
}
