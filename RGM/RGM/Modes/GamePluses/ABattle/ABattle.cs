using System;
using HarmonyLib;
using InventorySystem;
using CommandSystem;
using MEC;
using RemoteAdmin;

using static RGM.Modes.ABattleVariables.Abilities;
using static RGM.Modes.ABattleVariables.AbiltiyManagers;
using static RGM.Modes.ABattleVariables.MainManagers;
using static RGM.Modes.ABattleVariables.Cooldowns;
using static RGM.Modes.ABattleVariables.Serials;

using static RGM.Modes.ABattleFunctions.AbilityManagers;
using static RGM.Modes.ABattleFunctions.MainManagers;
using static RGM.Modes.ABattleFunctions.SpecificAbilities;

using static RGM.Modes.ABattleIEnumerators.MainManagers;
using static RGM.Modes.ABattleIEnumerators.SpecificAbilities;

using static RGM.Modes.ABattleEventArgs.PlayerEvents;
using static RGM.Modes.ABattleEventArgs.Scps.Scp0492Events;
using static RGM.Modes.ABattleEventArgs.Scps.Scp079Events;
using static RGM.Modes.ABattleEventArgs.Scps.Scp106Events;
using static RGM.Modes.ABattleEventArgs.Scps.Scp173Events;
using static RGM.Modes.ABattleEventArgs.Scps.Scp3114Events;
using static RGM.Modes.ABattleEventArgs.Scps.Scp096Events;
using static RGM.Modes.ABattleEventArgs.Scps.Scp049Events;
using static RGM.Modes.ABattleEventArgs.MapEvents;

using RGM.Modes.ABattlePatches;

using static HarmonyLib.AccessTools;
using Exiled.API.Features;

namespace RGM.Modes
{
    class ABattle
    {
        public static ABattle Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Jumping += OnJumping;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
            Exiled.Events.Handlers.Player.ChangingMicroHIDState += OnChangingMicroHIDState;
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;

            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

            Exiled.Events.Handlers.Scp049.Attacking += OnScp049Attacking;
            Exiled.Events.Handlers.Scp049.FinishingRecall += OnFinishingRecall;

            Exiled.Events.Handlers.Scp0492.ConsumedCorpse += OnConsumedCorpse;
            Exiled.Events.Handlers.Scp0492.TriggeringBloodlust += OnTriggeringBloodlust;

            Exiled.Events.Handlers.Scp096.Charging += OnCharging;

            Exiled.Events.Handlers.Scp106.Attacking += OnScp106Attacking;

            Exiled.Events.Handlers.Scp3114.Revealed += OnRevealed;

            Exiled.Events.Handlers.Scp079.GainingLevel += OnGainingLevel;
            Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
            Exiled.Events.Handlers.Scp079.ZoneBlackout += OnZoneBlackout;

            MapEditorReborn.Events.Handlers.Map.LoadingMap += OnLoadingMap;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(RequestManager());
            Timing.RunCoroutine(SynergyManager());

            Timing.RunCoroutine(UpgradeBody());
            Timing.RunCoroutine(FlashLight());
            Timing.RunCoroutine(Flamethrower());
            Timing.RunCoroutine(Blessing());
            Timing.RunCoroutine(Spirit());
            Timing.RunCoroutine(Twinkle());
            Timing.RunCoroutine(Medical());
            Timing.RunCoroutine(Radar());
            Timing.RunCoroutine(Radiation());
            Timing.RunCoroutine(StickySwamp());

            QueryProcessor.DotCommandHandler.RegisterCommand(new VoteFirst());
            QueryProcessor.DotCommandHandler.RegisterCommand(new VoteSecond());
            QueryProcessor.DotCommandHandler.RegisterCommand(new VoteThird());

            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(new AddAbility());

            Harmony harmony = new Harmony($"ABattle - {DateTime.Now.Ticks}");
            harmony.Patch(Method(typeof(Inventory), nameof(Inventory.Update)),
                transpiler: new HarmonyMethod(Method(typeof(InventoryUpdatePatch), nameof(InventoryUpdatePatch.Transpiler))));
        }
    }
}
