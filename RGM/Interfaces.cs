using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM;

public abstract class Mode
{
    public abstract void OnEnabled();

    public abstract void OnDisabled();

    public abstract ModeCategory ModeCategory { get; set; }
    public abstract ModeInfo ModeInfo { get; set; }
    public abstract ModeType ModeType { get; set; }

    public abstract string ModeName { get; set; }
    public abstract string ModeDescription { get; set; }
    public abstract string ModeColor { get; set; }
    public abstract string ModeAuthor { get; set; }
    public string ModeSuggester { get; set; } = "";
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


