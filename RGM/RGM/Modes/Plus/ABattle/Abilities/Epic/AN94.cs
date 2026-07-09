using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Modules;
using MEC;
using RGM.API.Features;
using RGM.Patches;

namespace RGM.Modes.Abilities.Epic;

[Ability("AN-94", "2점사 소총을 얻습니다.", AbilityCategory.Epic, AbilityType.EPIC_AN94)]

public class AN94 : Ability
{
    private ushort _an94Serial;
    private double _lastHandledTriggerPress =  double.NegativeInfinity;
    private bool _isApplyingBurstDamage;
    private int _burstDamageToken;
    private int _pendingBurstDamageToken;

    public override void OnEnabled()
    {
        // 서버에서도 트리거 누름/뗌 상태를 신뢰할 수 있도록 한 번만 패치를 적용한다.
        FirearmTriggerStatePatch.Ensure();

        Item item = Owner.AddItem(ItemType.GunAK);
        for (int i = 0; i < 4; i++) {
            Owner.AddItem(ItemType.Ammo762x39);
        }
        _an94Serial = item.Serial;
        ApplyAn94Settings(item.As<Firearm>());

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {

    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item == null || ev.Item.Serial != _an94Serial)
            return;

        ev.Player.AddHint("AN-94", $"<b><color={ABattle.RatingColor["영웅"]}>AN-94</color></b> 능력이 있는 AK 입니다");
    }
    public void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Item.Serial != _an94Serial) return;

        Firearm firearm = ev.Item.As<Firearm>();
        ApplyAn94Settings(firearm);

        // 반자동 매커니즘: 한 번의 트리거(마우스) 입력마다 첫 발만 허용한다.
        // 풀오토는 트리거를 누른 채로 유지하는 동안 같은 LastTriggerPress 값으로 여러 번의 Shooting 이벤트를 만든다.
        // 따라서 직전에 허용한 누름보다 더 최신의 누름이 들어왔을 때(= 손을 뗐다가 다시 누름)만 발사를 허용하고,
        // 같은 누름에서 이어지는 연사는 모두 취소한다. 시간 간격이 아닌 입력 엣지로 판별하므로 네트워크 지터에 영향을 받지 않는다.
        if (!IsFreshTriggerPull(firearm))
        {
            ev.IsAllowed = false;
            return;
        }

        _pendingBurstDamageToken = 0;

        if (TryConsumeBurstAmmo())
            RegisterPendingBurstDamage();
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (_isApplyingBurstDamage ||
            ev.Attacker != Owner ||
            ev.Player == ev.Attacker ||
            ev.Attacker.CurrentItem == null ||
            ev.Attacker.CurrentItem.Serial != _an94Serial ||
            ev.DamageHandler.CustomBase is not FirearmDamageHandler firearmDamageHandler ||
            !TryConsumeBurstDamageToken())
            return;

        Player target = ev.Player;
        Player attacker = ev.Attacker;
        float damage = ev.DamageHandler.Damage;

        Timing.CallDelayed(0.034f, () =>
        {
            if (!ev.IsAllowed ||
                target == null ||
                attacker == null ||
                !target.IsAlive)
                return;

            try
            {
                _isApplyingBurstDamage = true;
                firearmDamageHandler.Damage = damage;
                firearmDamageHandler.Attacker = attacker;
                firearmDamageHandler.IsSuicide = false;
                target.Hurt(firearmDamageHandler);
            }
            finally
            {
                _isApplyingBurstDamage = false;
            }
        });
    }

    private bool TryConsumeBurstAmmo()
    {
        if (Item.Get(_an94Serial) is not Firearm firearm || firearm.MagazineAmmo <= 0)
            return false;

        firearm.MagazineAmmo -= 1;
        return true;
    }

    private void RegisterPendingBurstDamage()
    {
        int token = ++_burstDamageToken;
        _pendingBurstDamageToken = token;

        Timing.CallDelayed(0.1f, () =>
        {
            if (_pendingBurstDamageToken == token)
                _pendingBurstDamageToken = 0;
        });
    }

    private bool TryConsumeBurstDamageToken()
    {
        if (_pendingBurstDamageToken == 0)
            return false;

        _pendingBurstDamageToken = 0;
        return true;
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
    private static void ApplyAn94Settings(Firearm firearm)
    {
        if (firearm == null)
            return;

        firearm.DamageFalloffDistance = 500f;
    }
}
