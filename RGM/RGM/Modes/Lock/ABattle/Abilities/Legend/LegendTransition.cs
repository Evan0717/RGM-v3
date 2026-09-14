namespace RGM.Modes.Abilities.Legend;

[Ability("상급 변이", """
                  다음 능력 선택창에서 <color=#DF0101>신화</color> 능력 등장 확률이 25%로 조정됩니다.
                  추가 모드 [잔칫상] 활성화 시, 확률이 추가로 15%p 증가합니다.
                  """, 
    AbilityCategory.Legend, AbilityType.LEGEND_TRANSITION, RoleAbility.None, true)]
public class LegendTransition : Ability;
