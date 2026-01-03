using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
using MEC;
using MultiBroadcast.API;
using ProjectMER.Features.Objects;
using Respawning;
using Respawning.Waves;
using RGM.API.Components;
using RGM.API.DataBases;
using RGM.API.Features;
using RGM.API.Interfaces;
using RGM.Modes;
using RGM.Patches;
using RGM.UserSettings;
using RGM.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UserSettings.ServerSpecific;
using static RGM.EventArgs.MEREvents;
using static RGM.EventArgs.PlayerEvents;
using static RGM.EventArgs.Scp079Events;
using static RGM.EventArgs.Scp244Events;
using static RGM.EventArgs.Scp330Events;
using static RGM.EventArgs.Scp1509Events;
using static RGM.EventArgs.ServerEvents;
using static RGM.EventArgs.WarheadEvents;
using static RGM.Variables.Variable;

namespace RGM
{
    public class RGM : Plugin<Config>
    {
        public static RGM Instance;

        public override string Name => "RGM";
        public override string Author => "GoldenPig1205";
        public override Version Version { get; } = new(3, 20, 11);
        public override Version RequiredExiledVersion { get; } = new(1, 2, 0, 5);

        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();

            ModeList = new Dictionary<ModeType, ModeData>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var modeAttribute = type.GetCustomAttribute<ModeAttribute>();

                if (modeAttribute == null)
                    continue;

                if (!typeof(Mode).IsAssignableFrom(type))
                    continue;

                try
                {
                    var mode = (Mode)Activator.CreateInstance(type);

                    ModeList.Add(modeAttribute.Type, new ModeData
                    {
                        Category = modeAttribute.Category,
                        Info = modeAttribute.Info,
                        Type = modeAttribute.Type,
                        Author = mode.Author,
                        Name = mode.Name,
                        Description = mode.Description,
                        Detail = mode.Detail,
                        Color = mode.Color,
                        Suggester = mode.Suggester,
                        Map = mode.Map
                    });
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to create an instance of mode {type.Name}: {ex}");
                }
            }

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Server.RespawnedTeam += OnRespawnedTeam;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.SpawningRagdoll += OnSpawningRagdoll;
            Exiled.Events.Handlers.Player.SpawnedRagdoll += OnSpawnedRagdoll;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
            Exiled.Events.Handlers.Player.DroppedAmmo += OnDroppedAmmo;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Kicking += OnKicking;
            Exiled.Events.Handlers.Player.Banning += OnBanning;
            Exiled.Events.Handlers.Player.ChangingGroup += OnChangingGroup;
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
            Exiled.Events.Handlers.Player.DamagingShootingTarget += OnDamagingShootingTarget;

            Exiled.Events.Handlers.Warhead.Detonating += OnDetonating;

            Exiled.Events.Handlers.Scp1509.Resurrecting += OnResurrecting;

            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;
            Exiled.Events.Handlers.Scp330.EatingScp330 += OnEatingScp330;

            Exiled.Events.Handlers.Scp244.UsingScp244 += OnUsingScp244;
            Exiled.Events.Handlers.Scp244.OpeningScp244 += OnOpeningScp244;

            Exiled.Events.Handlers.Scp079.Recontained += OnRecontained;

            ProjectMER.Events.Handlers.Schematic.SchematicSpawned += OnSchematicSpawned;
            ProjectMER.Events.Handlers.Schematic.SchematicDestroyed += OnSchematicDestroyed;

            ServerSpecificSettings.Init();

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += ServerSpecificSettings.OnSSInput;

            Harmony harmony = new Harmony($"Harmony - {DateTime.Now.Ticks}");
            harmony.Patch(AccessTools.Method(typeof(Map), nameof(Map.Broadcast), [typeof(ushort), typeof(string), typeof(Broadcast.BroadcastFlags), typeof(bool)]),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(BroadcastPostfix), nameof(BroadcastPostfix.Postfix))));
            harmony.Patch(AccessTools.Method(typeof(WaveSpawner), nameof(WaveSpawner.CanBeSpawned), [typeof(ReferenceHub)]),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(WavePostfix), nameof(WavePostfix.Postfix))));
        }
    }
}
