using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using PanoramaVoteManagerAPI;
using PanoramaVoteManagerAPI.Enums;
using PanoramaVoteManagerAPI.Vote;

namespace PanoramaVoteManager
{
    public partial class PanoramaVoteManager : BasePlugin
    {
        public override string ModuleName => "CS2 PanoramaVoteManager";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        private CVoteController? _voteController => Utilities.FindAllEntitiesByDesignerName<CVoteController>("vote_controller").LastOrDefault();
        private List<Vote> _votes = [];
        private Vote? _currentVote = null;
        private long _timeUntilNextVote = 0;

        public override void Load(bool hotReload)
        {
            API = this;
            // register plugin capability
            if (!hotReload)
                Capabilities.RegisterPluginCapability(IPanoramaVoteManagerAPI.Capability, () => API);
            // do not load if plugin is disabled
            if (!Config.Enabled) return;
            // register event handlers
            RegisterEventHandler<EventVoteCast>(OnVoteCast);
            RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        }

        public override void Unload(bool hotReload)
        {
            DeregisterEventHandler<EventVoteCast>(OnVoteCast);
            RemoveListener<Listeners.OnMapEnd>(OnMapEnd);
            // end votes (if any)
            EndVote();
        }

        private HookResult OnVoteCast(EventVoteCast @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.UserId == null) return HookResult.Continue;
            if (_currentVote == null) return HookResult.Continue;
            DebugPrint("OnVoteCast");
            // check which option got voted for
            VoteOptions votedOption = (VoteOptions)@event.VoteOption;
            if (votedOption == VoteOptions.YES)
                _currentVote.OnVoteYes(player.UserId.Value);
            else if (votedOption == VoteOptions.NO)
                _currentVote.OnVoteNo(player.UserId.Value);
            // send update to panorama
            SendMessageVoteUpdate(_currentVote);
            // end the vote if all players have voted
            if (_currentVote.GetYesVotes() + _currentVote.GetNoVotes() >= _currentVote.PlayerIDs.Count)
                EndVote();
            return HookResult.Continue;
        }

        private void OnMapEnd()
        {
            if (!Config.Enabled) return;
            DebugPrint("OnMapEnd");
            // end votes (if any)
            EndVote();
            // reset votes
            _votes.Clear();
        }

        private void StartVote()
        {
            if (!Config.Enabled) return;
            if (_currentVote != null || _votes.Count == 0) return;
            if (_timeUntilNextVote > DateTimeOffset.UtcNow.ToUnixTimeSeconds()) return;
            DebugPrint("StartVote");
            // set current vote
            _currentVote = _votes[0];
            _votes.RemoveAt(0);
            // initiate vote controller
            ResetVoteController();
            InitVoteController(_currentVote);
            SendMessageVoteUpdate(_currentVote);
            // send message
            SendMessageVoteStart(_currentVote);
            // add a timer to end the vote after the specified time
            var currentVote = _currentVote;
            AddTimer(_currentVote.Time, () =>
            {
                if (_currentVote == null || _currentVote != currentVote)
                {
                    DebugPrint("Vote was removed before it ended. Skipping EndVote");
                    return;
                }
                DebugPrint($"Vote ends in {_currentVote.Time} seconds");
                EndVote();
            });
        }

        private void EndVote(bool canceled = false)
        {
            if (_currentVote == null || _voteController == null) return;
            DebugPrint("EndVote");
            VoteStates result = _currentVote.OnVoteEnd();
            if (!canceled && result == VoteStates.SUCCESS)
            {
                // send user message to players
                SendMessageVoteEnd(_currentVote, true);
                // send vote result to external plugin
                if (_currentVote.Callback != null)
                    _currentVote.Callback(_currentVote, true);
            }
            else
            {
                // send user message to players
                SendMessageVoteEnd(_currentVote, false);
                // send vote result to external plugin
                if (_currentVote.Callback != null)
                    _currentVote.Callback(_currentVote, false);
            }
            // reset vote controller
            ResetVoteController();
            // reset current vote
            _currentVote = null;
            // set cooldown
            _timeUntilNextVote = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + Config.Cooldown;
            AddTimer(Config.Cooldown, () =>
            {
                DebugPrint($"Cooldown ended, starting next vote in {Config.Cooldown} seconds");
                StartVote();
            });
        }

