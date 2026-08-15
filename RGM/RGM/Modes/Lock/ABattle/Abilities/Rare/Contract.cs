using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Rare;

[Ability("계약", "지급된 동전을 튕기면 당장 죽지만, 다음 생에 능력 5개를 가진 채로 시작합니다.(별도 등급 확률 적용)", AbilityCategory.Rare, AbilityType.RARE_CONTRACT)]
public class Contract : Ability
{
    private ushort _contractCoinSerial;

    public override void OnEnabled()
    {
        Item cc = Owner.AddItem(ItemType.Coin);
        _contractCoinSerial = cc.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _contractCoinSerial)
            return;
        
        ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["희귀"]}>계약</color></color></b> 능력을 사용할 수 있습니다.");
    }

    private IEnumerator<float> OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (_contractCoinSerial != ev.Item.Serial)
            yield break;

        Player player = ev.Player;
        ev.Item.Destroy();

        if (GodModePlayers.Contains(player))
            GodModePlayers.Remove(player);
            
        player.RemoveAllAbilities();
        player.Kill("계약에 따라 당신은 죽었습니다.");

        while (!player.IsAlive)
            yield return Timing.WaitForOneFrame;
            
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    var rand = Random.Range(1, 401);
                    switch (rand)
                    {
                        case 1: // 0.25%
                            player.AddAbility(ABattle.Instance.GetRandomAbilities(player, AbilityCategory.Mythic, 1)[0]);
                            break;
                    
                        case <= 5: // 1.25%
                            player.AddAbility(ABattle.Instance.GetRandomAbilities(player, AbilityCategory.Legend, 1)[0]);
                            break;
                    
                        case <= 52: // 13.0%
                            player.AddAbility(ABattle.Instance.GetRandomAbilities(player, AbilityCategory.Epic, 1)[0]);
                            break;
                    
                        case <= 142: // 35.5%
                            player.AddAbility(ABattle.Instance.GetRandomAbilities(player, AbilityCategory.Rare, 1)[0]);
                            break;
                    
                        default: // 48.5%
                            player.AddAbility(ABattle.Instance.GetRandomAbilities(player, AbilityCategory.Common, 1)[0]);
                            break;
                    }
                }
            }
        );
    }
}
