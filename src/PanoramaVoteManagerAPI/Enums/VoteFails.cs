namespace PanoramaVoteManagerAPI.Enums
{
    public enum VoteFails
    {
        GENERIC = 0,
        TRANSITIONINGPLAYERS,
        RATEEXCEEDED,  // "time" is used
        YESMUSTEXCEEDNO,
        QUORUMFAILURE,
        ISSUEDISABLED,
        MAPNOTFOUND,
        MAPNAMEREQUIRED,
        FAILEDRECENTLY,  // "time" is used
        FAILEDRECENTKICK,  // "time" is used
        FAILEDRECENTCHANGEMAP,  // "time" is used
        FAILEDRECENTSWAPTEAMS,  // "time" is used
        FAILEDRECENTSCRAMBLETEAMS,  // "time" is used
        FAILEDRECENTRESTART,  // "time" is used
        TEAMCANTCALL,
        WAITINGFORPLAYERS,
        PLAYERNOTFOUND,  // Deprecated, same as generic failure
        CANNOTKICKADMIN,
        SCRAMBLEINPROGRESS,
        SWAPINPROGRESS,
        SPECTATOR,
        DISABLED,
        NEXTLEVELSET,
        REMATCH,
        TOOEARLYSURRENDER,
        CONTINUE,
        MATCHPAUSED,
        MATCHNOTPAUSED,
        NOTINWARMUP,
        NOT10PLAYERS,
        TIMEOUTACTIVE,
        TIMEOUTINACTIVE,  // Deprecated, same as generic failure
        TIMEOUTEXHAUSTED,
        CANTROUNDEND,
        // Represents the highest possible index of the enum
        MAX
    }
}