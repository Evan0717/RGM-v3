using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using RGM.API.Features;
using RGM.API.DataBases;
using PlayerRoles;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.RandomItem)]
    public class RandomItem : Mode
    {
        private static bool _isEnabled;
        
        public override string Name => "랜덤박스";
        public override string Description => "60초마다 랜덤한 아이템을 얻을 수 있습니다!";

        public override string Detail =>
            """
            무작위 아이템들이 동일한 확률로 지급됩니다.

            이후, 60초마다 무작위 아이템들을 하나 더 받습니다.
            """;

        public override string Color => "BFFF00";

        public static RandomItem Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            _isEnabled = true;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            _isEnabled = false;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
        }

        private IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                Timing.RunCoroutine(Spawned(player));
            }

            while (_isEnabled)
            {
                yield return Timing.WaitForSeconds(60f);

                foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x.Role.Type != RoleTypeId.Scp079))
                    try
                    {
                        Item item = player.AddRandomItem();

                        player.AddHint("랜덤박스", $"<color=#F3F781>{Trans.Item[item.Type]}</color>(을)를 지급받았습니다.",
                            5);
                    }
                    catch (KeyNotFoundException e)
                    { Log.Warn($"[RGM] RandomItem card fetch failure: {e.Message}"); }
                    catch (Exception ex) { Log.Error($"[RGM] RandomItem Mode Error: {ex}"); }
            }
        }

        private void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (ev.Player.IsNonePlayer())
                return;

            if (_isEnabled)
                Timing.RunCoroutine(Spawned(ev.Player));
        }

        private IEnumerator<float> Spawned(Player player)
        {
            if (!player.IsAlive)
                yield break;

            yield return Timing.WaitForOneFrame;

            player.ClearInventory();

            for (int i = 1; i < 9; i++)
            {
                player.AddRandomItem();

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}