using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using PanoramaVoteManagerAPI.Vote;

namespace PanoramaVoteManager
{
    public partial class PanoramaVoteManager
    {
        [ConsoleCommand("t", "test")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 0, usage: "!t")]
        public void CommandTest(CCSPlayerController player, CommandInfo command)
        {
            if (!player.UserId.HasValue) return;
            // notify user if vote a vote is already in queue
            if (_currentVote != null || _votes.Count > 0 || _timeUntilNextVote > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                // calculate approximate time of all votes in queue
                int time = _votes.Sum(v => v.Time + Config.Cooldown) + (_currentVote?.Time + Config.Cooldown ?? Config.Cooldown);
                command.ReplyToCommand(Localizer["vote.cooldown"].Value
                    .Replace("{time}", time.ToString()));
            }
            Random random = new Random();
            // Generates a random number between 3 and 12 inclusive
            int randomTime = random.Next(3, 13);
            // randomize vote initiator
            int voteId = random.NextDouble() < 0.5 ? 99 : player.UserId.Value;
            // add vote
            _votes.Add(new Vote(
                "#SFUI_vote_custom_default",
                new Dictionary<string, string> {
                    {"en", $"This is my cool vote -> {randomTime}"},
                    {"de", $"Mein toller Vote -> {randomTime}"},
                },
                randomTime,
                -1,
                [],
                voteId
            ));
            // start vote
            StartVote();
        }
    }
}
