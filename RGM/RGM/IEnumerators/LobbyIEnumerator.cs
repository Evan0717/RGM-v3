using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Exiled.API.Features;
using static RGM.Variables.Variable;
using RGM.API.Features;

namespace RGM.IEnumerators;

public static class LobbyIEnumerator
{
    public static IEnumerator<float> GameStartButton()
    {
        var remainingPress = 10;
        var buttonPressed = false;
        Transform redObject = null;

        while (!buttonPressed)
        {
            if (!FreezeGameStart)
            {
                var pressing = false;

                foreach (var player in PlayerManager.List)
                {
                    if (player == null || !player.IsConnected)
                        continue;

                    if (!Physics.Raycast(player.Position, Vector3.down, out var hit, 1f, (LayerMask)1)) continue;
                    if (hit.transform.name != "GameStartRed") continue;
                    if (PlayerManager.List.Count() > 1 && remainingPress <= 0) buttonPressed = true;

                    redObject = hit.transform;
                    pressing = true;

                    remainingPress -= 1;

                    redObject.position = new Vector3(redObject.position.x, redObject.position.y - 0.015f,
                        redObject.transform.position.z);
                }

                if (!pressing)
                    if (remainingPress < 10)
                    {
                        remainingPress += 1;

                        redObject.position = new Vector3(redObject.transform.position.x,
                            redObject.transform.position.y + 0.015f, redObject.transform.position.z);
                    }
            }

            yield return Timing.WaitForSeconds(0.1f);
        }

        if (SelectMode == "RandomSelect")
            Tools.PickModes();

        foreach (var player in Player.List.Where(x => !x.IsDnd()))
        {
            player.ClearInventory();
            player.Role.Set(RoleTypeId.Spectator);
        }

        Round.Start();
    }

    public static IEnumerator<float> ModeResetButton()
    {
        float remainingPress = 15;
        var buttonPressed = false;
        Transform redObject = null;

        while (!buttonPressed)
        {
            var pressing = false;
            var stack = 0;

            foreach (var player in PlayerManager.List)
            {
                if (player == null || !player.IsConnected)
                    continue;

                if (!Physics.Raycast(player.Position, Vector3.down, out var hit, 1f, (LayerMask)1)) continue;
                if (hit.transform.name != "ModeResetRed") continue;
                stack += 1;
                redObject = hit.transform;
                pressing = true;
            }

            if (PlayerManager.List.Count > 1)
            {
                const float maxPlayers = 30;
                var pressMultiplier = Math.Max(1, maxPlayers / Server.PlayerCount);
                var pressAmount = 0.012f * pressMultiplier;

                if (remainingPress <= 0)
                    buttonPressed = true;

                if (pressing)
                {
                    remainingPress -= pressAmount * stack;

                    redObject.position = new Vector3(redObject.position.x,
                        redObject.position.y - 0.00012f * stack * pressMultiplier,
                        redObject.transform.position.z);
                }
            }

            yield return Timing.WaitForSeconds(0.1f);
        }

        Tools.PickModes();
        Tools.MessageTranslated("",
            $"<mark=#ffff00aa><color=#000000><color=#ffffff>모드 투표 리스트</color>가 초기화되었습니다.</color></mark>");

        FreezeGameStart = true;

        yield return Timing.WaitForSeconds(10f);

        FreezeGameStart = false;
    }
}