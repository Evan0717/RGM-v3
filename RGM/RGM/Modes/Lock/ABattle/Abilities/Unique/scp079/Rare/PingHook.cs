using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp079;
using RGM.API.Features;
using RGM.Modes.SubClass;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("핑 갈고리", "다음 핑의 위치에 SCP가 아닌 랜덤한 플레이어 1명을 소환시킵니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_PINGHOOK, RoleAbility.Scp079)]
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

    private void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        OnDisabled();

        Vector3 pos = ev.Position;
        Player RandomPlayer1 = PlayerManager.List.Where(x => x.IsAlive && !NonePlayer.Players.Contains(x) && !x.IsScpRole()).GetRandomValue();
        RandomPlayer1.Position = new Vector3(pos.x, pos.y + 2, pos.z);
        /*Player RandomPlayer2 = PlayerManager.List.Where(x => x.IsAlive && !NonePlayer.Players.Contains(x) && !x.IsScpRole() && x != RandomPlayer1).GetRandomValue();
        RandomPlayer2.Position = new Vector3(pos.x, pos.y + 2, pos.z);*/
    }
}
