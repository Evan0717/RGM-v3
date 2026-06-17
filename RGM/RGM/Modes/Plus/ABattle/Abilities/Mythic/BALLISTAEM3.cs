using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability(
    "발리스타 MP3",
    "10초마다 탄약이 하나 추가되고, 90% 확률로 능력을 삭제하며, 벽을 관통하고, 1600 데미지를 입히는 입자 분열기를 받습니다. 투시 능력을 얻습니다. (사거리 75)",
    AbilityCategory.Mythic,
    AbilityType.MYTHIC_BALLISTAEM3)]
public class BALLISTAEM3 : Ability
{
    private ushort _serial;

    public override void OnEnabled()
    {
        Owner.AddAbility(AbilityType.EPIC_SCP1344);

        Item item = Owner.AddItem(ItemType.ParticleDisruptor);
        _serial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Shot += OnShot;

        Timing.RunCoroutine(PlanBAmmo());
    }

    public override void OnDisabled()
    {
    }
    
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
        if (_serial == ev.Player.CurrentItem.Serial && ev.Item != null)
        {
            if (_serial == ev.Item.Serial)
                ev.Player.AddHint("발리스타 MP3", $"<b><color={ABattle.RatingColor["신화"]}>발리스타 MP3</color></b> 능력이 있는 <b>입자 분열기</b>입니다!");
        }
    }

    private void OnShot(ShotEventArgs ev)
    {
        if (_serial != ev.Item.Serial) return;
        if (!Tools.TryGetLookPlayers(ev.Player, 75f, out List<Player> players, out _)) return;
        bool enemy = false;

        foreach (var player in players.Where(player =>
                     HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, player.ReferenceHub)))
        {
            if (!ABattle.Instance.PlayerAbilities.TryGetValue(player, out var ability) || ability.Count <= 0)
                player.Hit(ev.Player, 1600);
            else
            {
                if (Random.Range(1, 11) <= 9)
                {
                    player.DisableAllEffects();
                    player.RemoveAllAbilities();
                            
                    ABattle.Instance.PlayerAbilities[player].Clear();
                    ABattle.Instance.PlayerWorkstations[player].Clear();
                }
                player.Hit(ev.Player, 1600);
            }

            enemy = true;
        }

        if (enemy)
        {
            Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 3f);
        }
    }
}
