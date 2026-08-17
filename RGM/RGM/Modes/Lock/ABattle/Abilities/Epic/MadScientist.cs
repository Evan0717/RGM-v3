using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Warhead;
using MEC;
using PlayerRoles;
using RGM.API.DataBases;

namespace RGM.Modes.Abilities.Epic;

[Ability("매드 사이언티스트", "사망 시, 10초 뒤에 해당 자리에서 리셋됩니다. 랜덤한 능력 5개가 지급됩니다.", AbilityCategory.Epic,
    AbilityType.EPIC_MADSCIENTIST)]
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

    private void OnDied(DiedEventArgs ev)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (ev.Player != Owner ||
                !ev.Player.IsDead ||
                Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type) || 
                Warhead.IsDetonated)
                return;

            Timing.CallDelayed(10, () =>
            {
                if (ev.Player.IsDead) Owner.Role.Set(ev.TargetOldRole, RoleSpawnFlags.None);

                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            Owner.AddAbility(ABattle.Instance.GetRandomAbilities(ev.Player,
                                ABattle.Instance.GetCategory(Owner, allowAncient: false), 1)[0]);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Failed to add ability to Mad Scientist: {ex}");
                        }
                    }
                });
            });
        });
    }
}