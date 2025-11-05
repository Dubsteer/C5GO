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
        public TournamentDetails(IConnection conn, Tournament tournament)
        {
            InitializeComponent();

            playerRepo = new PlayerRepo(conn);
            matchRepo = new MatchRepo(conn);
            tournamentRepo = new TournamentRepo(conn);

            playerManager = new PlayerManager(playerRepo);
            matchManager = new MatchManager(matchRepo);
            tournamentManager = new TournamentManager(tournamentRepo, matchManager);

            currentTournament = tournament;

            players = tournamentManager.GetAllPlayersInTournament(tournament);
            matches = tournamentManager.GetAllMatchesInTournament(tournament);

            cbMatchStatus.Text = "All";

            RefreshDGV();
        }

        private PlayerManager playerManager;
        private PlayerRepo playerRepo;
        private MatchManager matchManager;
        private MatchRepo matchRepo;
        private TournamentManager tournamentManager;
        private TournamentRepo tournamentRepo;

        private List<Match> matches;
        private List<Player> players;
        private Tournament currentTournament;

        public void RefreshDGV()
        {
            dgvListOfMatches.DataSource = null;
            dgvListOfMatches.DataSource = matches;

            dgvListOfPlayers.DataSource = null;
            dgvListOfPlayers.DataSource = players;
        }

        private void btnEditMatch_Click(object sender, EventArgs e)
        {
            Form match = new ListOfMatches(matchManager, tournamentManager, currentTournament);
            match.Show();
        }

        private void btnRunTournament_Click(object sender, EventArgs e)
        {
            if (currentTournament.Status == LogicLayer.Enums.Status.Closed)
            {
                MessageBox.Show("Tournament is closed.");
                return;
            }

            var selectedPlayers = tournamentManager.GetAllPlayersInTournament(currentTournament);

            if (selectedPlayers.Count < 2)
            {
                MessageBox.Show("Not enough players.");
                return;
            }

            try
            {
                tournamentManager.TournamentLogic(
                    selectedPlayers,
                    currentTournament,
                    datePicker.Value,
                    (int)nUD.Value
                );

                MessageBox.Show("Tournament started!");

                dgvListOfMatches.DataSource = tournamentManager.GetAllMatchesInTournament(currentTournament);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void rdAZ_CheckedChanged(object sender, EventArgs e)
        {
            if (rdAZ.Checked)
            {
                players = players.OrderBy(p => p.Username).ToList();
                RefreshDGV();
            }
        }

        private void rdZA_CheckedChanged(object sender, EventArgs e)
        {
            if (rdZA.Checked)
            {
                players = players.OrderByDescending(p => p.Username).ToList();
                RefreshDGV();
            }
        }

        private void cbMatchStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMatchStatus.SelectedItem == null)
                return;

            if (cbMatchStatus.SelectedItem.ToString() == "All")
            {
                matches = tournamentManager.GetAllMatchesInTournament(currentTournament);
            }
            else
            {
                string selected = cbMatchStatus.Text;
                matches = tournamentManager
                    .GetAllMatchesInTournament(currentTournament)
                    .Where(m => m.Status.ToString() == selected)
                    .ToList();
            }

            dgvListOfMatches.DataSource = matches;
        }
    }
}
