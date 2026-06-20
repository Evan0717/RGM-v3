using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using PlayerRoles;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability(
    "발리스타 MP3",
    "10초마다 탄약이 하나 추가되고, 90% 확률로 능력을 삭제하며, 벽을 관통하고, 1300 데미지를 입히는 입자 분열기를 받습니다. 투시 능력을 얻습니다. (사거리 75)\n능력 삭제 시도에 실패하거나 능력이 없는 대상을 공격 시, 최종 데미지가 150% 증가합니다.",
    AbilityCategory.Mythic,
    AbilityType.MYTHIC_BALLISTAEM3)]
public class BALLISTAEM3 : Ability
{
    private ushort _serial;
    private bool _isActive;
    private const float Damage = 1500 * 2.7f;

    public override void OnEnabled()
    {
        Owner.AddAbility(AbilityType.EPIC_SCP1344);

        Item item = Owner.AddItem(ItemType.ParticleDisruptor);
        _serial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;

        Timing.RunCoroutine(PlanBAmmo());
    }

    public override void OnDisabled() { }
    
    private IEnumerator<float> PlanBAmmo()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(10f);

            if (Owner != null && Owner.IsAlive && Owner.CurrentItem != null)
            {
                var firearm = (Firearm)Item.Get(_serial);
                
                if (firearm.MaxMagazineAmmo > firearm.MagazineAmmo) {}

                firearm.MagazineAmmo += 1;
            }
        }
    }

    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item == null) return;
        
        if (_serial != ev.Player.CurrentItem.Serial) return;
            ev.Player.AddHint("발리스타 MP3",  $"<b><color={ABattle.RatingColor["신화"]}>발리스타 MP3</color></b> 능력이 있는 <b>입자 분열기</b>입니다!");
    }
    
    private void OnHurting(HurtingEventArgs ev)
    {
        if (_isActive) return;
        Waiting();     
        if (_serial != ev.Attacker.CurrentItem.Serial) return;
        if (!Tools.TryGetLookPlayers(ev.Attacker, 75f, out List<Player> players, out _)) return;
        bool enemy = false;

        Log.Info(players.Count);
        
        foreach (var player in players.Where(player => HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, player.ReferenceHub)).Where(player => player.Role != RoleTypeId.Spectator))
        {
            if (!ABattle.Instance.PlayerAbilities.TryGetValue(player, out var ability) || ability.Count <= 0)
                player.Hit(ev.Attacker, Damage);
            else if (Mathf.Clamp01(Random.Range(0.0f, 1f)) >= .1f)
            {
                player.Hit(ev.Attacker, Damage);
                
                player.DisableAllEffects();
                player.RemoveAllAbilities();
                    
                ABattle.Instance.PlayerAbilities[player].Clear();
                ABattle.Instance.PlayerWorkstations[player].Clear();
            }
            else
                player.Hit(ev.Attacker, 4105);
            
            enemy = true;
        }
        if (enemy)
            Hitmarker.SendHitmarkerDirectly(ev.Attacker.ReferenceHub, 3f);
    }

    private void Waiting()
    {
        _isActive = true;
        Timing.CallDelayed(3f, () => _isActive = false);
    }
}