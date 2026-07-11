using Exiled.API.Features.Items;
using System.Collections.Generic;
using MEC;
using RGM.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Gamble)]
    public class Gamble : Mode
    {
        public override string Name => "도박";
        public override string Description => "아이템을 떨구면 새로운 아이템을 획득합니다. 단, 2% 확률로 손이 잘립니다.";
        public override string Detail =>
"""
생각 없이 도박을 하다 보면 2%는 금방이랍니다.

<b>* SCP 진영의 경우에도</b>
[Space + ALT]ㅣ도박을 진행할 수 있습니다.
""";
        public override string Color => "8A4B08";


        public static Gamble Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            if (Random.Range(1, 101) <= 10) { //10% 확률로 워크스테이션 업그레이드 시작
                Tools.TryInstallMode(ModeType.ABattle);
            }
            yield return 0f;
        }
        
        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (!(ev.Player.IsScpRole() || ev.Player.Role.Type.ToString().Contains("Flamingo")))
                return;

            ev.Player.AddHint("도박 안내", $"<size=20>[Space + ALT]ㅣ도박을 진행할 수 있습니다.</size>", 10);
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Player.IsScpRole() || ev.Player.Role.Type.ToString().Contains("Flamingo") || !PlayerManager.List.Contains(ev.Player))
                return;

            List<ItemType> ItemList = Tools.EnumToList<ItemType>();
            ItemType Item = Tools.GetRandomValue(ItemList);

            int rand = UnityEngine.Random.Range(1, 101);

            if (0 < rand && rand < 3)
            {
                ev.Player.EnableEffect(EffectType.SeveredHands, 1, 50);
            }
            else
            {
                ev.Item.Destroy();
                Item CurrentItem = ev.Player.AddItem(Item);
                ev.Player.DropItem(CurrentItem);
            }
        }

        public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (!(ev.Player.IsScpRole() || ev.Player.Role.Type.ToString().Contains("Flamingo")) || !ev.Player.IsJumping || ev.Player.GetEffect(EffectType.SeveredHands).IsEnabled || !PlayerManager.List.Contains(ev.Player))
                return;

            int rand = UnityEngine.Random.Range(1, 101);

            if (0 < rand && rand < 3)
                ev.Player.EnableEffect(EffectType.SeveredHands, 1, 50);

            else
            {
                if (ev.Player.IsScpRole())
                    ev.Player.Hit(ev.Player, ev.Player.MaxHealth / 100);

                ev.Player.AddRandomItem();
            }
        }
    }
}
