using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Rare;

[Ability("자리 비움", "2분 동안 움직일 수 없고 아이템을 들 수 없습니다. 대신 2분 후 <color=#A4A4A4>일반</color>, <color=#2ECCFA>희귀</color>, <color=#FF00FF>영웅</color> 능력을 하나씩 획득합니다.", AbilityCategory.Rare, AbilityType.RARE_DND)]
public class Dnd : Ability
{
    public override void OnEnabled()
    {
        IEnumerator<float> enumerator()
        {
            Owner.EnableEffect(EffectType.Ensnared, 1, 120);

            for (int i = 0; i < 120; i++)
            {
                if (Owner.IsDead)
                    yield break;

                Owner.CurrentItem = null;

                yield return Timing.WaitForSeconds(1f);
            }

            if (!Owner.IsDead)
            {
                List<AbilityCategory> categories = new List<AbilityCategory>
                {
                    AbilityCategory.Common,
                    AbilityCategory.Rare,
                    AbilityCategory.Epic
                };

                foreach (var category in categories)
                {
                    try
                    {
                        Owner.AddAbility(ABattle.Instance.GetRandomAbilities(category, 3).ToList().Where(x => x != AbilityType.RARE_DND).GetRandomValue());
                        Timing.CallDelayed(1, () =>
                        {
                            Owner.RemoveAbility(this);
                        });
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Error while adding ability to {Owner.Nickname} ({Owner.UserId}): {e}");
                    }
                }

                Owner.AddAbility(AbilityType.DUMMY_NOAFK);
            }
            
        }

        Timing.RunCoroutine(enumerator());
    }

    public override void OnDisabled()
    {
    }
}
