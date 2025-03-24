using CounterStrikeSharp.API.Core.Capabilities;

namespace PanoramaVoteManagerAPI;

public interface IPanoramaVoteManagerAPI
{
    public static readonly PluginCapability<IPanoramaVoteManagerAPI> Capability = new("panoramavotemanager:api");

    public int AddVote(Vote.Vote vote);
    public bool RemoveVote(Vote.Vote vote);
}
