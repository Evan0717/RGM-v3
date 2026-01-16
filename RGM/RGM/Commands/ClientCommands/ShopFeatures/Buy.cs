using System;
using System.Collections.Generic;
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
    public class Buy : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string[] input = string.Join(" ", arguments).Split('/');
            Player player = Player.Get(sender);

            if (!ShopCooldown.Contains(player))
            {
                ShopCooldown.Add(player);

                Timing.CallDelayed(2f, () => ShopCooldown.Remove(player));

                if (Products.Select(x => x.Name).Contains(input[0]))
                {
                    Product product = Products.FirstOrDefault(x => x.Name == input[0]);

                    if (input.Count() == 1)
                    {
                        int rc = int.Parse(UsersManager.UsersCache[player.UserId][1]);

                        if (!product.IsPubliced)
                        {
                            response = "해당 품목은 구매할 수 없습니다.";
                            return false;
                        }

                        if (product.Price <= rc)
                        {
                            var ownedProducts = UsersManager.UsersCache[player.UserId][18];
                            int ownedCount = ownedProducts == "0" ? 0 : ownedProducts.Split('/').Length;

                            if (ownedCount >= 10)
                            {
                                response = "한 번에 10개를 초과하여 구매할 수 없습니다.";
                                return false;
                            }

                            UsersManager.UsersCache[player.UserId][1] = $"{rc - product.Price}";
                            if (ownedProducts == "0")
                                UsersManager.UsersCache[player.UserId][18] = input[0];
                            else
                                UsersManager.UsersCache[player.UserId][18] = string.Join("/", ownedProducts.Split('/').Append(input[0]));
                            UsersManager.SaveUsers();

                            foreach (var p in PlayerManager.List.Where(x => x.IsDead || Vector3.Distance(x.Position, player.Position) < 11))
                                p.AddBroadcast(5, $"<size=20>{Tools.BadgeFormat(player)}<color={player.Role.Color.ToHex()}>{player.DisplayNickname}</color>(이)가 {product.Name}(을)를 구매하였습니다.</size>");

                            Log.Info($"💰 구매ㅣ{player.Nickname}(`{player.Id}`, `{player.UserId}`, `{player.IPAddress}`) -> {product.Name} {product.Price}");

                            response = "구매 완료!";
                            return true;
                        }
                        else
                        {
                            response = "코인이 부족합니다.";
                            return false;
                        }
                    }
                    else if (product.Check(player, input[1]))
                    {
                        int rc = int.Parse(UsersManager.UsersCache[player.UserId][1]);

                        if (!product.IsPubliced)
                        {
                            response = "해당 품목은 구매할 수 없습니다.";
                            return false;
                        }

                        if (product.Price <= rc)
                        {
                            UsersManager.UsersCache[player.UserId][1] = $"{rc - product.Price}";
                            UsersManager.SaveUsers();

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

                            foreach (var p in PlayerManager.List.Where(x => x.IsDead || Vector3.Distance(x.Position, player.Position) < 11))
                                p.AddBroadcast(5, $"<size=20>{Tools.BadgeFormat(player)}<color={player.Role.Color.ToHex()}>{player.DisplayNickname}</color>(이)가 {product.Name}(을)를 구매하고 즉시 사용하였습니다.</size>");

                            Log.Info($"💰 구매 후 사용ㅣ{player.Nickname}(`{player.Id}`, `{player.UserId}`, `{player.IPAddress}`) -> {product.Name} {product.Price}");

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
                    response = $"\n<b>[상점 품목 목록]</b>\n\n{string.Join("\n", Products.Where(x => x.IsPubliced).Select(x => $"{x.Name}(${x.Price}) - {x.Description}"))}\n\n구매하려면 [.구매 <품목 이름>/<매개 변수>]을(를) 입력합니다.\n\n<b>[보유한 아이템]</b>\n\n{string.Join("\n", UsersManager.UsersCache[player.UserId][18].Split('/').GroupBy(item => item).Select(group => group.Count() > 1 ? $"{group.Key} x{group.Count()}" : group.Key))}";
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