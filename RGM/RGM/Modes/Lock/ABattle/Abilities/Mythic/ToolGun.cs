using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using ProjectMER.Features.Serializable;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Mythic;

[Ability("딸깍", "지급된 동전을 튕기면 보는 방향에 워크스테이션을 설치합니다. 단, 3% 확률로 즉사합니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_TOOLGUN)]
public class ToolGun : Ability
{
    private const float ForwardOffset = 1.5f;
    private const float DownwardOffset = 0.5f;

    private ushort coinSerial;

    public override void OnEnabled()
    {
        Item coin = Owner.AddItem(ItemType.Coin);
        coinSerial = coin.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    public override void OnDisabled()
    {
        
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial == coinSerial)
        {
            ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["신화"]}>워크스테이션</color></b>을 설치합니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (ev.Item.Serial != coinSerial)
            return;

        Player player = ev.Player;
        if (Random.Range(1, 101) <= 3)
        {
            if (GodModePlayers.Contains(player))
                GodModePlayers.Remove(player);

            player.Hit(player, player.MaxHealth);
        }

        Vector3 forward = player.CameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 rayOrigin = player.Position + forward * ForwardOffset + Vector3.up;
        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100, (LayerMask)1))
            return;

        new SerializableWorkstation
        {
            IsInteractable = true,
            Position = hit.point + Vector3.down * DownwardOffset,
            Rotation = new Vector3(0, player.Rotation.eulerAngles.y, 0),
            Scale = Vector3.one
        }.SpawnOrUpdateObject();
    }
}