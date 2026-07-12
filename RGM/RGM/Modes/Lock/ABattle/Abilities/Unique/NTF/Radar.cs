using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("레이더", "지급된 무전기를 들면 가장 가까운 유기체와의 거리를 확인할 수 있습니다.", AbilityCategory.NTF, AbilityType.NTF_RADAR)]
public class Radar : Ability
{
    ushort RadarSerial;
    CoroutineHandle _radar1;

    public override void OnEnabled()
    {
        Item rd = Owner.AddItem(ItemType.Radio);
        RadarSerial = rd.Serial;

        _radar1 = Timing.RunCoroutine(Radar1());
    }

    public override void OnDisabled()
    {
    }

    public IEnumerator<float> Radar1()
    {
        while (true)
        {
            foreach (var player in PlayerManager.List)
            {
                if (player.CurrentItem != null && RadarSerial == player.CurrentItem.Serial)
                {
                    if (Tools.TryGetNearestPlayer(player, out Player nearestPlayer, out float radius))
                    {
                        if (nearestPlayer != null && radius < 99999)
                            player.AddHint("레이더", $"<color={nearestPlayer.Role.Color.ToHex()}>{( Trans.Role[nearestPlayer.Role.Type])}</color> - {radius.ToString("F1")}m", 1.2f);
                    }
                }
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
