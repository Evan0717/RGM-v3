namespace RGM.Modes.Abilities.Dummy;

[Ability("개인 정보 유출", "<color=#FF0000>SCP-079</color>에게 능력을 탈취당했습니다.", AbilityCategory.Dummy, AbilityType.DUMMY_INFORMATIONLEAK)]
public class InformationLeak : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}