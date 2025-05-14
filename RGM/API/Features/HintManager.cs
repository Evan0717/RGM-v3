using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.API.Features
{
    public static class HintManager
    {
        private static Dictionary<Player, Dictionary<string, (string, float)>> _playerHints = new(); // Custom ID, (힌트, 남은 시간)

        public static IEnumerator<float> OnStarted()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive && _playerHints.ContainsKey(x) && _playerHints[x].Count > 0))
                {
                    player.ShowHint($"{string.Join("\n", _playerHints[player].Values.Select(x => x.Item1))}", 1.2f);
                }
                
                yield return Timing.WaitForSeconds(1);
            }
        }

        public static IEnumerator<float> RemoveHint()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive && _playerHints.ContainsKey(x)))
                {
                    foreach (var hint in _playerHints[player].ToList())
                    {
                        if (hint.Value.Item2 <= 0)
                        {
                            _playerHints[player].Remove(hint.Key);
                        }
                        else
                        {
                            _playerHints[player][hint.Key] = (hint.Value.Item1, hint.Value.Item2 - 0.1f);
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }
        
        public static void AddHint(this Player player, string customId, string hint, float duration = 3)
        {
            if (!_playerHints.ContainsKey(player))
                _playerHints[player] = new Dictionary<string, (string, float)> { };

            if (_playerHints[player].ContainsKey(customId))
            {
                _playerHints[player][customId] = (hint, duration);
            }
            else
            {
                _playerHints[player].Add(customId, (hint, duration));
            }
        }
    }
}