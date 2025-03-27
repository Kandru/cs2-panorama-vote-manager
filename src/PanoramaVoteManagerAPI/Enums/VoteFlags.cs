namespace PanoramaVoteManagerAPI.Enums;

[Flags]
public enum VoteFlags
{
    None = 0,
    // Vote is always successful regardless of the outcome
    AlwaysSuccessful = 1 << 0, // 1
}