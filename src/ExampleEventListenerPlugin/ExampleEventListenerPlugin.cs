using PanoramaVoteManagerAPI;
using PanoramaVoteManagerAPI.Vote;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;

namespace ExampleEventListenerPlugin
{
    public partial class ExampleEventListenerPlugin : BasePlugin
    {
        public override string ModuleName => "ExampleEventListenerPlugin";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";
        public override string ModuleVersion => "1.0.0";

        private static PluginCapability<IPanoramaVoteManagerAPI> VoteAPI { get; } = new("panoramavotemanager:api");
        private IPanoramaVoteManagerAPI? _voteManager;

        public override void Load(bool hotReload)
        {
            Console.WriteLine("==== Example Event Listener Plugin for CS2 Panorama Vote Manager loaded! ====");
        }

        public override void OnAllPluginsLoaded(bool hotReload)
        {
            Console.WriteLine("==== Example Event Listener Plugin for CS2 Panorama Vote Manager OnAllPluginsLoaded! ====");
            _voteManager = VoteAPI.Get();
        }

        public override void Unload(bool hotReload)
        {
            Console.WriteLine("==== Example Event Listener Plugin unloading! ====");
        }


        [ConsoleCommand("test", "test")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 0, usage: "!test")]
        public void CommandTest(CCSPlayerController player, CommandInfo command)
        {
            Console.WriteLine("==== CommandTest called! ====");
            if (!player.UserId.HasValue) return;
            Random random = new Random();
            // Generates a random number between 3 and 12 inclusive
            int randomTime = random.Next(10, 20);
            // randomize vote initiator
            int voteId = random.NextDouble() < 0.5 ? 99 : player.UserId.Value;
            Vote vote = new Vote(
                "#SFUI_vote_custom_default",
                new Dictionary<string, string> {
                    {"en", $"This is vote triggered by another plugin for {randomTime} seconds :)"},
                    {"de", $"Das ist ein Vote aus einem anderen Plugin für {randomTime} Sekunden :)"},
                },
                randomTime,
                -1,
                [],
                voteId,
                OnVoteResult
            );
            if (_voteManager != null)
            {
                var startTime = _voteManager.AddVote(vote);
                Console.WriteLine($"vote will start in approx. {startTime} seconds");
            }
            else
            {
                Console.WriteLine("VoteManager is null!");
            }
            Console.WriteLine("==== CommandTest finished! ====");
        }

        public void OnVoteResult(Vote vote, bool success)
        {
            Console.WriteLine("==== OnVoteResult called! ====");
            Console.WriteLine($"Vote: {vote.Title} was {(success ? "successful" : "unsuccessful")}");
        }
    }
}