        private void InitVoteController(Vote vote)
        {
            if (vote == null || _voteController == null) return;
            DebugPrint("InitVoteController");
            // initiate vote controller
            _voteController.PotentialVotes = vote.PlayerIDs.Count;
            _voteController.ActiveIssueIndex = (int)VoteTypes.UNKNOWN;
            _voteController.IsYesNoVote = true;
        }

        private void ResetVoteController()
        {
            if (_voteController == null) return;
            DebugPrint("ResetVoteController");
            // reset vote controller
            foreach (var i in Enumerable.Range(0, 5))
                _voteController.VoteOptionCount[i] = 0;
            for (int i = 0; i < Server.MaxPlayers; i++)
                _voteController.VotesCast[i] = (int)VoteOptions.REMOVE;
        }

        private void SendMessageVoteStart(Vote vote)
        {
            if (vote == null) return;
            DebugPrint("SendMessageVoteStart");
            // set recipients which should get the message (if applicable)
            RecipientFilter recipientFilter = [];
            foreach (var playerID in vote.PlayerIDs)
            {
                CCSPlayerController? player = Utilities.GetPlayerFromUserid(playerID);
                if (player == null || !player.IsValid) continue;
                recipientFilter.Add(player);
            }
            // get player slot of userid for initiator
            if (vote.Initiator != 99)
            {
                CCSPlayerController? player = Utilities.GetPlayerFromUserid(vote.Initiator);
                if (player != null && player.IsValid)
                    vote.Initiator = player.Slot;
            }
            UserMessage userMessage = UserMessage.FromPartialName("VoteStart");
            userMessage.SetInt("team", vote.Team);
            userMessage.SetInt("player_slot", vote.Initiator);
            userMessage.SetInt("vote_type", (int)VoteTypes.UNKNOWN);
            userMessage.SetString("disp_str", vote.Title);
            userMessage.SetString("details_str", vote.Text);
            userMessage.SetString("other_team_str", "#SFUI_otherteam_vote_unimplemented");
            userMessage.SetBool("is_yes_no_vote", true);
            userMessage.Send(recipientFilter);
        }

        private void SendMessageVoteEnd(Vote vote, bool success)
        {
            if (vote == null) return;
            DebugPrint("SendMessageVoteEnd");
            // set recipients which should get the message (if applicable)
            RecipientFilter recipientFilter = [];
            foreach (var playerID in vote.PlayerIDs)
            {
                CCSPlayerController? player = Utilities.GetPlayerFromUserid(playerID);
                if (player == null || !player.IsValid) continue;
                recipientFilter.Add(player);
            }
            if (!success)
            {
                UserMessage userMessage = UserMessage.FromPartialName("VoteFailed");
                userMessage.SetInt("reason", 0);
                userMessage.Send(recipientFilter);
                return;
            }
            else
            {
                UserMessage userMessage = UserMessage.FromPartialName("VotePass");
                userMessage.SetInt("team", vote.Team);
                userMessage.SetInt("vote_type", (int)VoteTypes.UNKNOWN);
                userMessage.SetString("disp_str", "#SFUI_vote_passed");
                userMessage.SetString("details_str", "");
                userMessage.Send(recipientFilter);
            }
        }

        private void SendMessageVoteUpdate(Vote vote)
        {
            if (vote == null || _voteController == null) return;
            DebugPrint("SendMessageVoteUpdate");
            var @event = new EventVoteChanged(true)
            {
                VoteOption1 = vote.GetYesVotes(),
                VoteOption2 = vote.GetNoVotes(),
                Potentialvotes = vote.PlayerIDs.Count,
            };
            @event.FireEvent(false);
        }
    }
}
