using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static RGM.Variables.ServerManagers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RGM;

public abstract class Mode
{
    public abstract void OnEnabled();

    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Detail { get; }
    public abstract string Color { get; }
    public virtual string Author { get; set; } = "GoldenPig1205";
    public virtual string Suggester { get; set; } = "";

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
public class ModeAttribute(ModeCategory category, ModeInfo info, ModeType type) : Attribute
{
    public ModeCategory Category { get; } = category;
    public ModeInfo Info { get; } = info;
    public ModeType Type { get; } = type;
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
    AI,
    ABattle,
    AdditionalWave,
    Blackout,
    Blessing,
    BomberMan,
    Builder,
    BombParty,
    Collector,
    Cell,
    Disguise,
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
    Skeleton,
    KoreanSpeed,
    Outlaw,
    PaperMan,
    RandomEffect,
    RandomItem,
    RandomSize,
    RedLightGreenLight,
    RocketLauncher,
    SCPRUSH,
    SnowBallFight,
    Siberia,
    SoulMate,
    Spirit,
    SuperStar,
    TrickorTreat,
    Unlimited,
    Juggernaut,
    JumpMap,
    MiniGames,
    PirateRoulette,
    Infection,
    Rescue05,
    RussianRoulette,
    SpeedRun,
    Spleef,
    TailCatcter,
    Tomb,
    WhereamI,
    WhoamI,
    WitGame
}


