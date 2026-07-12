using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

//[Ability("위치 추적", "10초 간 아군이 아닌 유저의 위치를 확인합니다.", AbilityCategory.Common, AbilityType.NORMAL_FINDLOCATION)]
public class FindLocation : Ability
{
    public override void OnEnabled()
    {
        Timing.RunCoroutine(FindLocationCoroutine(Tools.GetRandomValue(PlayerManager.List.Where(x => HitboxIdentity.IsEnemy(x.ReferenceHub, Owner.ReferenceHub)).ToList())));
    }

    public override void OnDisabled()
    {
    }

    public IEnumerator<float> FindLocationCoroutine(Player target)
    {
        for (int i = 1; i < 11; i++)
        {
            Owner.AddHint("위치 추적", $"소속이 <color={target.Role.Color.ToHex()}>{target.Role.Name}</color>인 ???은(는) <b>{target.CurrentRoom.Name}</b>에 있습니다.", 1.2f);
            yield return Timing.WaitForSeconds(1f);
        }
    }
}
