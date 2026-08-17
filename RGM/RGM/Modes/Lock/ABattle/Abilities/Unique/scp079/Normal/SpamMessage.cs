using System.Linq;
using System.Collections.Generic;
using RGM.API.Features;
using PlayerRoles;
using MultiBroadcast.Commands.Subcommands;
using Exiled.API.Enums;

using Exiled.API.Extensions;

namespace RGM.Modes.Abilities.Unique.Scp079.Common;

[Ability("스팸 문자", "적들에게 스팸 문자를 보내 시야를 20초간 방해합니다.", AbilityCategory.Common, AbilityType.NORMAL_SCP079_SPAMMESSAGE, RoleAbility.Scp079)]

public class SpamMessage : Ability
{
    public static List<string> SpamMessages = new List<string>
    {
$"""
<size=25>
<b>
[안전 안내문자] 최근에 <color={ABattle.RatingColor["희귀"]}>이차원 공간 도약</color>으로 인하여 많은 실종이 발생했습니다.
낭떠러지에서 동전을 든 인간을 조심하시고, A게이트, 탄약삼거리, 폭포방, 용광로 등의 방에
접근하지 말아주세요.
또한, <color={ABattle.RatingColor["희귀"]}>이차원 공간 도약</color>은 팀킬도 할 수 있으니, 주의하시기 바랍니다.
</b>
</size>
""",
$"""
<size=30>
<b>
 <color={ABattle.RatingColor["신화"]}>
[Web발신] 휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!
휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!
휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!
휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!
휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!
휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!
휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!휴버트는 15.0 업뎃을 하라!
</color>
</b>
</size>
"""
    };
    public override void OnEnabled()
    {
        foreach (var player in PlayerManager.List.Where(p => p.IsAlive && p.LeadingTeam == Owner.LeadingTeam))
        {
            player.AddHint("스팸 메시지", $"{SpamMessages.GetRandomValue()}", 20f);
        }
    }
}