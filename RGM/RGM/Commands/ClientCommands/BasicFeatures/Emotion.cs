using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MultiBroadcast.API;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using RGM.API;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ChangeEmotion : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player.IsHuman)
            {
                if (int.TryParse(arguments.At(0), out int num))
                {
                    try
                    {
                        player.Emotion = (EmotionPresetType)(num - 1);

                        response = $"감정을 성공적으로 변경했습니다.";
                        return true;
                    }
                    catch
                    {
                        response = $"1~7번 사이에서 입력해주세요.\n\n{string.Join("\n", Tools.EnumToList<EmotionPresetType>())}";
                        return false;
                    }
                }
                else
                {
                    response = $"1~7번 사이에서 입력해주세요.\n\n{string.Join("\n", Tools.EnumToList<EmotionPresetType>())}";
                    return false;
                }
            }
            else
            {
                response = "인간만 사용 가능한 명령어입니다.";
                return false;
            }
        }

        public string Command { get; } = "감정";

        public string[] Aliases { get; } = { "감정표현" };

        public string Description { get; } = "[RGM] 감정 표현을 더 쉽게 하세요.";

        public bool SanitizeResponse { get; } = true;
    }
}
