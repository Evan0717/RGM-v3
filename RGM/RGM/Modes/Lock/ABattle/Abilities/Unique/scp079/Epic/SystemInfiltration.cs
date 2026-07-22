using DiscordInteraction.Discord;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using static PlayerList;

namespace RGM.Modes.Abilities.Unique.Scp079.Epic;


[Ability("시스템 침투", "20% 확률로 추가 모드를 추가합니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_SystemInfiltration, RoleAbility.Scp079)]
public class SystemInfiltration : Ability
{
    public override void OnEnabled()
    {
        TryAddExtraMode();
    }

    public override void OnDisabled()
    {

    }


    private string PickExtraMode_79()
    {
        if (ABattle.CurrentExtraModes.Count >= ABattle.ExtraModes.Count) return "";
        string extraMode = Tools.GetRandomValue(ABattle.ExtraModes.Keys.Where(x => !ABattle.CurrentExtraModes.Contains(x)).ToList());
        ABattle.CurrentExtraModes.Add(extraMode);

        Webhook.Send($"추가 모드: {extraMode}");
        Log.Info($"추가 모드: {extraMode}");

        AllPlayerBroadcast($"<size=25><b><color=#fecdcd>{extraMode}</color></b></size>\n<size=20>{ABattle.ExtraModes[extraMode]}</size>");

        if (extraMode == "캐시 청소")
            Timing.RunCoroutine(ABattle.Instance.ClearCache());

        if (extraMode == "지원")
            Timing.RunCoroutine(ABattle.Instance.Backup());

        if (extraMode == "난장판")
        {
            for (int i = 0; i < 2; i++)
                PickExtraMode_79();
        }

        return extraMode;
    }

    public void AllPlayerBroadcast(string message)
    {
        foreach (var p in PlayerManager.List)
        {
                p.AddBroadcast(10, message);
                p.SendConsoleMessage("\n" + message, "white");
        }
    }

    private void TryAddExtraMode()
    {
        Owner.AddHint("침투","시스템 침투 시도 중...");
        Timing.CallDelayed(3.5f, () =>
        {
            if (Owner.IsAlive)
            {
                if (Random.Range(1, 6) == 1)
                { 
                    string extraMode = PickExtraMode_79();
                    if (!string.IsNullOrEmpty(extraMode))
                    {
                        Owner.AddHint("침투", $"시스템 침투에 성공하여 <b>{extraMode}</b> 모드가 추가되었습니다!");
                        Owner.AddAbility(AbilityType.DUMMY_INFILTRATIONSUCCESS);
                    }
                    else
                    {
                        Owner.AddHint("침투", "시스템 침투에 성공했으나, 더 이상 추가할 모드가 없습니다.");
                        Owner.AddAbility(AbilityType.DUMMY_INFILTRATIONSUCCESS);
                    }
                }
                else
                {
                    Owner.AddHint("침투", "시스템 침투에 실패하였습니다.");
                    Owner.AddAbility(AbilityType.DUMMY_INFILTRATIONFAIL);
                }

            }

        });
    }
}