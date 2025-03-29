using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;

namespace RGM.Modes.Abilities.Mythic;

[Ability("노클립", "[ALT]ㅣ유저와 붙어있지 않으면 초당 체력이 5%씩 감소하는 대신, 노클립을 사용할 수 있습니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_NOCLIP)]
public class Noclip : Ability
{
    CoroutineHandle _noclip;

    public override void OnEnabled()
    {
        _noclip = Timing.RunCoroutine(noclip());

        Owner.IsNoclipPermitted = true;
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_noclip);

        Owner.IsNoclipPermitted = false;
    }

    public IEnumerator<float> noclip()
    {
        while (true)
        {
            if (Owner.Role is FpcRole role)
            {
                if (role.IsNoclipEnabled)
                {
                    if (Tools.TryGetNearestPlayer(Owner, out Player nearestPlayer, out float radius))
                    {
                        if (radius > 10f)
                            Owner.Hurt(Owner.MaxHealth / 20, "육체가 신의 힘을 감당해내지 못했습니다.");
                    }
                }
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}