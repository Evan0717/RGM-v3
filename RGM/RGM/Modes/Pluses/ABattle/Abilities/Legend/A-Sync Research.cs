using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using ProjectMER.Features;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

[Ability("A-Sync Research", "지급된 영구적인 동전을 튕기면 최대 1분간 대상과 자신을 다른 공간으로 이동시킵니다.", AbilityCategory.Legend, AbilityType.LEGEND_ASYNC)]
public class ASync : Ability
{
    ushort serial = 0;
    int time = 0;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.Coin);
        serial = item.Serial;

        if (!MapUtils.LoadedMaps.ContainsKey("Backroom"))
        {
            Tools.LoadMap("Backroom");
        }

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
            if (serial == ev.Item.Serial)
                ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["전설"]}>A-Sync Research</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (ev.Item.Serial == serial)
        {
            if (Tools.TryGetLookPlayers(ev.Player, 100f, out List<Player> players, out RaycastHit? hit))
            {
                if (time > 0)
                {
                    ev.Player.AddHint("동전 사용 실패", $"{time}초 뒤 다시 시도해주세요.");
                }
                else
                {
                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);

                    Vector3 pos = GameObject.Find("[SP] First").transform.position;

                    ev.Player.Position = pos;
                    ev.Player.AddEffect(EffectType.MovementBoost, 20, 60);
                    ev.Player.AddEffect(EffectType.AntiScp207, 1, 60);

                    foreach (var player in players)
                    {
                        player.Position = pos;
                        player.AddEffect(EffectType.Stained, 1, 5);
                        player.AddEffect(EffectType.Flashed, 1, 1);
                    }

                    time = 60 * 4;

                    IEnumerator<float> timer()
                    {
                        while (time > 0)
                        {
                            time--;

                            yield return Timing.WaitForSeconds(1f);
                        }
                    }

                    void returnTo()
                    {
                        Vector3 pos2 = Door.Get(DoorType.NukeSurface).Position;

                        ev.Player.Position = new Vector3(pos2.x, pos2.y + 2, pos2.z);

                        foreach (var player in players)
                        {
                            player.Position = new Vector3(pos2.x, pos2.y + 2, pos2.z);
                        }
                    }

                    IEnumerator<float> returnToSurface()
                    {
                        yield return Timing.WaitForSeconds(60);

                        returnTo();
                    }

                    Timing.RunCoroutine(timer());
                    CoroutineHandle coroutine1 = Timing.RunCoroutine(returnToSurface());

                    void OnDied(DiedEventArgs ev2)
                    {
                        if (ev2.Player == ev.Player || players.Contains(ev2.Player))
                        {
                            returnTo();

                            Timing.KillCoroutines(coroutine1);

                            Exiled.Events.Handlers.Player.Died -= OnDied;
                        }
                    }

                    Exiled.Events.Handlers.Player.Died += OnDied;

                    Timing.CallDelayed(60, () =>
                    {
                        Exiled.Events.Handlers.Player.Died -= OnDied;
                    });
                }
            }
            else
                ev.Player.AddHint("동전 사용 실패", "대상을 정확히 지정해 주세요.");
        }
    }
}
