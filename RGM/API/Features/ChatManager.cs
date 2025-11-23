using Exiled.API.Features;
using MEC;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.ServerManagers;

namespace RGM.API.Features
{
    public static class ChatManager
    {
        public static IEnumerator<float> RunChat()
        {
            while (!Round.IsEnded)
            {
                foreach (var chat in Chats)
                {
                    LabApi.Features.Wrappers.TextToy text = Texts[chat.Key];

                    text.TextFormat = $"<size=1>{string.Join("\n", Chats[chat.Key].Select(x => $"{chat.Key.DisplayNickname}: <noparse>{x.Replace("</noparse>", "")}</noparse>"))}</size>";
                    text.Transform.localPosition = new Vector3(0, ((chat.Key.IsScp && !new List<RoleTypeId> { RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.Scp3114 }.Contains(chat.Key.Role.Type)) ? 1.1f : 0.8f) + 0.033f * Chats[chat.Key].Count(), 0);
                    
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
