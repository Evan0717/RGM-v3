using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using Player = Exiled.API.Features.Player;
using Round = Exiled.API.Features.Round;
using Server = Exiled.API.Features.Server;

namespace RGM.Modes;


[Mode(ModeCategory.Private, ModeInfo.Plus, ModeType.CBR)]
public class CBR : Mode
{
    public override void OnEnabled()
    {
        Round.IsLocked = true;
        Server.FriendlyFire = true;
        Start();
        
        Exiled.Events.Handlers.Player.Died += OnDied;
        
        if (!(PlayerManager.List.Count < 2)) return;
        Round.IsLocked = false;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    private static void OnDied(DiedEventArgs _)
    {
        if (!(PlayerManager.List.Count < 2)) return;
        Round.IsLocked = false;
    }

    private static void Start()
    {
        var players = PlayerManager.List.Where(x => !x.IsNPC);

        var enumerable = players as Player[] ?? players.ToArray();
        foreach (var player in enumerable)
        {
            player.RoleManager.ServerSetRole(RoleTypeId.ClassD, RoleChangeReason.Respawn);
            Timing.RunCoroutine(AddItem(player));
        }

        Server.ExecuteCommand("/decontamination f");
    }

    private static IEnumerator<float> AddItem(Player pl)
    {
        while (pl.Items.Count < 8) pl.AddItem(ItemType.Medkit);
        yield return Timing.WaitForOneFrame;
    }
    
    public override string Name => "화생방";
    public override string Description => "혼돈의 군대식 가스실에서 생존해보세요!";
    public override string Detail => "구급상자 8개가 주어집니다. 어떻게든 생존하세요. 가스의 세기는 다를 것입니다.";
    public override string Color  => "FFF347";
}