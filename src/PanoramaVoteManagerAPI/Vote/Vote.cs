using CounterStrikeSharp.API;
using PanoramaVoteManagerAPI.Enums;

namespace PanoramaVoteManagerAPI.Vote;

public class Vote
{
    public string SFUI { get; set; }
    public Dictionary<string, string> Text { get; set; }
    public int Time { get; set; }
    public int Team { get; set; }
    public List<int> PlayerIDs { get; set; }
    public int Initiator { get; set; }
    public float MinSuccessPercentage { get; set; } = 0.51f;
    public int MinVotes { get; set; } = 1;
    public Dictionary<int, int> _voters { get; set; } = new Dictionary<int, int>();
    public VoteFlags Flags { get; set; } = VoteFlags.None;
    public Action<Vote, bool>? Callback { get; set; } = null;

    public Vote(string sfui,
        Dictionary<string, string> text,
        int time,
        int team,
        List<int> playerIDs,
        int initiator = 99,
        float minSuccessPercentage = 0.51f,
        int minVotes = 1,
        VoteFlags flags = VoteFlags.None,
        Action<Vote, bool>? callback = null)
    {
        SFUI = sfui;
        Text = text;
        Time = time;
        Team = team;
        PlayerIDs = playerIDs;
        Initiator = initiator;
        MinSuccessPercentage = minSuccessPercentage;
        MinVotes = minVotes;
        Flags = flags;
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
            {
                // remove missing players from _voters list
                _voters.Remove(missingPlayer);
                PlayerIDs.Remove(missingPlayer);
            }
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
        // prepare vote result
        int totalVotes = GetYesVotes() + GetNoVotes();
        float successPercentage = PlayerIDs.Count > 0
            ? (float)GetYesVotes() / PlayerIDs.Count
            : 0;
        // return success if VoteFlags.AlwaysSuccessful is set
        if (Flags.HasFlag(VoteFlags.AlwaysSuccessful))
            return VoteStates.SUCCESS;
        // if no players are eligible to vote, consider it failed
        if (PlayerIDs.Count == 0)
            return VoteStates.FAILED;
        // if total votes are less than MinVotes return failed
        if (totalVotes < MinVotes)
            return VoteStates.FAILED;
        // if success percentage is greater than MinSuccessPercentage return success
        else if (successPercentage >= MinSuccessPercentage && GetYesVotes() > GetNoVotes())
            return VoteStates.SUCCESS;
        // if yes vote count is less than no vote count return failed
        else if (GetYesVotes() < GetNoVotes())
            return VoteStates.FAILED;
        // return a draw if none of the above conditions are met
        else
            return VoteStates.DRAW;
    }
}
