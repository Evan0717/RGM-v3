using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.API.Features
{
    /**
     * <summary>채팅 관련된 작업을 처리합니다</summary>
     */
    public static class ChatManager
    {
        /**
         * <summary>플레이어의 머리 위에 TextToy를 띄웁니다</summary>
         * <returns>MEC 코루틴</returns>
         */
        public static IEnumerator<float> RunChat()
        {
            while (!Round.IsEnded)
            {
                foreach (var chat in Chats)
                {
                    LabApi.Features.Wrappers.TextToy text = Texts[chat.Key];

                    text.TextFormat = $"<size=1>{string.Join("\n", Chats[chat.Key].Select(x => $"<i>{chat.Key.DisplayNickname}</i>: <noparse>{x.Replace("</noparse>", "")}</noparse>"))}</size>";
                    text.Transform.localPosition = new Vector3(0, ((chat.Key.IsScpRole() && !new List<RoleTypeId> { RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.Scp3114 }.Contains(chat.Key.Role.Type)) ? 1.1f : 0.8f) + 0.033f * Chats[chat.Key].Count(), 0);
                    
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
