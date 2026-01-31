using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RGM.API;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VoiceChat.Codec;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.RandomBlockMode)]
    public class RandomBlockMode : Mode
    {
        public override string Name => "랜덤금지모드";
        public override string Description => "절대로 금지된 행동을 해선 안됩니다!";
        public override string Detail =>
"""
5 ~ 180초 사이에 플레이어마다 금지된 행동이 변경됩니다.
금지된 행동이 고지되기 전에 2초 간 경고 시간이 주어집니다.

금지된 행동을 하면 귀여워질 수 있습니다.
""";
        public override string Color => "d97053";

        CoroutineHandle _onModeStarted;
        CoroutineHandle _check;

        Dictionary<Player, BlockedActions> dict = new();
        Dictionary<Player, Vector3> pos_dict = new();

        enum BlockedActions
        {
            달리기,
            점프,
            공격,
            말하기,
            아이템_사용,
            문_상호작용,
            발전기_열기,
            카드키_들기,
            총_들기,
            의료_아이템_들기,
            천천히_걷기,
            탈출하기,
            움직이기,
        }

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Jumping += OnJumping;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
            Exiled.Events.Handlers.Player.UsingItem += OnUsedItem;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.OpeningGenerator += OnOpeningGenerator;
            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _check = Timing.RunCoroutine(check());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Jumping -= OnJumping;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;
            Exiled.Events.Handlers.Player.UsingItem -= OnUsedItem;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.OpeningGenerator -= OnOpeningGenerator;
            Exiled.Events.Handlers.Player.ChangingItem -= OnChangingItem;
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_check);
        }

        IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var room in Room.List)
                    room.Color = UnityEngine.Color.red;

                GlobalPlayer.TryPlay("출석 체크", 1.5f);

                yield return Timing.WaitForSeconds(2);

                foreach (var room in Room.List)
                    room.ResetColor();

                ushort time = (ushort)UnityEngine.Random.Range(5, 181);

                foreach (var player in PlayerManager.List)
                {
                    if (!dict.ContainsKey(player))
                        dict.Add(player, BlockedActions.달리기);

                    var blockedAction = Tools.EnumToList<BlockedActions>().GetRandomValue();
                    dict[player] = blockedAction;

                    player.AddBroadcast(time, $"<size=30>당신은 <color=red>{blockedAction.ToString().Replace("_", " ")}</color>(을)를 할 수 없습니다.</size>");
                }

                yield return Timing.WaitForSeconds(time);
            }
        }

        IEnumerator<float> check()
        {
            while (true)
            {
                foreach (var player in dict.Keys)
                {
                    var blockedAction = dict[player];
                    FirstPersonMovementModule fpcModule = (player.ReferenceHub.roleManager.CurrentRole as FpcStandardRoleBase).FpcModule;

                    if (blockedAction == BlockedActions.달리기)
                    {
                        if (fpcModule.CurrentMovementState == PlayerMovementState.Sprinting)
                            player.ExplodeGrenade(ignore: true);
                    }

                    if (blockedAction == BlockedActions.천천히_걷기)
                    {
                        if (fpcModule.CurrentMovementState == PlayerMovementState.Sneaking)
                            player.ExplodeGrenade(ignore: true);
                    }

                    if (blockedAction == BlockedActions.움직이기)
                    {
                        if (!pos_dict.ContainsKey(player))
                            pos_dict.Add(player, player.Position);

                        if (Vector3.Distance(pos_dict[player], player.Position) > 0.1f)
                            player.ExplodeGrenade(ignore: true);

                        pos_dict[player] = player.Position;
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        void OnJumping(JumpingEventArgs ev)
        {
            if (dict.ContainsKey(ev.Player) && dict[ev.Player] == BlockedActions.점프)
                ev.Player.ExplodeGrenade(ignore: true);
        }

        void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && dict.ContainsKey(ev.Attacker) && dict[ev.Attacker] == BlockedActions.공격)
                ev.Attacker.ExplodeGrenade(ignore: true);
        }
        
        void OnVoiceChatting(VoiceChattingEventArgs ev)
        {
            if (dict.ContainsKey(ev.Player) && dict[ev.Player] == BlockedActions.말하기)
                ev.Player.ExplodeGrenade(ignore: true);
        }

        void OnUsedItem(UsingItemEventArgs ev)
        {
            if (dict.ContainsKey(ev.Player) && dict[ev.Player] == BlockedActions.아이템_사용)
                ev.Player.ExplodeGrenade(ignore: true);
        }

        void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (dict.ContainsKey(ev.Player) && dict[ev.Player] == BlockedActions.문_상호작용)
                ev.Player.ExplodeGrenade(ignore: true);
        }

        void OnOpeningGenerator(OpeningGeneratorEventArgs ev)
        {
            if (dict.ContainsKey(ev.Player) && dict[ev.Player] == BlockedActions.발전기_열기)
                ev.Player.ExplodeGrenade(ignore: true);
        }

        void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (dict.ContainsKey(ev.Player)) 
            {
                if (dict[ev.Player] == BlockedActions.카드키_들기 && ev.Item.Type.IsKeycard())
                    ev.Player.ExplodeGrenade(ignore: true);

                if (dict[ev.Player] == BlockedActions.총_들기 && ev.Item.Type.IsWeapon())
                    ev.Player.ExplodeGrenade(ignore: true);

                if (dict[ev.Player] == BlockedActions.의료_아이템_들기 && ev.Item.Type.IsMedical())
                    ev.Player.ExplodeGrenade(ignore: true);
            }
        }

        void OnEscaping(EscapingEventArgs ev)
        {
            if (dict.ContainsKey(ev.Player) && dict[ev.Player] == BlockedActions.탈출하기)
                ev.Player.ExplodeGrenade(ignore: true);
        }
    }
}
