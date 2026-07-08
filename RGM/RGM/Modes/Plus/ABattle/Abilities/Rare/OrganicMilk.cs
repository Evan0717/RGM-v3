using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Rare;

[Ability("유기농 우유", "자신에게 해로운 효과만 제거합니다.", AbilityCategory.Rare, AbilityType.RARE_ORGANICMILK)]
public class OrganicMilk : Ability
{
    private static readonly HashSet<EffectType> KeptBuffs =
    [
        EffectType.Scp1853,
        EffectType.Invigorated,
        EffectType.Invisible,
        EffectType.RainbowTaste,
        EffectType.BodyshotReduction,
        EffectType.DamageReduction,
        EffectType.MovementBoost,
        EffectType.Vitality,
        EffectType.SpawnProtected,
        EffectType.Ghostly,
        EffectType.SilentWalk,
        EffectType.Fade,
        EffectType.FocusedVision,
        EffectType.AnomalousRegeneration,
        EffectType.Scp1344,
        EffectType.Scp207,
        EffectType.AntiScp207,
        EffectType.Lightweight,
        EffectType.NightVision,
        EffectType.FogControl,
        EffectType.PitDeath
    ];
    ushort CoinSerial = 0;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.Coin);
        CoinSerial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item != null)
        {
            if (CoinSerial == ev.Item.Serial)
                ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["희귀"]}>유기농 우유</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (CoinSerial == ev.Item.Serial && ev.Player.CurrentRoom.Type != RoomType.Pocket)
        {
            foreach (var effectType in Owner.ActiveEffects
                         .Select(effect => effect.GetEffectType())
                         .Where(effectType => !KeptBuffs.Contains(effectType))
                         .ToList())
            {
                Owner.DisableEffect(effectType);
                ev.Player.DisableEffect(effectType);
            }

            ev.Item.Destroy();
        }
    }
}
