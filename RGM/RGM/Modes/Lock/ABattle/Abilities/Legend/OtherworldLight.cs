using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles.PlayableScps.HumeShield;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

[Ability("이계의 빛", """
                  시공의 빛으로 적을 멸합니다.
                  지급된 동전으로 상대 타격 시, 상대의 현재 체력, 실드를 1로 만듭니다. 해당 공격은 『관통』 효과가 적용됩니다.
                  타격 시 10초간 HS 회복이 차단되며, 재사용 대기시간 90초가 적용됩니다.
                  """, AbilityCategory.Legend, AbilityType.LEGEND_OTHERWORLDLIGHT)]
public class OtherworldLight : Ability
{
    private const float HsBlockDuration = 10f;

    private ushort _serial;
    private int _cooldown;

    private class TemporaryHumeShieldBlocker : IHumeShieldBlocker
    {
        public bool HumeShieldBlocked { get; set; } = true;
    }

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.Coin);
        _serial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _serial)
            return;
        
        ev.Player.AddHint("동전 사용 설명", $"이 동전으로 상대를 맞추면 <b><color={ABattle.RatingColor["전설"]}>이계의 빛</color></b> 능력을 사용할 수 있습니다.");
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (ev.Item.Serial != _serial)
            return;

        if (!ev.Player.TryGetLookPlayer(100f, out Player target, out RaycastHit? _))
        {
            ev.Player.AddHint("동전 사용 실패", "대상을 정확히 지정해 주세요.");
            return;
        }

        if (!HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, target.ReferenceHub))
        {
            ev.Player.AddHint("동전 사용 실패", "잘못된 대상입니다.");
            return;
        }

        if (_cooldown > 0)
        {
            ev.Player.AddHint("동전 사용 실패", $"{_cooldown}초 뒤 다시 시도해주세요.");
            return;
        }

        DynamicHumeShieldController ctrl = null;
        TemporaryHumeShieldBlocker blocker = null;

        if (target.ReferenceHub.roleManager.CurrentRole is IHumeShieldedRole { HumeShieldModule: DynamicHumeShieldController module })
        {
            ctrl = module;
            blocker = new TemporaryHumeShieldBlocker();
            ctrl.AddBlocker(blocker);
        }

        target.Health = 1;
        target.ArtificialHealth = 1;

        if (ctrl != null)
        {
            ctrl.HsCurrent = 1f;
            Timing.CallDelayed(HsBlockDuration, () => blocker.HumeShieldBlocked = false);
        }
        else
        {
            target.HumeShield = 1;
        }

        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);

        _cooldown = 90;
        Timing.RunCoroutine(CooldownTimer());
    }

    private IEnumerator<float> CooldownTimer()
    {
        while (_cooldown > 0)
        {
            _cooldown--;
            yield return Timing.WaitForSeconds(1f);
        }
    }
}