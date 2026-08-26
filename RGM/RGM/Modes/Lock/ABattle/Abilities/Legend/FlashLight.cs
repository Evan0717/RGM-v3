using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("플래시라이트", "지급된 손전등을 들고 상대를 쳐다보면 눈뽕 공격을 가할 수 있습니다.", AbilityCategory.Legend, AbilityType.LEGEND_FLASHLIGHT)]
public class FlashLight : Ability
{
    CoroutineHandle _onStarted;
    private ushort _flashLightSerial;

    public override void OnEnabled()
    {
        Item fl = Owner.AddItem(ItemType.Flashlight);
        _flashLightSerial = fl.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;

        if (Timing.IsRunning(_onStarted)) return;
        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _flashLightSerial)
            return;
        
        ev.Player.AddHint("플래시라이트", $"손전등을 상대에게 비추면 <b><color={ABattle.RatingColor["전설"]}>플래시라이트</color></b> 능력을 사용할 수 있습니다.");
    }

    private IEnumerator<float> OnStarted()
    {
        while (true)
        {
            foreach (var player in PlayerManager.List.Where(player =>
             player.IsAlive &&
             player.CurrentItem != null &&
             _flashLightSerial == player.CurrentItem.Serial))
            {
                if (player.HasAbility(AbilityType.SYNERGY_REFLECTEDLIGHT))
                {
                    foreach (var target in PlayerManager.List.Where(p => p != null && p.IsAlive))
                    {
                        if (target == player) continue;
                        if (!HitboxIdentity.IsEnemy(player.ReferenceHub, target.ReferenceHub)) continue;

                        if (player.IsLookingAt(target, fov: 5))
                        {
                            Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 2f);
                            target.EnableEffect(EffectType.Burned, 1, 10f);
                            target.EnableEffect(EffectType.Flashed, 1, 1.5f);
                            target.AddHint("따가움", "<b><color=#FFFF00>불타는 안구</color></b>");
                        }
                        else if (player.IsLookingAt(target, fov: 20))
                        {
                            Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.8f);
                            target.EnableEffect(EffectType.Flashed, 1, 1.5f);
                        }
                    }
                }
                else
                {
                    if (!player.TryGetLookPlayer(45, out var target, out _)) continue;
                    if (player == target || !HitboxIdentity.IsEnemy(player.ReferenceHub, target.ReferenceHub)) continue;

                    Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.8f);
                    target.EnableEffect(EffectType.Flashed, 1, 1.5f);
                }
            }

            yield return Timing.WaitForSeconds(0.0417f);
        }
    }
}
