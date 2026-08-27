namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.JohnWick)]
    public class JohnWick : Mode
    {
        public override string Name => "존 윅";
        public override string Description => "권총류 무기의 데미지가 일정 배율로 상승합니다.";
        public override string Detail =>
"""
COM-15
COM-18
COM-45
.44 리볼버
""";
        public override string Color => "2EFEF7";

        public static JohnWick Instance;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        private void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker == null) return;
            ev.Amount *= ev.Attacker.CurrentItem.Type switch
            {
                ItemType.GunCOM15 => 6.5f,
                ItemType.GunCOM18 => 3.8f,
                ItemType.GunCom45 => 2.6f,
                ItemType.GunRevolver => 1.8f,
                _ => 1f
            };
        }
    }
}
