namespace PanoramaVoteManagerAPI.Enums
{
    public enum VoteTypes
    {
        RESET = -1,
        UNKNOWN = 1,
        KICK,
        CHANGELEVEL,
        NEXTLEVEL,
        SWAPTEAMS,
        SCRAMBLE,
        RESTARTGAME,
        SURRENDER,
        REMATCH,
        CONTINUE,
        PAUSEMATCH,
        UNPAUSEMATCH,
        LOADBACKUP,
        ENDWARMUP,
        STARTTIMEOUT,
        ENDTIMEOUT,
        READYFORMATCH,
        NOTREADYFORMATCH,
    }
}