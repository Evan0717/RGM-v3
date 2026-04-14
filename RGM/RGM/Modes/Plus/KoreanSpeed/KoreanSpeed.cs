using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes;

[Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.KoreanSpeed)]
public class KoreanSpeed : Mode
{
    public override string Name => "한국인이 좋아하는 속도";
    public override string Description => "누군가가 사망할 때마다 모두의 속도가 증가합니다.";

    public override string Detail =>
        "<b><color=#FB00FF>슈</color><color=#D200D5>우</color><color=#A901AB>우</color><color=#800282>우</color><color=#570358>웅</color><color=#2E042E>화</color></b>";

    public override string Color => "5882FA";

    public static KoreanSpeed Instance;

        int count;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.ThrowingRequest += OnThrowingRequest;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;
            Exiled.Events.Handlers.Player.ThrowingRequest -= OnThrowingRequest;
        }

    private void OnDied(DiedEventArgs ev)
    {
        if (count != 125)
            count++;

            foreach (var player in PlayerManager.List)
            {
                player.EnableEffect(EffectType.MovementBoost, (byte)(count * 2));
            }
        }

    private void OnSearchingPickup(SearchingPickupEventArgs ev)
    {
        ev.SearchTime -= count * 0.1f;
    }

    private void OnThrowingRequest(ThrowingRequestEventArgs ev)
    {
        ev.Throwable.PinPullTime -= count * 0.1f;
    }


}