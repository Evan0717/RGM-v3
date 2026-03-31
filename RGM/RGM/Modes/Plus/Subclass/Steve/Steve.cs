using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp173;
using HarmonyLib;
using MEC;
using Mirror;

using PlayerRoles;
using PlayerStatsSystem;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using RGM.API.Features;
using RGM.Commands.RemoteAdminCommands;
using RGM.UserSettings;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes.SubClass
{
    public static class Steve
    {
        public static List<Player> Players = new();

        public static void Create(Player player)
        {
            IEnumerator<float> main()
            {
                Animator componentInChildren = player.GameObject.GetComponentInChildren<Animator>();

                attach(player, "Steve_Head", HumanBodyBones.Head, componentInChildren);
                attach(player, "Steve_Body", HumanBodyBones.Spine, componentInChildren);
                attach(player, "Steve_LeftArm", HumanBodyBones.LeftUpperArm, componentInChildren);
                attach(player, "Steve_RightArm", HumanBodyBones.RightUpperArm, componentInChildren);
                attach(player, "Steve_LeftLeg", HumanBodyBones.LeftUpperLeg, componentInChildren);
                attach(player, "Steve_RightLeg", HumanBodyBones.RightUpperLeg, componentInChildren);

                yield break;
            }

            var main_c = Timing.RunCoroutine(main());

            Vector3 getPositionOffset(string name)
            {
                if (name == "Steve_Head")
                {
                    return new Vector3(0f, 0f, 0f);
                }
                if (name == "Steve_Body")
                {
                    return new Vector3(0f, -1f, -0.15f);
                }
                if (name == "Steve_LeftArm")
                {
                    return new Vector3(-0.2f, 0f, 0f);
                }
                if (name == "Steve_RightArm")
                {
                    return new Vector3(0.2f, 0f, 0f);
                }
                if (name == "Steve_LeftLeg")
                {
                    return new Vector3(0f, 0f, 0f);
                }
                if (!(name == "Steve_RightLeg"))
                {
                    return Vector3.zero;
                }
                return new Vector3(0f, 0f, 0f);
            }

            Vector3 getRotationOffset(string name)
            {
                if (name == "Steve_Head")
                {
                    return new Vector3(0f, 180f, 0f);
                }
                if (name == "Steve_Body")
                {
                    return new Vector3(0f, 180f, 0f);
                }
                if (name == "Steve_LeftArm")
                {
                    return new Vector3(180f, 180f, 0f);
                }
                if (name == "Steve_RightArm")
                {
                    return new Vector3(180f, 180f, 0f);
                }
                if (name == "Steve_LeftLeg")
                {
                    return new Vector3(180f, 180f, 0f);
                }
                if (!(name == "Steve_RightLeg"))
                {
                    return Vector3.zero;
                }
                return new Vector3(180f, 180f, 0f);
            }

            void attach(Player player, string name, HumanBodyBones boneType, Animator animator)
            {
                Transform boneTransform = animator.GetBoneTransform(boneType);
                if (boneTransform == null)
                    return;

                SchematicObject obj = ObjectSpawner.SpawnSchematic(name, boneTransform.position);

                Vector3 positionOffset = getPositionOffset(name);
                Vector3 rotationOffset = getRotationOffset(name);

                Timing.RunCoroutine(followBone(player, obj.transform, boneTransform, positionOffset, rotationOffset));
            }

            IEnumerator<float> followBone(Player player, Transform obj, Transform bone, Vector3 posOffset, Vector3 rotOffset)
            {
                while (true)
                {
                    obj.position = bone.position + posOffset;
                    obj.rotation = bone.rotation * Quaternion.Euler(rotOffset);
                    yield return Timing.WaitForOneFrame;
                }
            }

            void OnChangingRole(ChangingRoleEventArgs ev)
            {
                if (ev.Player == player)
                {
                    if (Players.Contains(player))
                        Players.Remove(player);

                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
                    });
                }
            }


            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        }
    }
}
