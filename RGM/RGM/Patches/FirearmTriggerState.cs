using System;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;

namespace RGM.Patches
{
    // 서버는 SimpleTriggerModule 이 클라이언트로부터 트리거 입력(Cmd)을 수신하지만,
    // ServerSetTrigger 는 그 값을 RPC 로 다른 클라이언트들에게 중계만 할 뿐 서버 자신의 SyncData 에는 저장하지 않는다.
    // (SendRpc 는 네트워크로만 전송되며, 데디케이트 서버는 자신이 보낸 RPC 를 스스로 수신/처리하지 않기 때문)
    // 그 결과 서버에서는 LastTriggerPress / TriggerHeld 가 항상 기본값(false/0)으로 남아 신뢰할 수 없다.
    //
    // 이 패치는 서버가 수신한 트리거 입력을 그대로 SyncData 에 반영해, 서버에서도 트리거 상태를 신뢰할 수 있게 만든다.
    // 게임 내부 발사 로직은 서버측 트리거 상태(EquipUpdate 의 IsControllable 분기)를 사용하지 않으므로 부작용이 없다.
    public static class FirearmTriggerStatePatch
    {
        private static bool _applied;

        public static void Ensure()
        {
            if (_applied)
                return;

            _applied = true;

            try
            {
                Harmony harmony = new Harmony("RGM.FirearmTriggerState");

                harmony.Patch(
                    AccessTools.Method(typeof(SimpleTriggerModule), nameof(SimpleTriggerModule.ServerSetTrigger)),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(FirearmTriggerStatePatch), nameof(ServerSetTriggerPostfix))));
            }
            catch (Exception e)
            {
                Log.Error($"[FirearmTriggerStatePatch] Failed to apply patch: {e}");
            }
        }

        public static void ServerSetTriggerPostfix(SimpleTriggerModule __instance, bool isHeld)
        {
            try
            {
                SimpleTriggerModule.GetData(__instance.ItemSerial).Set(isHeld);
            }
            catch (Exception e)
            {
                Log.Error($"[FirearmTriggerStatePatch] Exception: {e}");
            }
        }
    }
}
