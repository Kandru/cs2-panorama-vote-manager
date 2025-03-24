using CounterStrikeSharp.API;
using PanoramaVoteManagerAPI.Enums;

namespace PanoramaVoteManagerAPI.Vote;

public class Vote
{
    public string Title { get; set; }
    public string Text { get; set; }
    public int Time { get; set; }
    public int Team { get; set; }
    public List<int> PlayerIDs { get; set; }
    public int Initiator { get; set; }
    public float MinSuccessPercentage { get; set; } = 0.5f;
    public int MinVotes { get; set; } = 1;
    public Dictionary<int, int> _voters { get; set; } = new Dictionary<int, int>();
    public Action<Vote, bool>? Callback { get; set; } = null;

    public Vote(string title, string text, int time, int team, List<int> playerIDs, int initiator = 99, Action<Vote, bool>? callback = null)
    {
        Title = title;
        Text = text;
        Time = time;
        Team = team;
        PlayerIDs = playerIDs;
        Initiator = initiator;
        Callback = callback;
        // update playerIDs
        UpdatePlayerIDs();
    }


    public void OnVoteYes(int playerID)
    {
        // add player to vote list
        _voters[playerID] = (int)VoteOptions.YES;
        // update playerIDs
        UpdatePlayerIDs();
    }

    public void OnVoteNo(int playerID)
    {
        // add player to vote list
        _voters[playerID] = (int)VoteOptions.NO;
        // update playerIDs
        UpdatePlayerIDs();
    }

    private void UpdatePlayerIDs()
    {
        var players = Utilities.GetPlayers().Where(p => !p.IsBot && !p.IsHLTV);
        // check if playerIDs in players list if not empty
        if (PlayerIDs.Count > 0)
        {
            // get playerids which are disconnected
            var missingPlayers = PlayerIDs.Except(players.Select(p => p.UserId!.Value));
            foreach (var missingPlayer in missingPlayers)
                _voters.Remove(missingPlayer);
            // update PlayerIDs list
            PlayerIDs = [.. players.Where(p => p.UserId.HasValue).Select(p => p.UserId!.Value)];
        }
        else
        {
            // if playerIDs is empty, add all players
            PlayerIDs = [.. players.Where(p => p.UserId.HasValue).Select(p => p.UserId!.Value)];
        }
    }

    public int GetYesVotes()
    {
        return _voters.Count(v => v.Value == (int)VoteOptions.YES);
    }

    public int GetNoVotes()
    {
        return _voters.Count(v => v.Value == (int)VoteOptions.NO);
    }

    public VoteStates OnVoteEnd()
    {
        int totalVotes = GetYesVotes() + GetNoVotes();
        float successPercentage = (float)GetYesVotes() / totalVotes;

        if (totalVotes < MinVotes)
            return VoteStates.FAILED;
        else if (successPercentage >= MinSuccessPercentage && GetYesVotes() > GetNoVotes())
            return VoteStates.SUCCESS;
        else if (GetYesVotes() < GetNoVotes())
            return VoteStates.FAILED;
        else
            return VoteStates.DRAW;
    }
}
