using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DesktopApp
{
    public partial class TeamTournamentEditorForm : Form
    {
        private Tournament tournament;
        private TeamManager teamManager;
        private TournamentManager tournamentManager;
        private IConnection connection;

        private List<Team> allTeams;
        private List<Team> teamsInTournament;
        private List<Team> teamsNotInTournament;

        public TeamTournamentEditorForm(
            Tournament tournament,
            TeamManager teamManager,
            TournamentManager tournamentManager,
            IConnection connection)
        {
            InitializeComponent();

            this.tournament = tournament;
            this.teamManager = teamManager;
            this.tournamentManager = tournamentManager;
            this.connection = connection;

            LoadData();
        }

        private void LoadData()
        {
            allTeams = teamManager.GetAllTeams();

            var teamIds = tournamentManager.GetTeamsInTournament(tournament);

            teamsInTournament = allTeams
                .Where(t => teamIds.Contains(t.Id))
                .ToList();

            teamsNotInTournament = allTeams
                .Where(t => !teamIds.Contains(t.Id))
                .ToList();

            dgvTeamsInTournament.DataSource = teamsInTournament
                .Select(t => new { t.Id, t.Name, Members = t.Members.Count })
                .ToList();

            dgvTeamsAvailable.DataSource = teamsNotInTournament
                .Select(t => new { t.Id, t.Name, Members = t.Members.Count })
                .ToList();
        }

        private void btnAddTeam_Click(object sender, EventArgs e)
        {
            if (dgvTeamsAvailable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a team to add.");
                return;
            }

            int teamId = Convert.ToInt32(dgvTeamsAvailable.SelectedRows[0].Cells["Id"].Value);

            tournamentManager.AddTeamToTournament(teamId, tournament.Id);

            LoadData();
        }

        private void btnRemoveTeam_Click(object sender, EventArgs e)
        {
            if (dgvTeamsInTournament.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a team to remove.");
                return;
            }

            int teamId = Convert.ToInt32(dgvTeamsInTournament.SelectedRows[0].Cells["Id"].Value);

            tournamentManager.RemoveTeamFromTournament(teamId, tournament.Id);

            LoadData();
        }

        private void btnAutoFill_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var team in teamsInTournament)
                {
                    int missing = 5 - team.Members.Count;
                    if (missing <= 0) continue;

                    var freeUsers = teamManager.GetUsersWithoutTeam();

                    if (freeUsers.Count < missing)
                    {
                        MessageBox.Show($"Not enough free users to fill team {team.Name}");
                        continue;
                    }

                    foreach (var user in freeUsers.Take(missing))
                    {
                        teamManager.AddUserToTeam_AdminOverride(team.Id, user.Id.Value);
                    }
                }

                MessageBox.Show("Teams auto-filled successfully.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
