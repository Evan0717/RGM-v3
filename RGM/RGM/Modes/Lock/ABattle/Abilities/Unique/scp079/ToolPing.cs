using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Patches.Events.Player;
using MEC;
using ProjectMER.Features.Serializable;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Unique.Scp079;


[Ability("따아알깍", "핑을 찍으면 워크스테이션을 설치합니다. (쿨타임 3초)", AbilityCategory.Mythic, AbilityType.MYTHIC_SCP079_TOOLPING, RoleAbility.Scp079)]
public class ToolGun : Ability
{
    bool isScp079Cooldown = false;
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {

    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player == null || ev.Player != Owner) return;
        if (!isScp079Cooldown)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                new SerializableWorkstation
                {
                    IsInteractable = true,
                    Position = ev.Position,
                    Rotation = new Vector3(0, 0, 0),
                    Scale = Vector3.one
                }.SpawnOrUpdateObject();
                isScp079Cooldown = true;

                Timing.CallDelayed(3f, () =>
                {
                    isScp079Cooldown = false;
                });
            });
        }
    }
}