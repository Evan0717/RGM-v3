using System.Linq;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp173.Rare;

[Ability("육중한 무게", "[<color=#2ECCFA>희귀</color>] 이차원도약 능력에 영향을 받지 않습니다.", AbilityCategory.Rare, AbilityType.RARE_SCP173_IMMENSEWEIGHT, RoleAbility.Scp173)]
public class ImmenseWeight : Ability;
