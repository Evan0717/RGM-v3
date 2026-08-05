namespace RGM.Modes.Abilities.Epic;

[Ability("초재생", "안티 콜라를 지급받습니다. 20% 확률로 추가 획득합니다.", AbilityCategory.Epic, AbilityType.EPIC_ANTISCP207)]
public class AntiScp207 : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.AntiSCP207);
        if (UnityEngine.Random.Range(1, 101) <= 20) Owner.AddItem(ItemType.AntiSCP207);
    }
}
