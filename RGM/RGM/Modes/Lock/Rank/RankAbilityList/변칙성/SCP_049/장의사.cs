using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp049;
using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("장의사", "시체 소생 가능 시간이 5초 증가합니다.", RankAbilityType.장의사, RankCategory.SCP_049, RankAbilityCategory.변칙성, "😷")]
    public class 장의사 : RankAbility
    {
        private Harmony _harmony;
        private MethodInfo _targetMethod;
        private string _harmonyId = $"장의사 - {DateTime.Now.Ticks}";

        public override void OnEnabled()
        {
            _harmony = new Harmony(_harmonyId);

            _targetMethod = AccessTools.Method(
                typeof(Scp049ResurrectAbility),
                "CheckBeginConditions"
            );

            var transpiler = new HarmonyMethod(
                typeof(Scp049ResurrectAbility_Patch),
                nameof(Scp049ResurrectAbility_Patch.Transpiler)
            );

            _harmony.Patch(_targetMethod, transpiler: transpiler);
        }

        public override void OnDisabled()
        {
            if (_targetMethod != null)
                _harmony.Unpatch(_targetMethod, HarmonyPatchType.Transpiler, _harmonyId);

            _harmony = null;
        }

        public class Scp049ResurrectAbility_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var code in instructions)
                {
                    yield return code;

                    if (code.opcode == OpCodes.Ldc_R4 && code.operand is float f && (f == 18f || f == 12f))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 5f);
                        yield return new CodeInstruction(OpCodes.Add);
                    }
                }
            }
        }
    }
}
