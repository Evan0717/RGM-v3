using Exiled.API.Features;
using LabApi.Events.Arguments.PlayerEvents;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.ToolGun;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Mythic;

[Ability("툴건", "아무 기능도 없는 권총을 얻습니다. 사용을 시도할 때마다 20%의 체력을 소실합니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_TOOLGUN)]
public class ToolGun : Ability
{
    public override void OnEnabled()
    {
        if (ToolGunItem.TryAdd(Owner))
        {
        }

        LabApi.Events.Handlers.PlayerEvents.DryFiringWeapon += OnPlayerDryFiringWeapon;
    }

    public override void OnDisabled()
    {
    }

    public void OnPlayerDryFiringWeapon(PlayerDryFiringWeaponEventArgs ev)
    {
        if (ev.FirearmItem.IsToolGun(out ToolGunItem toolGun))
        {
            Player player = (Player)ev.Player;

            player.Hit(player, player.MaxHealth / 5);
        }
    }
}