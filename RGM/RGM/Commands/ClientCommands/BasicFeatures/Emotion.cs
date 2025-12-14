using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MultiBroadcast.API;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using RGM.API;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ChangeEmotion : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            try
            {
                if (EmotionCooldown.Contains(player))
                {
                    response = en ? "You are sending input at too fast an interval!" : "너무 빠른 간격으로 입력을 보내고 있습니다!";
                    return false;
                }
                else if (player.IsHuman)
                {
                    if (int.TryParse(arguments.At(0), out int num))
                    {
                        player.Emotion = (EmotionPresetType)(num - 1);

                        response = en ? "Successfully changed emotions." : $"감정을 성공적으로 변경했습니다.";
                        return true;
                    }
                    else
                    {
                        response = en ? $"Please enter between 1 and 7.\n\n{string.Join("\n", Tools.EnumToList<EmotionPresetType>())}" : $"1~7번 사이에서 입력해주세요.\n\n{string.Join("\n", Tools.EnumToList<EmotionPresetType>())}";
                        return false;
                    }
                }
                else if (player.Role is Scp3114Role scp3114)
                {
                    if (int.TryParse(arguments.At(0), out int num))
                    {
                        scp3114.StartDancing((DanceType)(num - 1));

                        response = en ? "The dance started successfully." : $"댄스를 성공적으로 시작했습니다.";
                        return true;
                    }
                    else
                    {
                        response = en ? $"Please enter between 1 and 7.\n\n{string.Join("\n", Tools.EnumToList<DanceType>())}" : $"1~7번 사이에서 입력해주세요.\n\n{string.Join("\n", Tools.EnumToList<DanceType>())}";
                        return false;
                    }
                }
                else
                {
                    response = en ? "This command is for human use only." : "인간만 사용 가능한 명령어입니다.";
                    return false;
                }
            }
            catch
            {
                if (player.IsHuman)
                {
                    player.Emotion = (EmotionPresetType)UnityEngine.Random.Range(0, 7);

                    response = en ? "Successfully changed emotions." : $"감정을 성공적으로 변경했습니다.";
                    return true;
                }
                else if (player.Role is Scp3114Role scp3114)
                {
                    scp3114.StartDancing((DanceType)UnityEngine.Random.Range(0, 7));

                    response = en ? "The dance started successfully." : $"댄스를 성공적으로 시작했습니다.";
                    return true;
                }
                else
                {
                    response = en ? "This command is for human use only." : "인간만 사용 가능한 명령어입니다.";
                    return false;
                }
            }
        }

        public string Command { get; } = "emotion";

        public string[] Aliases { get; } = { "감정표현", "댄스", "감정" };

        public string Description { get; } = en ? "[RGM] Make it easier to express your emotions." : "[RGM] 감정 표현을 더 쉽게 하세요.";

        public bool SanitizeResponse { get; } = true;
    }
}
