using DataLayer.Repos;
using LogicLayer;
using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Managers;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DesktopApp
{
    public partial class TournamentDetails : Form
    {
        private PlayerManager playerManager;
        private PlayerRepo playerRepo;

        private MatchManager matchManager;
        private MatchRepo matchRepo;

        private TournamentManager tournamentManager;
        private TournamentRepo tournamentRepo;

        private TeamMatchManager teamMatchManager;
        private TeamMatchRepo teamMatchRepo;

        private TeamRepo teamRepo;
        private TeamManager teamManager;

        private List<Match> matches;
        private List<Player> players;

        private Tournament currentTournament;
        private IConnection connection;

        public TournamentDetails(IConnection conn, Tournament tournament)
        {
            InitializeComponent();

            this.connection = conn;
            currentTournament = tournament;

            // DATA
            playerRepo = new PlayerRepo(conn);
            matchRepo = new MatchRepo(conn);
            tournamentRepo = new TournamentRepo(conn);
            teamMatchRepo = new TeamMatchRepo(conn);
            teamRepo = new TeamRepo(conn);

            // MANAGERS
            playerManager = new PlayerManager(playerRepo);
            matchManager = new MatchManager(matchRepo);
            teamMatchManager = new TeamMatchManager(teamMatchRepo);
            teamManager = new TeamManager(teamRepo);

            tournamentManager = new TournamentManager(
                tournamentRepo,
                matchManager,
                teamMatchManager
            );

            // HEADER TEXT
            lblTournamentName.Text = $"Tournament: {tournament.Name}";
            lblTournamentStatus.Text = $"Status: {tournament.Status}";
            lblTournamentType.Text = tournament.IsTeamTournament ? "Type: Team" : "Type: Solo";

            // LOAD
            players = tournamentManager.GetAllPlayersInTournament(tournament);
            matches = tournamentManager.GetAllMatchesInTournament(tournament);

            SetupUIBasedOnTournamentType();
        }

        private void SetupUIBasedOnTournamentType()
        {
            if (currentTournament.IsTeamTournament)
            {
                // TEAM UI
                tabPlayersTeams.Text = "Teams";
                tabMatches.Text = "Team Matches";

                dgvTeams.Visible = true;
                dgvTeamMatches.Visible = true;

                dgvListOfPlayers.Visible = false;
                dgvListOfMatches.Visible = false;

                lblFilterPlayers.Visible = false;
                rdAZ.Visible = false;
                rdZA.Visible = false;

                lblFilterMatches.Visible = false;
                cbMatchStatus.Visible = false;

                btnGenerateBracket.Text = "Generate Team Bracket";

                // LOAD TEAMS
                var teamIds = tournamentManager.GetTeamsInTournament(currentTournament);

                var teamsDisplay = new List<object>();

                foreach (var id in teamIds)
                {
                    var t = teamManager.GetTeam(id);
                    teamsDisplay.Add(new
                    {
                        Id = t.Id,
                        Name = t.Name,
                        MembersCount = t.Members?.Count ?? 0
                    });
                }

                dgvTeams.DataSource = teamsDisplay;

                dgvTeamMatches.DataSource =
                    teamMatchManager.GetTeamMatchesByTournament(currentTournament.Id);
            }
            else
            {
                dgvTeams.Visible = false;
                dgvTeamMatches.Visible = false;

                dgvListOfPlayers.Visible = true;
                dgvListOfMatches.Visible = true;

                lblFilterPlayers.Visible = true;
                rdAZ.Visible = true;
                rdZA.Visible = true;

                lblFilterMatches.Visible = true;
                cbMatchStatus.Visible = true;

                dgvListOfPlayers.DataSource = players;
                dgvListOfMatches.DataSource = matches;
            }
        }

        private void btnGenerateBracket_Click(object sender, EventArgs e)
        {
            try
            {
                if (!currentTournament.IsTeamTournament)
                {
                    tournamentManager.GenerateSoloBracket(players, currentTournament);
                    MessageBox.Show("Bracket generated!");
                    dgvListOfMatches.DataSource =
                        tournamentManager.GetAllMatchesInTournament(currentTournament);
                }
                else
                {
                    var teamIds = tournamentManager.GetTeamsInTournament(currentTournament);

                    if (teamIds.Count != 8 && teamIds.Count != 12 && teamIds.Count != 16)
                    {
                        MessageBox.Show("Team bracket requires 8, 12, or 16 teams.");
                        return;
                    }

                    tournamentManager.GenerateTeamBracket(teamIds, currentTournament);
                    MessageBox.Show("Team bracket generated!");
                    dgvTeamMatches.DataSource =
                        teamMatchManager.GetTeamMatchesByTournament(currentTournament.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void rdAZ_CheckedChanged(object sender, EventArgs e)
        {
            if (rdAZ.Checked && !currentTournament.IsTeamTournament)
            {
                players = players.OrderBy(p => p.Username).ToList();
                dgvListOfPlayers.DataSource = players;
            }
        }

        private void rdZA_CheckedChanged(object sender, EventArgs e)
        {
            if (rdZA.Checked && !currentTournament.IsTeamTournament)
            {
                players = players.OrderByDescending(p => p.Username).ToList();
                dgvListOfPlayers.DataSource = players;
            }
        }

        private void cbMatchStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentTournament.IsTeamTournament)
                return;

            if (cbMatchStatus.SelectedItem.ToString() == "All")
            {
                dgvListOfMatches.DataSource =
                    tournamentManager.GetAllMatchesInTournament(currentTournament);
            }
            else
            {
                string selected = cbMatchStatus.Text;

                dgvListOfMatches.DataSource =
                    tournamentManager.GetAllMatchesInTournament(currentTournament)
                    .Where(m => m.Status.ToString() == selected)
                    .ToList();
            }
        }

        private void btnEditMatch_Click(object sender, EventArgs e)
        {
            if (!currentTournament.IsTeamTournament)
                new ListOfMatches(matchManager, tournamentManager, currentTournament).Show();
            else
                MessageBox.Show("Team match manual editing is done in bracket view.");
        }

        private void btnViewBracket_Click(object sender, EventArgs e)
        {
            if (!currentTournament.IsTeamTournament)
            {
                MessageBox.Show("Only team tournaments have bracket view.");
                return;
            }

            var bracket = new TeamBracketView(
                currentTournament,
                teamMatchManager,
                teamManager
            );

            bracket.Show();
        }

        // 🔥 NEW BUTTON: opens the full team editor
        private void btnManageTeams_Click(object sender, EventArgs e)
        {
            if (!currentTournament.IsTeamTournament)
            {
                MessageBox.Show("Only team tournaments have team management.");
                return;
            }

            TeamTournamentEditorForm form = new TeamTournamentEditorForm(
                currentTournament,
                teamManager,
                tournamentManager,
                connection
            );

            form.ShowDialog();

            // Refresh UI after editing
            SetupUIBasedOnTournamentType();
        }
    }
}
