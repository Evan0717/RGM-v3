using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using RGM.API.Features;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RGM.Modes.Abilities.Mythic;

[Ability("데몬 코어", "수상한 플루토늄 반구입니다. 임계질량을 넘는 순간 무슨 일이 일어날지 모릅니다.",
    AbilityCategory.Mythic, AbilityType.MYTHIC_HALLOWEEN_DEMONCORE,
    RoleAbility.None, false, AbilityHolidayType.Halloween)]
public class DemonCore : Ability
{
    private const float UseLockDuration = 0.2f;
    private const float RadiationInterval = 0.2f;
    private const float RadiationHealthFraction = 0.01f;

    // 같은 구역은 한 번만 오염된다. 능력 보유자가 죽어도 피해 주체가 남도록 인스턴스 밖에 둔다.
    private static readonly Dictionary<ZoneType, Player> RadiationByZone = [];
    private static CoroutineHandle _radiation;
    private static bool _roundHooked;

    private ushort _demonCoreSerial;
    private int _destructionLevel;
    private int _remainingUses;
    private float _nextUseTime;
    private bool _detonated;
    private bool _watchingSwing;
    private bool _watchingCharge;

    public override void OnEnabled()
    {
        EnsureRoundHook();

        // WearState 는 넣지 않는다. 파괴 강도 0은 5회, 4는 1회만 쓸 수 있다.
        _destructionLevel = Random.Range(0, 5);
        _remainingUses = 5 - _destructionLevel;

        _demonCoreSerial = Owner.AddItem(ItemType.Jailbird).Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Item.Swinging += OnSwinging;
        Exiled.Events.Handlers.Item.ChargingJailbird += OnChargingJailbird;
        Exiled.Events.Handlers.Item.JailbirdChargeComplete += OnChargeComplete;
        Exiled.Events.Handlers.Item.JailbirdChangingWearState += OnChangingWearState;

        Owner.AddHint("데몬 코어", Describe(), 4f);
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Item.Swinging -= OnSwinging;
        Exiled.Events.Handlers.Item.ChargingJailbird -= OnChargingJailbird;
        Exiled.Events.Handlers.Item.JailbirdChargeComplete -= OnChargeComplete;
        Exiled.Events.Handlers.Item.JailbirdChangingWearState -= OnChangingWearState;
    }

    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Player == null || ev.Item?.Serial != _demonCoreSerial)
            return;

        ev.Player.AddHint("데몬 코어", Describe());
    }

    private void OnSwinging(SwingingEventArgs ev)
    {
        // 이 이벤트는 서버가 공격을 수락하기 전에 온다. 수락된 휘두르기만 한 프레임 뒤에 센다.
        if (_detonated || _watchingSwing || !ev.IsAllowed || ev.Player == null ||
            ev.Item is not Jailbird jailbird || jailbird.Serial != _demonCoreSerial ||
            jailbird.Base._attackTriggered)
        {
            return;
        }

        _watchingSwing = true;
        Player attacker = ev.Player;
        Timing.CallDelayed(Timing.WaitForOneFrame, () => ConfirmSwing(attacker));
    }

    private void ConfirmSwing(Player attacker)
    {
        _watchingSwing = false;
        if (_detonated || attacker?.CurrentItem is not Jailbird jailbird ||
            jailbird.Serial != _demonCoreSerial || !jailbird.Base._attackTriggered)
        {
            return;
        }

        ResetDurability(jailbird);
        RegisterUse(attacker);
    }

    private void OnChargingJailbird(ChargingJailbirdEventArgs ev)
    {
        if (ev.Jailbird?.Serial != _demonCoreSerial)
            return;

        ResetDurability(ev.Jailbird);
    }

    private void OnChargeComplete(JailbirdChargeCompleteEventArgs ev)
    {
        if (_detonated || _watchingCharge || !ev.IsAllowed || ev.Player == null ||
            ev.Jailbird?.Serial != _demonCoreSerial || ev.Jailbird.Base._charging)
        {
            return;
        }

        _watchingCharge = true;
        Player attacker = ev.Player;
        Timing.CallDelayed(Timing.WaitForOneFrame, () => ConfirmCharge(attacker));
    }

    private void ConfirmCharge(Player attacker)
    {
        _watchingCharge = false;
        if (_detonated || attacker?.CurrentItem is not Jailbird jailbird ||
            jailbird.Serial != _demonCoreSerial || !jailbird.Base._charging)
        {
            return;
        }

        ResetDurability(jailbird);
        RegisterUse(attacker);
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker?.CurrentItem is not Jailbird jailbird ||
            jailbird.Serial != _demonCoreSerial ||
            ev.DamageHandler.Type != DamageType.Jailbird)
        {
            return;
        }

        // 타격 도중 쌓인 피해량은 공격이 끝난 뒤 되돌린다.
        Player attacker = ev.Attacker;
        Timing.CallDelayed(Timing.WaitForOneFrame, () => ResetDurability(attacker));
    }

    private void OnChangingWearState(JailbirdChangingWearStateEventArgs ev)
    {
        if (ev.Jailbird?.Serial != _demonCoreSerial)
            return;

        // 게임 내구도가 깎여 파괴 단계가 올라가는 것만 막는다.
        if (ev.NewWearState > ev.OldWearState)
            ev.IsAllowed = false;
    }

    private void RegisterUse(Player attacker)
    {
        if (_detonated || Time.time < _nextUseTime)
            return;

        _nextUseTime = Time.time + UseLockDuration;
        _remainingUses--;

        if (_remainingUses > 0)
        {
            attacker.AddHint("데몬 코어", Describe(), 2f);
            Timing.CallDelayed(Timing.WaitForOneFrame, () => ResetDurability(attacker));
            return;
        }

        _detonated = true;
        Vector3 position = attacker.Position;
        ZoneType zone = attacker.Zone;
        Timing.CallDelayed(Timing.WaitForOneFrame, () => Detonate(attacker, position, zone));
    }

    private void Detonate(Player attacker, Vector3 position, ZoneType zone)
    {
        if (attacker == null)
            return;

        DestroyCore(attacker);

        if (!attacker.IsConnected)
            return;

        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, attacker);
        grenade.FuseTime = 0.1f;
        grenade.SpawnActive(position, attacker);

        if (!IsFacilityZone(zone))
            return;

        foreach (Player target in PlayerManager.List)
        {
            if (!target.IsAlive || target == attacker || target.Zone != zone)
                continue;

            if (HitboxIdentity.IsEnemy(attacker.ReferenceHub, target.ReferenceHub))
                ApplyInstantKill.Apply(attacker, target);
            else
                ApplyLethalDamage.Apply(attacker, target);
        }

        if (attacker.IsAlive)
            ApplyLethalDamage.Apply(attacker, attacker);

        Contaminate(zone, attacker);
    }

    private void DestroyCore(Player attacker)
    {
        Item held = attacker.Items.FirstOrDefault(item => item.Serial == _demonCoreSerial);
        if (held != null)
            attacker.RemoveItem(held);

        Pickup dropped = Pickup.List.FirstOrDefault(pickup => pickup.Serial == _demonCoreSerial);
        dropped?.Destroy();
    }

    private void ResetDurability(Player player)
    {
        if (player == null)
            return;

        foreach (Item item in player.Items)
        {
            if (item is Jailbird jailbird && jailbird.Serial == _demonCoreSerial)
                ResetDurability(jailbird);
        }
    }

    private void ResetDurability(Jailbird jailbird)
    {
        if (jailbird == null || jailbird.Serial != _demonCoreSerial)
            return;

        if (jailbird.TotalCharges != 0)
            jailbird.TotalCharges = 0;

        if (jailbird.TotalDamageDealt != 0f)
            jailbird.TotalDamageDealt = 0f;
    }

    private string Describe()
    {
        return $"<b><color={ABattle.RatingColor["신화"]}>데몬 코어</color></b> 능력이 있는 <b>제일버드</b>입니다!\n" +
               $"파괴 강도 <b>{_destructionLevel}</b> · 남은 사용 <b>{_remainingUses}</b>회";
    }

    private static bool IsFacilityZone(ZoneType zone)
    {
        return zone is ZoneType.LightContainment
            or ZoneType.HeavyContainment
            or ZoneType.Entrance
            or ZoneType.Surface;
    }

    private static void Contaminate(ZoneType zone, Player attacker)
    {
        if (!RadiationByZone.ContainsKey(zone))
            RadiationByZone[zone] = attacker;

        if (!Timing.IsRunning(_radiation))
            _radiation = Timing.RunCoroutine(RadiationLoop());
    }

    private static IEnumerator<float> RadiationLoop()
    {
        while (RadiationByZone.Count > 0)
        {
            yield return Timing.WaitForSeconds(RadiationInterval);

            if (RadiationByZone.Count == 0)
                yield break;

            foreach (Player player in PlayerManager.List)
            {
                if (!player.IsAlive || !RadiationByZone.TryGetValue(player.Zone, out Player attacker))
                    continue;

                if (attacker == null || !attacker.IsConnected)
                {
                    RadiationByZone.Remove(player.Zone);
                    continue;
                }

                ApplyFixedDamage.Apply(attacker, player, player.MaxHealth * RadiationHealthFraction);
            }
        }
    }

    private static void EnsureRoundHook()
    {
        if (_roundHooked)
            return;

        _roundHooked = true;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        Exiled.Events.Handlers.Server.WaitingForPlayers += ClearRadiation;
    }

    private static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        ClearRadiation();
    }

    private static void ClearRadiation()
    {
        RadiationByZone.Clear();
        Timing.KillCoroutines(_radiation);
    }
}
