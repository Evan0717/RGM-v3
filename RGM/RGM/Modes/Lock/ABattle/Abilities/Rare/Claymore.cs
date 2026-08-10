using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles.PlayableScps;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using RGM.API.Features;
using UnityEngine;
namespace RGM.Modes.Abilities.Rare;

[Ability("크레모아", "크레모아를 설치할 수 있는 동전을 지급받습니다.", AbilityCategory.Rare, AbilityType.RARE_CLAYMORE)]
public class Claymore : Ability
{
    private ushort _claymoreCoinSerial;
    private CoroutineHandle _handle;
    private static int excludeMask = ~((1 << 8) | (1 << 2));
    private static float distance = 5f;
    private static float ForwardRange = 7f;
    private static float BackwardRange = 2f;

    public override void OnEnabled()
    {
        Item cc = Owner.AddItem(ItemType.Coin);
        _claymoreCoinSerial = cc.Serial;
        
        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }
    
    public override void OnDisabled() {}

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _claymoreCoinSerial)
            return;
        
        ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["희귀"]}>크레모아</color></color></b> 능력을 사용할 수 있습니다.");
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (_claymoreCoinSerial != ev.Item.Serial)
            return;
        

        if (!Tools.TryGetLookPoint(ev.Player, distance, Tools.SurfaceType.Floor, out Vector3 point,
                layerMask: excludeMask))
        {
            ev.Player.AddHint("동전 사용 설명", "벽이나 가파른 경사에는 설치하실 수 없습니다.");
            return;
        }
        
        ev.Item.Destroy();

        SchematicObject claymore = ObjectSpawner.SpawnSchematic("Claymore", point, Quaternion.Euler(0, ev.Player.Rotation.eulerAngles.y, 0));
        _handle = Timing.RunCoroutine(Corutine(point, claymore));
    }
    //TODO: 중간에 Owner 가 나가는 체크 꼭 해줘야함
    private IEnumerator<float> Corutine(Vector3 position, SchematicObject schematic)
    {
        Vector3 forward = schematic.transform.forward;
        float maxRange = Mathf.Max(ForwardRange, BackwardRange);
        int visionMask = VisionInformation.VisionLayerMask;
        while (true)
        {
            foreach (var player in PlayerManager.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, position) <= maxRange))
            {
                if (player == Owner || !HitboxIdentity.IsEnemy(Owner.ReferenceHub, player.ReferenceHub))
                    continue;

                Vector3 offset = player.Position - position;
                float forwardDistance = Vector3.Dot(forward, offset);

                bool inRange = forwardDistance >= 0
                    ? forwardDistance <= ForwardRange
                    : -forwardDistance <= BackwardRange;

                if (!inRange)
                    continue;
                
                if (Physics.Linecast(position, player.Position, visionMask))
                    continue;
                
                ExplosiveGrenade eg = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                eg.FuseTime = 0f;
                eg.MaxRadius = 4.5f;
                eg.SpawnActive(Tools.GetPointForward(schematic.transform, 2.5f), Owner);
                
                
                schematic.Destroy();
                yield break;
            }

            yield return Timing.WaitForOneFrame;
        }
    }
}