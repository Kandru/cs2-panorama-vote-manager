using CounterStrikeSharp.API;

namespace PanoramaVoteManager
{
    public partial class PanoramaVoteManager
    {
        private void DebugPrint(string message)
        {
            if (Config.Debug)
            {
                Console.WriteLine(Localizer["core.debugprint"].Value.Replace("{message}", message));
            }
        }

        private void EnableServerSideVoting()
        {
            Server.ExecuteCommand("sv_allow_votes true");
            Server.ExecuteCommand("sv_vote_allow_in_warmup true");
            Server.ExecuteCommand("sv_vote_allow_spectators true");
            Server.ExecuteCommand("sv_vote_count_spectator_votes true");
            if (!Config.ServerDisableVoteOptions)
            {
                return;
            }

            Server.ExecuteCommand("sv_vote_issue_changelevel_allowed false");
            Server.ExecuteCommand("sv_vote_issue_kick_allowed false");
            Server.ExecuteCommand("sv_vote_issue_loadbackup_allowed false");
            Server.ExecuteCommand("sv_vote_issue_matchready_allowed false");
            Server.ExecuteCommand("sv_vote_issue_nextlevel_allowed false");
            Server.ExecuteCommand("sv_vote_issue_nextlevel_allowextend false");
            Server.ExecuteCommand("sv_vote_issue_nextlevel_choicesmode false");
            Server.ExecuteCommand("sv_vote_issue_nextlevel_prevent_change false");
            Server.ExecuteCommand("sv_vote_issue_pause_match_allowed false");
            Server.ExecuteCommand("sv_vote_issue_restart_game_allowed false");
            Server.ExecuteCommand("sv_vote_issue_scramble_teams_allowed false");
            Server.ExecuteCommand("sv_vote_issue_surrrender_allowed false");
            Server.ExecuteCommand("sv_vote_issue_swap_teams_allowed false");
            Server.ExecuteCommand("sv_vote_issue_timeout_allowed false");
        }
    }
}