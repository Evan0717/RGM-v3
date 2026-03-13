using AdminToys;
using Discord;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.API.Features.Items;
using InventorySystem.Configs;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.OverlayAnims;
using PlayerStatsSystem;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using RGM.API.Interfaces;
using RGM.Modes.SubClass;
using RGM.UserSettings;
using RGM.Variables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using UserSettings.ServerSpecific;
using Utils.Networking;
using static RGM.IEnumerators.ServerIEnumerator;
using static RGM.Variables.Variable;

namespace RGM.API.Features
{
    public static class ServerManager
    {
        public static void Setup()
        {
            Server.ExecuteCommand("rnr");

            InventoryLimits.StandardCategoryLimits[ItemCategory.SpecialWeapon] = 8;
            InventoryLimits.StandardCategoryLimits[ItemCategory.SCPItem] = 8;
            InventoryLimits.Config.RefreshCategoryLimits();

            foreach (var data in CandyDataDict)
                Scp330Candies.DictionarizedCandies.Add(data.Key, data.Value);

            GlobalPlayer = AudioPlayer.CreateOrGet($"Global AudioPlayer", condition: (ReferenceHub hub) =>
            {
                return !MuteBGMPlayers.Contains(Player.Get(hub));
            }, onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000);
            });

            Timing.RunCoroutine(InputCooldown());
            Timing.RunCoroutine(SyncSpectatedHint());
            Timing.RunCoroutine(RenewalPlayersInfo());
            Timing.RunCoroutine(HintManager.OnStarted());
            Timing.RunCoroutine(HintManager.RemoveHint());
            Timing.RunCoroutine(ChatManager.RunChat());
        }
    }
}
