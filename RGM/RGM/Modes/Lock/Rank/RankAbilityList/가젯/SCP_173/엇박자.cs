using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Scp173;
using MEC;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("엇박자", "다음 이동 쿨타임을 즉시 1초로 조정합니다.", RankAbilityType.엇박자, RankCategory.SCP_173, "🎶", 60)]
    public class 엇박자 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            if (Owner.Role is Scp173Role scp173)
            {
                float originCooldown = scp173.BlinkCooldown;

                scp173.BlinkCooldown = 1;

                Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

                void OnBlinking(BlinkingEventArgs ev)
                {
                    if (ev.Player == Owner)
                    {
                        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                        {
                            scp173.BlinkCooldown = originCooldown;
                        });
                    }

                    Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;
                }
            }
        }
    }
}
