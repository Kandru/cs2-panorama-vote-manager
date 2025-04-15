using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Entities;
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

        private readonly PlayerLanguageManager playerLanguageManager = new();
        private CVoteController? _voteController;
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
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        }

        public override void Unload(bool hotReload)
        {
            DeregisterEventHandler<EventVoteCast>(OnVoteCast);
            RemoveListener<Listeners.OnMapStart>(OnMapStart);
            RemoveListener<Listeners.OnMapEnd>(OnMapEnd);
            // end votes (if any)
            EndVote();
        }

        private HookResult OnVoteCast(EventVoteCast @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.UserId == null
                || _voteController == null
                || !_voteController.IsValid
                || _currentVote == null) return HookResult.Continue;
            DebugPrint("OnVoteCast");
            // check which option got voted for
            VoteOptions votedOption = (VoteOptions)@event.VoteOption;
            if (votedOption == VoteOptions.YES)
            {
                _currentVote.OnVoteYes(player.UserId.Value);
            }
            else if (votedOption == VoteOptions.NO)
            {
                _currentVote.OnVoteNo(player.UserId.Value);
            }
            // send update to panorama
            SendMessageVoteUpdate(_currentVote);
            // end the vote if all players have voted
            if (_currentVote.GetYesVotes() + _currentVote.GetNoVotes() >= _currentVote.PlayerIDs.Count
                // or if no votes can't overtake yes votes anymore or draw is possible
                || (_currentVote.GetYesVotes() >= (_currentVote.PlayerIDs.Count - _currentVote.GetYesVotes())
                    && !_currentVote.Flags.HasFlag(VoteFlags.DoNotEndUntilAllVoted)))
                EndVote();
            return HookResult.Continue;
        }

        private void OnMapStart(string mapName)
        {
            if (Config.ServerSideVoting)
                AddTimer(3f, () => EnableServerSideVoting());
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
            // initiate vote controller
            InitVoteController();
            // stop if vote controller is not available
            if (_voteController == null
                || !_voteController.IsValid) return;
            DebugPrint("StartVote");
            // set current vote
            _currentVote = _votes[0];
            _votes.RemoveAt(0);
            // set cooldown
            _timeUntilNextVote = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _currentVote.Time + Config.Cooldown;
            // set default values to vote controller
            _voteController.PotentialVotes = _currentVote.PlayerIDs.Count;
            _voteController.ActiveIssueIndex = (int)VoteTypes.KICK;
            _voteController.IsYesNoVote = true;
            // send vote update to panorama
            SendMessageVoteUpdate(_currentVote);
            // send user message to panorama
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
            if (_currentVote == null
                || _voteController == null
                || !_voteController.IsValid) return;
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

        private void InitVoteController()
        {
            DebugPrint("InitVoteController");
            if (_voteController == null
                || !_voteController.IsValid)
            {
                DebugPrint("VoteController not found - checking for new one");
                _voteController = Utilities.FindAllEntitiesByDesignerName<CVoteController>("vote_controller").Last();
                if (_voteController == null
                    || !_voteController.IsValid)
                {
                    DebugPrint("VoteController not available - creating a new one");
                    _voteController = Utilities.CreateEntityByName<CVoteController>("vote_controller");
                }
                if (_voteController == null
                    || !_voteController.IsValid)
                {
                    DebugPrint("VoteController could not be created - aborting");
                    return;
                }
            }
        }

        private void ResetVoteController()
        {
            if (_voteController == null
                || !_voteController.IsValid) return;
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
            // get player slot of userid for initiator
            if (vote.Initiator != 99)
            {
                CCSPlayerController? player = Utilities.GetPlayerFromUserid(vote.Initiator);
                if (player != null && player.IsValid)
                    vote.Initiator = player.Slot;
            }
            // send message to each recipient to allow individual translation
            foreach (var playerID in vote.PlayerIDs)
            {
                CCSPlayerController? player = Utilities.GetPlayerFromUserid(playerID);
                if (player == null || !player.IsValid) continue;
                // get translation for player (if available), otherwise use server language, otherwise use first entry
                string text = vote.Text.TryGetValue(playerLanguageManager.GetLanguage(new SteamID(player.NetworkIDString)).TwoLetterISOLanguageName, out string? playerLanguage) ? playerLanguage
                    : vote.Text.TryGetValue(CoreConfig.ServerLanguage, out string? serverLanguage) ? serverLanguage
                    : vote.Text.First().Value ?? string.Empty;
                UserMessage userMessage = UserMessage.FromPartialName("VoteStart");
                userMessage.SetInt("team", vote.Team);
                userMessage.SetInt("player_slot", vote.Initiator);
                userMessage.SetInt("vote_type", (int)VoteTypes.RESET);
                userMessage.SetString("disp_str", vote.SFUI);
                userMessage.SetString("details_str", text);
                userMessage.SetBool("is_yes_no_vote", true);
                userMessage.Send([player]);
            }
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
            // send user message to indicate vote result
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
            if (vote == null
                || _voteController == null
                || !_voteController.IsValid) return;
            DebugPrint("SendMessageVoteUpdate");
            var @event = NativeAPI.CreateEvent("vote_changed", true);
            NativeAPI.SetEventInt(@event, "vote_option1", vote.GetYesVotes());
            NativeAPI.SetEventInt(@event, "vote_option2", vote.GetNoVotes());
            NativeAPI.SetEventInt(@event, "vote_option3", 0);
            NativeAPI.SetEventInt(@event, "vote_option4", 0);
            NativeAPI.SetEventInt(@event, "vote_option5", 0);
            NativeAPI.SetEventInt(@event, "potentialVotes", vote.PlayerIDs.Count);
            NativeAPI.FireEvent(@event, false);
        }
    }
}
