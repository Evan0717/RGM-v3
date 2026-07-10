using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("수리 기사", "모든 잠겨진 문에 액세스할 수 있으며, 테슬라를 작동시키지 않습니다.", AbilityCategory.Epic, AbilityType.EPIC_REPAIRMAN)]
public class RepairMan : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
        Exiled.Events.Handlers.Player.InteractingLocker += OnInteractingLocker;
        Exiled.Events.Handlers.Player.OpeningGenerator += OnOpeningGenerator;
        Exiled.Events.Handlers.Player.ClosingGenerator += OnClosingGenerator;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
        Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
        Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractingLocker;
        Exiled.Events.Handlers.Player.OpeningGenerator -= OnOpeningGenerator;
        Exiled.Events.Handlers.Player.ClosingGenerator -= OnClosingGenerator;
    }

    public void OnInteractingDoor(InteractingDoorEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.Door.Type == DoorType.Scp079First)
        {
            ev.Player.AddHint("헤비도어", "이 헤비도어는 능력으로 개폐가 불가능합니다.", 1.2f);
            return;
        }

        if (Warhead.IsInProgress)
        {
            ev.Player.AddHint("알파 핵탄투", "알파 핵탄투가 작동 중일때는 문을 개폐할 수 없습니다.", 1.2f);
            return;
        }

        ev.IsAllowed = false;

        if (ev.Door.IsOpen)
            ev.Door.IsOpen = false;

        else
            ev.Door.IsOpen = true;
    }

    public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ev.IsTriggerable = false;
    }

    public void OnInteractingLocker(InteractingLockerEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ev.IsAllowed = false;

        if (ev.InteractingChamber.IsOpen)
            ev.InteractingChamber.IsOpen = false;

        else
            ev.InteractingChamber.IsOpen = true;
    }

    public void OnOpeningGenerator(OpeningGeneratorEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ev.IsAllowed = false;
        ev.Generator.IsOpen = false;
    }

    public void OnClosingGenerator(ClosingGeneratorEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ev.IsAllowed = false;
        ev.Generator.IsOpen = true;
    }
}
