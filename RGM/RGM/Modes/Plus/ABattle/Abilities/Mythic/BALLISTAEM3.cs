using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability("발리스타 MP3", "10초마다 탄약이 하나 추가되고, 능력을 삭제하며, 벽을 관통하고, 1200 데미지를 입히는 입자 분열기를 받습니다. 투시 능력을 얻습니다. (사거리 75)", AbilityCategory.Mythic, AbilityType.MYTHIC_BALLISTAEM3)]
public class BALLISTAEM3 : Ability
{
    ushort serial = 0;

    public override void OnEnabled()
    {
        Owner.AddAbility(AbilityType.EPIC_SCP1344);

        Item item = Owner.AddItem(ItemType.ParticleDisruptor);
        serial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Shot += OnShot;

        Timing.RunCoroutine(PlanBAmmo());
    }

    public override void OnDisabled()
    {
    }

    public IEnumerator<float> PlanBAmmo()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(10f);

            if (Owner != null || Owner.IsAlive || Owner.CurrentItem != null)
            {
                Firearm firearm = (Firearm)Item.Get(serial);

                if (firearm.MaxMagazineAmmo > firearm.MagazineAmmo)
                    firearm.MagazineAmmo += 1;
            }
        }
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item != null && serial == ev.Player.CurrentItem.Serial)
        {
            if (serial == ev.Item.Serial)
                ev.Player.AddHint("발리스타 MP3", $"<b><color={ABattle.RatingColor["신화"]}>발리스타 MP3</color></b> 능력이 있는 <b>입자 분열기</b>입니다!");
        }
    }

    public void OnShot(ShotEventArgs ev)
    {
        if (serial == ev.Item.Serial)
        {
            if (Tools.TryGetLookPlayers(ev.Player, 75f, out List<Player> players, out RaycastHit? hit))
            {
                bool enemy = false;

                foreach (var player in players)
                {
                    if (HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, player.ReferenceHub))
                    {
                        if (!ABattle.Instance.PlayerAbilities.ContainsKey(player) || ABattle.Instance.PlayerAbilities[player].Count <= 0)
                            player.Hit(ev.Player, 1200);
                        
                        else
                        {
                            if (Random.Range(1, 11) <= 9)
                            {
                                player.DisableAllEffects();
                                player.RemoveAllAbilities();

                                ABattle.Instance.PlayerWorkstations[player].Clear();
                                ABattle.Instance.PlayerAbilities[player].Clear();
                            }
                            player.Hit(ev.Player, 1200);
                        }
                        
                        
                        enemy = true;
                    }
                }

                if (!enemy)
                {
                }
                else
                {
                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 3f);
                }
            }
        }
    }
}