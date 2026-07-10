namespace RGM.Modes.Abilities.Epic;

[Ability("럭키비키", "이전에 방문했던 워크스테이션에서 다시 한번 더 능력을 획득할 수 있습니다.", AbilityCategory.Epic, AbilityType.EPIC_LUCKYVIKEY)]
public class LuckyVicky : Ability
{
    public override void OnEnabled()
    {
        ABattle.Instance.PlayerWorkstations[Owner].Clear();
    }

    public override void OnDisabled()
    {
    }
}
