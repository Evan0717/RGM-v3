using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerRoles;
using RGM.Modes;
using System.Xml.Linq;
using Exiled.API.Extensions;

namespace RGM.Modes.Abilities.Mythic;

[Ability(
    "구속", "공격불가 상태로 대상을 끌고다닐 수 있고, 벽을 관통할 수 있는(이때 데미지는 없음) 리볼버를 얻습니다. (사거리 100)\nalt를 눌러 상태를 해제시킬 수 있습니다.",
    AbilityCategory.Mythic,
    AbilityType.MYTHIC_ANCHOR)] 
public class Anchor : Ability
{
    ushort itemSerial = 0;
    private List<Player> TargetPlayer = new();
    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.GunRevolver);

        itemSerial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
        Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;

        Timing.RunCoroutine(Main());
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (itemSerial != ev.Player.CurrentItem.Serial) return;
        if (ev.Item == null) return;

        if (itemSerial == ev.Item.Serial)
                ev.Player.AddHint("구속", $"<b><color={ABattle.RatingColor["신화"]}>구속</color></b> 능력이 있는 <b>리볼버</b>입니다!");
        
    }

    private void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Player != Owner) return;
        if (itemSerial != ev.Player.CurrentItem.Serial) return;
        if (ev.Item.Serial != itemSerial) return;
        if (!Tools.TryGetLookPlayers(ev.Player, 100f, out List<Player> players, out _)) return;
        bool enemy = false;

        foreach (var player in players)
        {
            if (player == null) continue;
            if (player.Role == RoleTypeId.Spectator) continue;
            if (!HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, player.ReferenceHub)) continue;
            if (TargetPlayer.Contains(player)) continue;
            Ability EnemyAnchor = ABattle.Instance.GetAbility(player, AbilityType.MYTHIC_ANCHOR);


            if (EnemyAnchor != null && ((Anchor)EnemyAnchor).TargetPlayer.Contains(Owner))
            {
                Owner.AddHint("알림", $"구속당한 상태에서 구속한 플레이어를 구속할 수 없습니다.", 3f); //이론상으로 서로 구속하면 날라댕길껄요
                continue;
            }

            bool isAlreadyCaptured = false;
            foreach (var otherPlayer in PlayerManager.List)
            {
                var otherAnchor = ABattle.Instance.GetAbility(otherPlayer, AbilityType.MYTHIC_ANCHOR) as Anchor;

                if (otherAnchor != null && otherAnchor.TargetPlayer.Contains(player))
                {
                    isAlreadyCaptured = true;
                    break;
                }
            }
            if (isAlreadyCaptured)
            {
                Owner.AddHint("알림", $"다른 플레이어에게 구속당한 플레이어는 구속할 수 없습니다.", 3f);
                continue;
            }

            TargetPlayer.Add(player);

            enemy = true;
        }

        if (enemy)
            Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 3f);

        Timing.CallDelayed(1, () =>
        {
            ev.Item.As<Firearm>().MagazineAmmo = 6;
        });
    }

    private IEnumerator<float> Main()
    {
        while (true)
        {
            if (Owner.IsDead || Owner == null) yield break;

            if(TargetPlayer.Count > 0)
            {
                Owner.AddHint("알림", $"구속을 해제하려면 리볼버를 들고 [ALT]를 누르십시오.\n현재 붙잡힌 플레이어 수: {TargetPlayer.Count}", 0.1f);
            }

            Vector3 origin = Owner.ReferenceHub.PlayerCameraReference.position;
            Vector3 direction = Owner.ReferenceHub.PlayerCameraReference.forward;

            float maxDistance = 4f;
            int ignorePlayerMask = ~(1 << LayerMask.NameToLayer("Player"));
            Vector3 position = origin + (direction * maxDistance);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, ignorePlayerMask))
                position = hit.point - (direction * 0.2f);

            foreach (var player in TargetPlayer.ToList())
            {
                if (player == null || player.IsDead || player.Role == RoleTypeId.Spectator)
                {
                    TargetPlayer.Remove(player);
                    continue;
                }
                player.Position = position;
                if(Owner.CurrentItem.Serial == itemSerial) player.AddEffect(EffectType.Fade, 230, 0.05f); //시야 방해 방지
                player.AddHint("알림", $"{Owner.Nickname}에게 붙잡혔습니다.\n다른 플레이어를 공격 할 수 없습니다.", 0.05f);
            }
             yield return Timing.WaitForSeconds(0.05f);
        }
    }


    public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        if (ev.Player != Owner) return;

        TargetPlayer.Clear();
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker == ev.Player) return;

        if (TargetPlayer.Contains(ev.Attacker))
        {
            ev.IsAllowed = false;
        }
    }
}

