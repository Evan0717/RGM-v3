using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static RGM.Variables.ServerManagers;

namespace RGM;

public abstract class Mode
{
    public abstract void OnEnabled();

    public abstract void OnDisabled();

    public ModeData Data { get; set; }
}

public class ModeData
{
    public ModeCategory Category { get; set; }
    public ModeInfo Info { get; set; }
    public ModeType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Detail { get; set; }
    public string Color { get; set; }
    public string Author { get; set; }
    public string Suggester { get; set; } = "";
}

[AttributeUsage(AttributeTargets.Class)]
public class ModeAttribute(ModeCategory category, ModeInfo info, ModeType type, string name, string description, string detail, string color, string author, string suggester) : Attribute
{
    public ModeCategory Category { get; } = category;
    public ModeInfo Info { get; } = info;
    public ModeType Type { get; } = type;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string Detail { get; } = detail;
    public string Color { get; } = color;
    public string Author { get; } = author;
    public string Suggester { get; } = suggester;
}

public static class ModeExtensions 
{
    public static ModeData GetModeData(this ModeType modeType) => ModeList[modeType];
}

public enum ModeCategory 
{
    None,
    Public,
    Private,
    OnlySub
}

public enum ModeInfo
{
    None,
    Plus,
    Set
}

public enum ModeType
{
    None,
    ABattle,
    AdditionalWave,
    Blackout,
    Blessing,
    BombParty,
    Collector,
    Cell,
    Distancing,
    DeadLine,
    DeathRun,
    DoubleUp,
    FriendlyFire,
    FreeForAll,
    Gamble,
    GGClub,
    GunGame,
    JohnWick,
    HIDE,
    HideAndSeek,
    HotPotato,
    KoreanSpeed,
    Outlaw,
    PaperMan,
    RandomEffect,
    RandomItem,
    RandomSize,
    RedLightGreenLight,
    RocketLauncher,
    SCPRUSH,
    Siberia,
    SoulMate,
    Spirit,
    Spooky,
    SuperStar,
    TrickorTreat,
    Unlimited,
    Juggernaut,
    JumpMap,
    MiniGames,
    PirateRoulette,
    Rescue05,
    RussianRoulette,
    SpeedRun,
    Spleef,
    TailCatcher,
    Tomb,
    WhereamI,
    WhoamI,
    WitGame

}


