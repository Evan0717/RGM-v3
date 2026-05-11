using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using UnityEngine;

using Exiled.Events.EventArgs.Scp079;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Scp1507;

using static RGM.Variables.Variable;
using Exiled.API.Enums;

namespace RGM.Modes;

public class StoreEventHandler(Store Store)
{
    public static StoreEventHandler Instance;

    internal void RegisterEvents()
    {
        Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        Exiled.Events.Handlers.Player.Died += OnDied;

        Exiled.Events.Handlers.Scp1507.SpawningFlamingos += OnSpawningFlamingos;
    }

    internal void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        Exiled.Events.Handlers.Player.Died -= OnDied;

        Exiled.Events.Handlers.Scp1507.SpawningFlamingos -= OnSpawningFlamingos;
    }

    private IEnumerator<float> OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (ev.Player.IsDead || ev.NewRole.IsDead() || ev.Player.GetStoreItems().Count() == 0)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () => 
            {
                Store.Reset(ev.Player);
            });
        }
        else
        {
            if (ev.Reason == SpawnReason.Escaped)
            {
                Timing.RunCoroutine(Store.RestoreStoreItems(new List<Player>() { ev.Player }));
            }
        }

        yield break;
    }

    private void OnDied(DiedEventArgs ev)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            Store.Reset(ev.Player);
        });
    }


    private void OnSpawningFlamingos(SpawningFlamingosEventArgs ev)
    {
        Timing.RunCoroutine(Store.RestoreStoreItems(ev.SpawnablePlayers.ToList()));
    }
}