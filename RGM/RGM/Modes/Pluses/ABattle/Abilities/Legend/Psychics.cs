using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using MultiBroadcast.API;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Legend;

[Ability("영매", "당신을 바라보는 관전자 수에 비례하여 능력치가 상승합니다. 사망해도 모두에게 이야기할 수 있습니다.", AbilityCategory.Legend, AbilityType.LEGEND_PSYCHICS)]
public class Psychics : Ability
{
    CoroutineHandle _blessing;

    public override void OnEnabled()
    {
        _blessing = Timing.RunCoroutine(Blessing());
        Timing.RunCoroutine(Speaker());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_blessing);
    }

    public IEnumerator<float> Blessing()
    {
        while (true)
        {
            int s = Owner.CurrentSpectatingPlayers.Count();

            Owner.GetEffect(EffectType.MovementBoost).Intensity = (byte)(2.5 * s);
            Owner.GetEffect(EffectType.DamageReduction).Intensity = (byte)(2.5 * s);
            Owner.Heal(0.35f * s);

            if (Owner.Role is Scp079Role scp079)
                scp079.Energy += 0.35f * s;

            if (s >= 5)
                Owner.IsUsingStamina = false;

            else
                Owner.IsUsingStamina = true;

            if (s >= 10)
                Owner.IsBypassModeEnabled = true;

            else
                Owner.IsBypassModeEnabled = false;

            if (s >= 15)
                Owner.EnableEffect(EffectType.Ghostly, 1, 1.2f);

            if (s >= 20)
            {
                if (UnityEngine.Random.Range(1, 51) == 1)
                {
                    Item Item = Owner.AddItem(Tools.GetRandomValue(Tools.EnumToList<ItemType>()));
                }
            }

            if (s >= 25)
            {
                if (!Owner.IsNoclipPermitted)
                    Owner.IsNoclipPermitted = true;

                Owner.AddBroadcast(1, "<b><i>[ALT] 키를 눌러 <color=red>신의 권능</color>을 사용할 수 있습니다!!!</i></b>");
            }
            else
            {
                if (Owner.IsNoclipPermitted)
                    Owner.IsNoclipPermitted = false;
            }

            if (s >= 30)
                Owner.EnableEffect(EffectType.Invisible, 1, 1.2f);

            yield return Timing.WaitForSeconds(1f);
        }
    }

    public IEnumerator<float> Speaker()
    {
        while (true)
        {
            if (Owner.IsAlive)
            {
                if (IntercomPlayers.Contains(Owner))
                    IntercomPlayers.Remove(Owner);

                if (Owner.VoiceChannel == VoiceChat.VoiceChatChannel.Intercom)
                    Server.ExecuteCommand($"/speak {Owner.Id} 0");
            }
            else
            {
                if (!IntercomPlayers.Contains(Owner))
                    IntercomPlayers.Add(Owner);

                if (Owner.VoiceChannel != VoiceChat.VoiceChatChannel.Intercom)
                    Server.ExecuteCommand($"/speak {Owner.Id} 1");
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
