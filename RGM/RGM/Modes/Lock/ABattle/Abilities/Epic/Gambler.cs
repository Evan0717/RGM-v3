using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("도박꾼", "아이템을 버리면 새로운 아이템을 받지만, 2% 확률로 손이 잘립니다.", AbilityCategory.Epic, AbilityType.EPIC_GAMBLER)]
public class Gambler : Ability
{
    public override void OnEnabled()
    {
        if (Owner.IsScpRole() || Owner.Role.Type.ToString().Contains("Flamingo"))
            Owner.AddHint("도박", $"<size=20>[Space + ALT]ㅣ도박을 진행할 수 있습니다.</size>", 10);

        Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
        Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
        Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
    }

    public void OnDroppingItem(DroppingItemEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        int rand = UnityEngine.Random.Range(1, 101);
        if (0 < rand && rand < 3)
        {
            Owner.EnableEffect(EffectType.SeveredHands, 1, 50);
        }
        else
        {
            ev.Item.Destroy();
            Item CurrentItem = Owner.AddRandomItem();
            Owner.DropItem(CurrentItem);
        }
    }

    public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        if (!(Owner.IsScpRole() || Owner.Role.Type.ToString().Contains("Flamingo")) || !Owner.IsJumping || Owner.GetEffect(EffectType.SeveredHands).IsEnabled)
            return;

        int rand = UnityEngine.Random.Range(1, 101);

        if (0 < rand && rand < 3)
            Owner.EnableEffect(EffectType.SeveredHands, 1, 50);

        else
        {
            if (Owner.IsScpRole())
                Owner.Hit(Owner, Owner.MaxHealth / 100);

            Owner.AddRandomItem();
        }
    }
}
