using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using InventorySystem.Items.Firearms.Modules;
using MEC;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API.Features;
using RGM.API.Interfaces;
using UnityEngine;

using static RGM.Variables.ServerManagers;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Buy : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string input = string.Join(" ", arguments);
            Player player = Player.Get(sender);

            if (!ShopCooldown.Contains(player))
            {
                ShopCooldown.Add(player);

                Timing.CallDelayed(5f, () => ShopCooldown.Remove(player));

                if (Products.Select(x => x.Name).Contains(input))
                {
                    Product product = Products.FirstOrDefault(x => x.Name == input);

                    if (product.Check(player))
                    {
                        int rp = int.Parse(UsersManager.UsersCache[player.UserId][1]);

                        if (product.Price <= rp)
                        {
                            UsersManager.UsersCache[player.UserId][1] = $"{rp - product.Price}";
                            UsersManager.SaveUsers();

                            try
                            {
                                product.Script.Invoke(player);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }

                            response = "구매 완료!";
                            return true;
                        }
                        else
                        {
                            response = "코인이 부족합니다.";
                            return false;
                        }
                    }
                    else
                    {
                        response = "구매 조건에 맞지 않습니다. 설명을 제대로 읽어보세요.";
                        return false;
                    }
                }
                else
                {
                    response = $"\n<b>[상점 품목 목록]</b>\n\n{string.Join("\n", Products.Select(x => $"{x.Name}(${x.Price}) - {x.Description}"))}\n\n구매하려면 [.구매 (품목 이름)]을 입력합니다.";
                    return false;
                }
            }
            else
            {
                response = "쿨다운 중입니다. 조금 있다가 이용해주세요.";
                return false;
            }
            
        }

        public string Command { get; } = "구매";

        public string[] Aliases { get; } = { "상점" };

        public string Description { get; } = "[RGM] 상점을 확인해보세요. (구매하려면 이름을 매개 변수에 입력합니다.)";

        public bool SanitizeResponse { get; } = true;
    }
}