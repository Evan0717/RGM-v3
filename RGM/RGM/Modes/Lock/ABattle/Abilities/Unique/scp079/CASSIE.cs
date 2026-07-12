namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("C.A.S.S.I.E.", "서버 내의 모두에게 말을 전달할 수 있는 [.캐시 (할 말}] 명령어 사용권을 1회 추가합니다.", AbilityCategory.Scp079, AbilityType.SCP079_CASSIE)]
public class CASSIE : Ability
{
    public override void OnEnabled()
    {
        if (ABattleVar.CASSIE.ContainsKey(Owner))
            ABattleVar.CASSIE[Owner]++;

        else
            ABattleVar.CASSIE.Add(Owner, 1);
    }

    public override void OnDisabled()
    {
    }
}
