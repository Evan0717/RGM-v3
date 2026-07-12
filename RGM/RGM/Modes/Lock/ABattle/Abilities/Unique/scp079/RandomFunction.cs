using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp079;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("랜덤 함수", "정전 시, 랜덤한 방 5개를 추가로 정전합니다.", AbilityCategory.Scp079, AbilityType.SCP079_RANDOMFUNCTION)]
public class RandomFunction : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.RoomBlackout += OnRoomBlackout;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.RoomBlackout -= OnRoomBlackout;
    }

    public void OnRoomBlackout(RoomBlackoutEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        for (int i = 1; i < 6; i++)
        {
            Room SelectedRoom = Tools.GetRandomValue(Room.List.ToList());

            SelectedRoom.TurnOffLights(10);
        }
    }
}
