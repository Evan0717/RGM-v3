namespace RGM.Modes.Abilities.Unique.Human.Mythic;

/*[Ability("이중인격",
    """
    두 가지 형상, 환멸과 연극의 모습으로 변신합니다.
    지급된 아이템을 사용하여 인격을 변경할 수 있으며, 각 인격은 특정 종류의 능력치를 크게 강화시킵니다.
    """, 
    AbilityCategory.Mythic, AbilityType.MYTHIC_HUMAN_DUALPERSONALITY, RoleAbility.Human)]*/

public class DualPersonality : Ability
{
    /*
     * 이중인격 구현
     *
     * 1. 두 가지 형상(연극/환멸) 상태를 지정. 두 형상 간 전환 쿨타임은 1초로 설정. 형상 간 전환 때 마다 생존 효과 부여.
     * 추가로, 전환 때 마다 이동 불가.(Sinkhole, Ensnared 효과 부여)
     * 2. 능력 획득 시 기본 형상은 연극 형상으로 지정.
     * 3. 형상 간 전환은 Lantern으로 하며, 해당 아이템 내장 상호작용 기능(Lantern ON/OFF)을 통해 전환 가능하도록 구현. 해당 아이템은 버릴 수 없도록 설정.
     * 3-1. Lantern ON일 때 연극의 형상, OFF 일때 환멸의 형상으로 설정. 혼동을 방지하기 위해 전환 후 1 + OneFrame초 간 아이템 상호작용을 금지함.
     * 
     * 4. 연극의 형상 능력치
     * 4-1. 0.5초 간격으로 최대 HP의 7% 회복, 회복량 만큼 AHP가 증가하며, 최대 AHP도 동시에 증가.
     * 4-2. 해당 AHP는 30초간 지속되며, 30초가 지날 시 지속적으로 감소(감소량은 게임 기본 값 사용)
     * 4-3. 이동 속도 30% 감소
     * 4-3-1. 감속을 구현할 때, 현재 자신의 이동속도 증감량을 계산하여, 해당 값에서 -40을 적용. (단순히 이펙트를 적용하는 게 아님!)
     * 이동속도 증감량은 현재 Owner의 MovementBoost 또는 Slowness의 intensity 값을 가져와서 계산.
     * 증감량을 계산하였을 때, 양수면 MovementBoost, 음수면 Slowness 적용. 0이면 아무것도 적용하지 않음.
     * 4-4. 공격력 30% 감소
     * 4-5. 공격 시 피해량의 8%를 회복함(Owner.Heal 로 적용.)
     *
     * 5. 환멸의 형상 능력치
     * 5-1. 1초 간격으로 최대 HP의 2%만큼 자가 피해를 입으며, 최소치는 최대 체력의 10%까지만 감소 가능.
     * 5-2. 이동 속도 45% 증가.(AddEffect로 추가)
     * 5-3. 공격력 45% 증가.
     * 5-4. 공격 시, 자신의 잃은 HP만큼 추가 피해를 입히며, 해당 피해는 관통 고정 피해로 적용(ApplyFixedDamage)
     * 
     */
}