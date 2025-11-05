using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using DesktopApp.UserControls;
using System.Diagnostics;
using System.Xml;
using Mysqlx.Crud;
using System.Diagnostics.Metrics;
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
            players = tournamentManager.GetAllPlayersInTournament(tournament);
            matches = tournamentManager.GetAllMatchesInTournament(tournament);
            currentTournament = tournament;

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
        private List<Tournament> tournaments;
        private Tournament currentTournament;
        private IConnection connection;
        private bool isTournamentRunning;

        private void btnEditMatch_Click(object sender, EventArgs e)
        {
            Form match = new ListOfMatches(matchManager,tournamentManager, currentTournament);
            match.Show();
        }

        public void RefreshDGV()
        {
            dgvListOfMatches.DataSource = null;
            dgvListOfMatches.DataSource = matches;
            dgvListOfPlayers.DataSource = null;
            dgvListOfPlayers.DataSource = players;
        }

        private void btnRunTournament_Click(object sender, EventArgs e)
        {
            if (currentTournament.Closed)
            {
                MessageBox.Show("Tournament is not open for registration");
                return;
            }

            var allPlayers = playerManager.GetAllPlayers();
            var selectedPlayers = new List<Player>();
            foreach (DataGridViewRow row in dgvListOfPlayers.Rows)
            {
                Player p = allPlayers.FirstOrDefault(player => player.Id.ToString() == row.Cells[1].Value.ToString());
                selectedPlayers.Add(p);
            }

            try
            {
                tournamentManager.TournamentLogic(selectedPlayers, currentTournament, datePicker.Value, (int)nUD.Value);
                MessageBox.Show("Tournament has started");
                dgvListOfMatches.DataSource = tournamentManager.GetAllMatchesInTournament(currentTournament);

                tournamentManager.CloseTournament(currentTournament);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnRunTournament.Hide();
        }

        private void rdAZ_CheckedChanged(object sender, EventArgs e)
        {
            if (rdAZ.Checked)
            {
                players.Sort((p1, p2) => p1.Username.CompareTo(p2.Username));
                RefreshDGV();
            }
        }

        private void rdZA_CheckedChanged(object sender, EventArgs e)
        {
            if (rdZA.Checked)
            {
                players.Sort((p1, p2) => p1.Username.CompareTo(p2.Username));
                players.Reverse();
                RefreshDGV();
            }

        }

        private void cbMatchStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMatchStatus.SelectedItem == null)
                return;
            var matches = new List<Match>();
            if (cbMatchStatus.SelectedItem.ToString() == "All")
                matches = tournamentManager.GetAllMatchesInTournament(currentTournament);
            else
                matches = tournamentManager.GetAllMatchesInTournament(currentTournament).Where(m => m.Status.ToString() == cbMatchStatus.Text).ToList();
            dgvListOfMatches.DataSource = matches;
        }
    }
}
