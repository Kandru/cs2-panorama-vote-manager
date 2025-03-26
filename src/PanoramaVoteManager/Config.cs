using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json.Serialization;

namespace PanoramaVoteManager
{
    public class PluginConfig : BasePluginConfig
    {
        // disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        // cooldown between votes in seconds
        [JsonPropertyName("cooldown")] public int Cooldown { get; set; } = 5;
        // enable server side voting
        [JsonPropertyName("server_enable_voting")] public bool ServerSideVoting { get; set; } = true;
        // disable server side vote options
        [JsonPropertyName("server_disable_vote_options")] public bool ServerDisableVoteOptions { get; set; } = true;
    }

    public partial class PanoramaVoteManager : BasePlugin, IPluginConfig<PluginConfig>
    {
        public required PluginConfig Config { get; set; }

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            // update config and write new values from plugin to config file if changed after update
            Config.Update();
            Console.WriteLine(Localizer["core.config"]);
        }
    }
}
