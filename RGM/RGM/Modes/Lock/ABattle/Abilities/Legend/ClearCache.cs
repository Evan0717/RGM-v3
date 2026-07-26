using Exiled.API.Extensions;
using MEC;
using System.Collections.Generic;
using System.Linq;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("캐시 청소", $"2분마다 자신의 워크스테이션 업그레이드 이용 기록을 초기화합니다.", AbilityCategory.Legend, AbilityType.LEGEND_CLEARCACHE, RoleAbility.None, true)]
public class ClearCache : Ability
{
    CoroutineHandle _clearcache;
    public override void OnEnabled()
    {
        _clearcache = Timing.RunCoroutine(clearCache());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_clearcache);
    }

    public IEnumerator<float> clearCache()
    {
        while (Owner != null && Owner.IsConnected && Owner.IsAlive)
        {
           ABattle.Instance.PlayerWorkstations[Owner].Clear();

           if (Owner != null && Owner.IsConnected && Owner.IsAlive)
               Owner.AddBroadcast(10, $"<b><size=20>캐시 청소가 완료되었습니다. 이전에 방문한 워크스테이션에서 능력을 다시 얻을 수 있습니다.</size></b>");
            
            yield return Timing.WaitForSeconds(60 * 2);
        }

        yield break;
    }
}
