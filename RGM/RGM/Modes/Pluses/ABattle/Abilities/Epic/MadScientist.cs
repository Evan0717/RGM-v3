using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Epic;

[Ability("매드 사이언티스트", "사망 시, 10초 뒤에 해당 자리에서 리셋됩니다. 랜덤한 능력 5개가 지급됩니다.", AbilityCategory.Epic, AbilityType.EPIC_MADSCIENTIST)]
public class MadScientist : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    public void OnDied(DiedEventArgs ev)
    {
        if (ev.Player != Owner || Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
            return;

        Timing.CallDelayed(10, () =>
        {
            Owner.Role.Set(ev.TargetOldRole, RoleSpawnFlags.None);

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        Owner.AddAbility(ABattle.Instance.GetRandomAbilities(ABattle.Instance.GetCategory(Owner), 1)[0]);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to add ability to Mad Scientist: {ex}");
                    }
                }
            });
        });
    }
}
