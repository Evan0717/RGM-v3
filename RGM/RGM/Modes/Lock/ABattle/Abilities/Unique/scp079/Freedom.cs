using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("자유", "이미 작동된 발전기를 제외한 나머지 발전기는 2분 간 잠깁니다.", AbilityCategory.Scp079, AbilityType.SCP079_FREEDOM)]
public class Freedom : Ability
{
    public override void OnEnabled()
    {
        IEnumerator<float> enumerator()
        {
            for (int i = 0; i < 120; i++)
            {
                foreach (var generator in Generator.List)
                {
                    if (generator.State != GeneratorState.Engaged)
                        generator.State = GeneratorState.Unlocked;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        Timing.RunCoroutine(enumerator());
    }

    public override void OnDisabled()
    {
    }
}
