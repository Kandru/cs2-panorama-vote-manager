using PanoramaVoteManagerAPI;
using PanoramaVoteManagerAPI.Vote;

namespace PanoramaVoteManager
{
    public partial class PanoramaVoteManager : IPanoramaVoteManagerAPI
    {
        public static IPanoramaVoteManagerAPI API = null!;

        // adds a vote to the queue. Returns the time in seconds until the vote will be executed
        public int AddVote(Vote vote)
        {
            int delay = _votes.Sum(v => v.Time + Config.Cooldown) + (_currentVote?.Time + Config.Cooldown ?? Config.Cooldown);
            // add vote to queue
            _votes.Add(vote);
            // check if vote needs to be added to queue
            if (_currentVote != null || _votes.Count > 1 || _timeUntilNextVote > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                return delay;
            }
            else
            {
                StartVote();
                return 0;
            }
        }

        // removes a vote from the queue. Returns true if the vote was removed successfully
        public bool RemoveVote(Vote vote)
        {
            // check if vote is in queue
            if (_votes.Contains(vote))
            {
                // if in queue simply remove
                _ = _votes.Remove(vote);
                return true;
            }
            else if (_currentVote == vote)
            {
                // if current vote remove and end vote
                EndVote(true);
                return true;
            }
            return false;
        }
    }
}