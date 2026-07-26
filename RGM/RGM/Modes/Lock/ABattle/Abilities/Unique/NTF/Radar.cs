using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("레이더", "가장 가까운 유기체와의 거리를 상시로 확인할 수 있습니다.", AbilityCategory.Common, AbilityType.NORMAL_NTF_RADAR, RoleAbility.NTF)]
public class Radar : Ability
{
    CoroutineHandle _radar1;

    public override void OnEnabled()
    {
        _radar1 = Timing.RunCoroutine(Radar1());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_radar1);
    }

    public IEnumerator<float> Radar1()
    {
        while (true)
        {
            if (Owner != null && Owner.IsAlive)
            {
                if (Tools.TryGetNearestPlayer(Owner, out Player nearestPlayer, out float radius))
                {
                    if (nearestPlayer != null && radius < 99999)
                        Owner.AddHint("레이더", $"<align=left><size=20><color={nearestPlayer.Role.Color.ToHex()}>{Trans.Role[nearestPlayer.Role.Type]}</color> - {radius.ToString("F1")}m</size></align>", 1.2f);
                }
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
