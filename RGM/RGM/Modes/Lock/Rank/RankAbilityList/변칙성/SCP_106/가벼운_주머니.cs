using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;
using UnityEngine;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("가벼운 주머니", "한방에 차원 주머니로 보내는 대신, 차원 주머니 탈출 확률이 50%로 조정됩니다.", RankAbilityType.가벼운_주머니, RankCategory.SCP_106, RankAbilityCategory.변칙성, "🎒")]
    public class 가벼운_주머니 : RankAbility
    {
        private List<Player> _addedPlayers = [];
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += OnFailingEscapePocketDimension;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= OnFailingEscapePocketDimension;
        }

        void OnHurting(HurtingEventArgs ev)
        {
            if (_addedPlayers.Contains(ev.Player)) return;
            if (ev.Attacker == Owner)
            {
                ev.Player.AddEffect(EffectType.PocketCorroding, 1);
                _addedPlayers.Add(ev.Player);
            }
        }

        void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
        {
            if (Random.Range(1, 3) != 1) return;
            ev.IsAllowed = false;

            ev.Player.RemoveEffect(EffectType.PocketCorroding, 1);
            ev.Player.Position = Player.List.GetRandomValue(x => x.IsAlive && x != ev.Player).Position;
            if (_addedPlayers.Contains(ev.Player))
                _addedPlayers.Remove(ev.Player);
        }
        
        private void OnDying(DyingEventArgs ev)
        {
            if (_addedPlayers.Contains(ev.Player))
                _addedPlayers.Remove(ev.Player);
        }
    }
}
