using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Extensions;
using PanoramaVoteManagerAPI.Vote;

namespace PanoramaVoteManager
{
    public partial class PanoramaVoteManager
    {
        [ConsoleCommand("panoramavotemanager", "PanoramaVoteManager admin commands")]
        [CommandHelper(whoCanExecute: CommandUsage.SERVER_ONLY, minArgs: 1, usage: "<command>")]
        public void CommandMapVote(CCSPlayerController player, CommandInfo command)
        {
            string subCommand = command.GetArg(1);
            switch (subCommand.ToLower(System.Globalization.CultureInfo.CurrentCulture))
            {
                case "reload":
                    Config.Reload();
                    command.ReplyToCommand(Localizer["admin.reload"]);
                    break;
                case "test":
                    // notify user if vote a vote is already in queue
                    if (_currentVote != null || _votes.Count > 0 || _timeUntilNextVote > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        // calculate approximate time of all votes in queue
                        int time = _votes.Sum(v => v.Time + Config.Cooldown) + (_currentVote?.Time + Config.Cooldown ?? Config.Cooldown);
                        command.ReplyToCommand(Localizer["vote.cooldown"].Value
                            .Replace("{time}", time.ToString()));
                    }
                    Random random = new();
                    // Generates a random number between 3 and 12 inclusive
                    int randomTime = random.Next(3, 13);
                    // add vote
                    _votes.Add(new Vote(
                        "#SFUI_vote_passed_changelevel",
                        new Dictionary<string, string> {
                            {"en", $"This is my cool vote -> {randomTime}"},
                            {"de", $"Mein toller Vote -> {randomTime}"},
                        },
                        randomTime,
                        -1,
                        [],
                        99
                    ));
                    // start vote
                    StartVote();
                    break;
                default:
                    command.ReplyToCommand(Localizer["admin.unknown_command"].Value
                        .Replace("{command}", subCommand));
                    break;
            }
        }
    }
}
