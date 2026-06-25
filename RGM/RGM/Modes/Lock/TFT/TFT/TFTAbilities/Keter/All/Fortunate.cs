using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("행운아", "모든 문들을 5% 확률로 열고 닫을 수 있습니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Continuous, TFTAbilityType.Fortunate, "🍀")]
public class Fortunate : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnActivatingWarheadPanel;
        Exiled.Events.Handlers.Player.UnlockingGenerator += OnUnlockingGenerator;
        Exiled.Events.Handlers.Player.InteractingLocker += OnInteractingLocker;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
        Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
        Exiled.Events.Handlers.Player.UnlockingGenerator -= OnUnlockingGenerator;
        Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractingLocker;
    }

    void OnInteractingDoor(InteractingDoorEventArgs ev)
    {
        if (ev.Player == Owner)
        {
            if (ev.Door.Type == DoorType.Scp079First)
            {
                return;
            }

            if (Warhead.IsInProgress)
            {
                return;
            }

            if (Random.Range(1, 21) == 1)
            {
                ev.IsAllowed = true;
            }
        }
    }

    void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
    {
        if (ev.Player == Owner)
        {
            if (Random.Range(1, 21) == 1)
            {
                ev.IsAllowed = true;
            }
        }
    }

    void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
    {
        if (ev.Player == Owner)
        {
            if (Random.Range(1, 21) == 1)
            {
                ev.IsAllowed = true;
            }
        }
    }

    void OnInteractingLocker(InteractingLockerEventArgs ev)
    {
        if (ev.Player == Owner)
        {
            if (Random.Range(1, 21) == 1)
            {
                ev.IsAllowed = true;
            }
        }
    }
}
