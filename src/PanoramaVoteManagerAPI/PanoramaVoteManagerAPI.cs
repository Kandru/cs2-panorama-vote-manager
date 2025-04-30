using CounterStrikeSharp.API.Core.Capabilities;

namespace PanoramaVoteManagerAPI
{
    public interface IPanoramaVoteManagerAPI
    {
        static readonly PluginCapability<IPanoramaVoteManagerAPI> Capability = new("panoramavotemanager:api");

        int AddVote(Vote.Vote vote);
        bool RemoveVote(Vote.Vote vote);
    }
}
