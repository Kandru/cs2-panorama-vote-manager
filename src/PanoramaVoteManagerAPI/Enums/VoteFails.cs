namespace PanoramaVoteManagerAPI.Enums;

public enum VoteFails
{
    GENERIC = 0,
    TRANSITIONING_PLAYERS,
    RATE_EXCEEDED,  // "time" is used
    YES_MUST_EXCEED_NO,
    QUORUM_FAILURE,
    ISSUE_DISABLED,
    MAP_NOT_FOUND,
    MAP_NAME_REQUIRED,
    FAILED_RECENTLY,  // "time" is used
    FAILED_RECENT_KICK,  // "time" is used
    FAILED_RECENT_CHANGEMAP,  // "time" is used
    FAILED_RECENT_SWAPTEAMS,  // "time" is used
    FAILED_RECENT_SCRAMBLETEAMS,  // "time" is used
    FAILED_RECENT_RESTART,  // "time" is used
    TEAM_CANT_CALL,
    WAITINGFORPLAYERS,
    PLAYERNOTFOUND,  // Deprecated, same as generic failure
    CANNOT_KICK_ADMIN,
    SCRAMBLE_IN_PROGRESS,
    SWAP_IN_PROGRESS,
    SPECTATOR,
    DISABLED,
    NEXTLEVEL_SET,
    REMATCH,
    TOO_EARLY_SURRENDER,
    CONTINUE,
    MATCH_PAUSED,
    MATCH_NOT_PAUSED,
    NOT_IN_WARMUP,
    NOT_10_PLAYERS,
    TIMEOUT_ACTIVE,
    TIMEOUT_INACTIVE,  // Deprecated, same as generic failure
    TIMEOUT_EXHAUSTED,
    CANT_ROUND_END,
    // Represents the highest possible index of the enum
    MAX
}