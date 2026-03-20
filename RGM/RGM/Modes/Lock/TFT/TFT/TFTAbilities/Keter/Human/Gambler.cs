using CustomPlayerEffects;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.Human;

[TFTAbility("도박꾼", "능력 키(ALT)를 누르면 들고 있는 아이템이 랜덤하게 변경됩니다. (10% 확률로 손이 잘립니다.)", TFTAbilityLevel.Keter, TFTAbilityCategory.Human, TFTAbilityPoint.ALT, TFTAbilityType.Gambler, "🎰")]
public class Gambler : TFTAbility
{
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
        if (Owner == ev.Player && ev.Player.CurrentItem != null)
        {
            try
            {
                ev.Player.RemoveHeldItem();

                if (Random.Range(0, 100) < 10)
                    Owner.EnableEffect(EffectType.SeveredHands, 1, 50);

                else
                {
                    Owner.AddItem(Function.EnumToList<ItemType>().GetRandomValue());
                }
            }
            catch { }
        }
    }
}
