using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.ClassD;

[Ability("절도죄", "[ALT]를 눌러 상대의 아이템 중 하나를 빼앗을 수 있습니다. (쿨타임 1분)", AbilityCategory.ClassD, AbilityType.CLASSD_LARCENY)]
public class Larceny : Ability
{
    int PickPocketCooldown = 0;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
    }

    public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        if (ev.Player != Owner || PickPocketCooldown > 0)
            return;

        if (Tools.TryGetLookPlayer(ev.Player, 2f, out Player player, out RaycastHit? hit))
        {
            PickPocketCooldown = 60;

            if (!player.IsInventoryEmpty)
            {
                Item Item = Tools.GetRandomValue(player.Items.ToList());

                player.RemoveItem(Item);
                player.AddHint("소매치기", "주머니가 허전합니다..", 1.2f);
                Item I = ev.Player.AddItem(Item.Type);
                ev.Player.AddHint("소매치기", "소매치기에 성공했습니다.", 1.2f);

                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.7f);
            }
            else
            {
                player.AddHint("소매치기", "누군가가 소매치기를 하려고 시도했습니다.", 1.2f);
                ev.Player.AddHint("소매치기", "소매치기에 실패했습니다.\n대상은 아이템을 가지고 있지 않습니다.", 1.2f);
            }

            Timing.CallDelayed(60, () =>
            {
                PickPocketCooldown = 0;
            });
        }
    }
}
