using System.Collections.Generic;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Rare;

[Ability("계약", "지급된 동전을 튕기면 당장 죽지만, 다음 생에 능력 3개를 가진 채로 시작합니다.", AbilityCategory.Rare, AbilityType.RARE_CONTRACT)]
public class Contract : Ability
{
    ushort ContractCoinSerial = 0;

    public override void OnEnabled()
    {
        Item cc = Owner.AddItem(ItemType.Coin);
        ContractCoinSerial = cc.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item != null)
        {
            if (ContractCoinSerial == ev.Item.Serial)
                ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["희귀"]}>계약</color></color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public IEnumerator<float> OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        ushort Serial = ev.Item.Serial;

        if (ContractCoinSerial == Serial)
        {
            ev.Item.Destroy();

            Owner.RemoveAllAbilities();

            if (GodModePlayers.Contains(Owner))
                GodModePlayers.Remove(Owner);

            ev.Player.Kill("계약에 따라 당신은 죽었습니다.");

            while (!ev.Player.IsAlive)
                yield return Timing.WaitForOneFrame;

            for (int i = 1; i < 4; i++)
            {
                ABattle.Instance.StartSelect(ev.Player);

                while (ABattle.Instance.IsSelecting[ev.Player])
                    yield return Timing.WaitForOneFrame;
            }
        }
    }
}
