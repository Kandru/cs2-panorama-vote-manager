namespace PanoramaVoteManagerAPI.Enums;

[Flags]
public enum VoteFlags
{
    None = 0,
    // Vote is always successful regardless of the outcome
    AlwaysSuccessful = 1 << 0, // 1
    // vote should not end before all players have voted or timeout
    DoNotEndUntilAllVoted = 1 << 1, // 2
}