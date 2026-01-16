using AdminToys;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;

using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Serializable;
using RGM.API;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using static PlayerList;
using static RGM.Variables.Variable;
using static UnityEngine.GraphicsBuffer;

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Split : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            List<Player> TeamA = new List<Player>();
            List<Player> TeamB = new List<Player>();


            var players = PlayerManager.List.Where(x => !x.IsTutorial).ToList();
            players.ShuffleList();

            int halfCount = players.Count / 2;

            TeamA = players.Take(halfCount).ToList();
            TeamB = players.Skip(halfCount).ToList();

            foreach (var ply in TeamA)
            {
                ply.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
            }

            foreach (var ply in TeamB)
            {
                ply.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
                ply.ClearInventory();
            }

            response = $"편을 갈랐습니다. (D계급 - {TeamA.Count()} / 과학자 - {TeamB.Count()})";

            return true;
        }

        public string Command { get; } = "편가르기";

        public string[] Aliases { get; } = { "팀매칭" };

        public string Description { get; } = "튜토리얼을 제외한 나머지가 반으로 갈라집니다. (과학자, 죄수)";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Roulette : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            string args = string.Join(" ", arguments).Replace($"{arguments.At(0)} ", "");

            List<string> poll = arguments.At(0) == "player" ? Player.List.Select(x => x.DisplayNickname).ToList() : arguments.At(0).Split('/').ToList();

            int spinCount = UnityEngine.Random.Range(30, 55);

            IEnumerator<float> roulette()
            {
                string result = "";

                int fastSpins = (int)(spinCount * 0.5f);
                int midSpins = (int)(spinCount * 0.3f);
                int slowSpins = spinCount - fastSpins - midSpins;

                float fastDelay = 0.05f;
                float midDelay = 0.15f;
                float slowDelay = 0.35f;

                MultiBroadcast.API.MultiBroadcast.AddMapBroadcast(20, "test", tag: "룰렛");

                // 빠른 회전
                for (int i = 0; i < fastSpins; i++)
                {
                    string spinValue = poll.GetRandomValue();
                    result = spinValue;

                    foreach (var player in Player.List)
                        player.EditBroadcast($"{args}\n<size=25><color=#FFD700>룰렛 돌리는 중...</color> <b>{spinValue}</b></size>", tag: "룰렛");

                    yield return Timing.WaitForSeconds(fastDelay);
                }

                // 중간 속도 회전
                for (int i = 0; i < midSpins; i++)
                {
                    string spinValue = poll.GetRandomValue();
                    result = spinValue;

                    foreach (var player in Player.List)
                        player.EditBroadcast($"{args}\n<size=25><color=#FFD700>룰렛 돌리는 중...</color> <b>{spinValue}</b></size>", tag: "룰렛");

                    yield return Timing.WaitForSeconds(midDelay);
                }

                // 느린 회전 (박진감)
                for (int i = 0; i < slowSpins; i++)
                {
                    string spinValue = poll.GetRandomValue();
                    result = spinValue;

                    foreach (var player in Player.List)
                        player.EditBroadcast($"{args}\n<size=25><color=#FFD700>룰렛 돌리는 중...</color> <b>{spinValue}</b></size>", tag: "룰렛");

                    yield return Timing.WaitForSeconds(slowDelay + (i * 0.1f));
                }

                MultiBroadcast.API.MultiBroadcast.EditBroadcast($"{args}\n<size=25><b><color=#00FF00>룰렛 결과: {result}</color></b></size>", tag: "룰렛");
            }

            Timing.RunCoroutine(roulette());

            response = "룰렛을 시작합니다!";

            return true;
        }

        public string Command { get; } = "룰렛";

        public string[] Aliases { get; } = { "뽑기" };

        public string Description { get; } = "룰렛을 돌립니다. 결과는 모두가 확인할 수 있습니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
