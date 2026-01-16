using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using InventorySystem.Items.Firearms.Modules;
using MEC;

using PlayerRoles;
using RGM.API.Features;
using RGM.API.Interfaces;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Use : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string[] input = string.Join(" ", arguments).Split('/');
            Player player = Player.Get(sender);

            if (!ShopCooldown.Contains(player))
            {
                ShopCooldown.Add(player);

                Timing.CallDelayed(2f, () => ShopCooldown.Remove(player));

                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (Products.Select(x => x.Name).Contains(input[0]))
                {
                    if (uc[18].Contains(input[0]))
                    {
                        Product product = Products.FirstOrDefault(x => x.Name == input[0]);

                        if (input.Count() == 1)
                        {
                            response = $"사용은 반드시 <아이템 이름>/{{매개 변수}} 양식을 지켜주십시오.\n\n<b>[보유한 아이템]</b>\n\n{string.Join("\n", uc[18].Split('/').GroupBy(item => item).Select(group => group.Count() > 1 ? $"{group.Key} x{group.Count()}" : group.Key))}\n\n사용하려면 [.사용 <품목 이름>/<매개 변수>]을(를) 입력합니다.";
                            return false;
                        }
                        else
                        {
                            if (product.Check(player, input[1]))
                            {
                                int maxtries = 10;
                                int tries = 0;

                                while (maxtries > tries)
                                {
                                    tries++;

                                    try
                                    {
                                        product.Script.Invoke(player, $"{input[1]}");
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(ex);
                                        continue;
                                    }
                                }

                                List<string> strings = uc[18].Split('/').ToList();
                                strings.Remove(input[0]);
                                UsersManager.UsersCache[player.UserId][18] = string.Join("/", strings);
                                UsersManager.SaveUsers();

                                foreach (var p in PlayerManager.List.Where(x => x.IsDead || Vector3.Distance(x.Position, player.Position) < 11))
                                    p.AddBroadcast(5, $"<size=20>{Tools.BadgeFormat(player)}<color={player.Role.Color.ToHex()}>{player.DisplayNickname}</color>(이)가 {product.Name}(을)를 사용하였습니다.</size>");

                                Log.Info($"🔑 사용ㅣ{player.Nickname}(`{player.Id}`, `{player.UserId}`, `{player.IPAddress}`) -> {product.Name} {product.Price}");

                                response = $"<b>{product.Name}</b> 사용 완료!";
                                return true;
                            }
                            else
                            {
                                response = "사용 조건에 맞지 않습니다. 설명을 제대로 읽어보세요.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        response = $"보유하지 않은 아이템입니다.\n\n<b>[보유한 아이템]</b>\n\n{string.Join("\n", uc[18].Split('/').GroupBy(item => item).Select(group => group.Count() > 1 ? $"{group.Key} x{group.Count()}" : group.Key))}\n\n사용하려면 [.사용 <품목 이름>/<매개 변수>]을(를) 입력합니다.";
                        return false;
                    }
                }
                else
                {
                    response = $"존재하지 않는 아이템입니다.\n\n<b>[보유한 아이템]</b>\n\n{string.Join("\n", uc[18].Split('/').GroupBy(item => item).Select(group => group.Count() > 1 ? $"{group.Key} x{group.Count()}" : group.Key))}\n\n사용하려면 [.사용 <품목 이름>/<매개 변수>]을(를) 입력합니다.";
                    return false;
                }
            }
            else
            {
                response = "쿨다운 중입니다. 조금 있다가 이용해주세요.";
                return false;
            }
        }

        public string Command { get; } = "사용";

        public string[] Aliases { get; } = { "use" };

        public string Description { get; } = "[RGM] 보유한 아이템을 사용해보세요.";

        public bool SanitizeResponse { get; } = true;
    }
}