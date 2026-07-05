using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Modules;
using RGM.API.Features;
using RGM.Patches;

namespace RGM.Modes.Abilities.Epic;

[Ability("Springfield M1A", "반자동 소총을 얻습니다.", AbilityCategory.Epic, AbilityType.EPIC_SPRINGFIELDM1A)]
public class SpringfieldM1A : Ability
{
    private ushort _m1ASerial;

    // 직전에 발사를 허용했던 "트리거 누름 시각". 같은 누름(풀오토 유지)에서는 이 값이 변하지 않는다.
    private double _lastHandledTriggerPress = double.NegativeInfinity;

    public override void OnEnabled()
    {
        // 서버에서도 트리거 누름/뗌 상태를 신뢰할 수 있도록 한 번만 패치를 적용한다.
        FirearmTriggerStatePatch.Ensure();

        Item item = Owner.AddItem(ItemType.GunE11SR);

        _m1ASerial = item.Serial;
        ApplySpringfieldSettings(item.As<Firearm>());

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Player != Owner || ev.Item == null || ev.Item.Serial != _m1ASerial)
            return;

        ev.Player.AddHint("Springfield M1A", $"<b><color={ABattle.RatingColor["영웅"]}>Springfield M1A</color></b> 능력이 있는 E11SR 입니다");
    }

    public void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Player != Owner || ev.Item.Serial != _m1ASerial)
            return;

        Firearm firearm = ev.Item.As<Firearm>();
        ApplySpringfieldSettings(firearm);

        // 반자동 매커니즘: 한 번의 트리거(마우스) 입력마다 첫 발만 허용한다.
        // 풀오토는 트리거를 누른 채로 유지하는 동안 같은 LastTriggerPress 값으로 여러 번의 Shooting 이벤트를 만든다.
        // 따라서 직전에 허용한 누름보다 더 최신의 누름이 들어왔을 때(= 손을 뗐다가 다시 누름)만 발사를 허용하고,
        // 같은 누름에서 이어지는 연사는 모두 취소한다. 시간 간격이 아닌 입력 엣지로 판별하므로 네트워크 지터에 영향을 받지 않는다.
        if (!IsFreshTriggerPull(firearm))
            ev.IsAllowed = false;
    }

    private bool IsFreshTriggerPull(Firearm firearm)
    {
        if (firearm?.Base == null)
            return true;

        ITriggerControllerModule trigger = null;

        foreach (ModuleBase module in firearm.Base.Modules)
        {
            if (module is ITriggerControllerModule controller)
            {
                trigger = controller;
                break;
            }
        }

        // 트리거 컨트롤러를 찾지 못하면 판별할 수 없으므로 정상 발사로 둔다.
        if (trigger == null)
            return true;

        double press = trigger.LastTriggerPress;

        // 직전에 허용한 누름보다 더 최신의 누름 → 새 트리거 입력 → 첫 발만 허용
        if (press > _lastHandledTriggerPress)
        {
            _lastHandledTriggerPress = press;
            return true;
        }

        return false;
    }

    private static void ApplySpringfieldSettings(Firearm firearm)
    {
        if (firearm == null)
            return;

        firearm.DamageFalloffDistance = 500f;
        firearm.Damage = 48.7f;
    }
}
