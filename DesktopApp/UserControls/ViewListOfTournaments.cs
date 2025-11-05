using LogicLayer;
using DataLayer.Repos;
using LogicLayer.Managers;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DesktopApp.UserControls
{
    public partial class ViewListOfTournaments : UserControl
    {
        private IConnection connection;
        private MatchRepo matchRepo;
        private MatchManager matchManager;
        private TournamentRepo tournamentRepo;
        private TournamentManager tournamentManager;
        private List<Control> parentControls;

        public ViewListOfTournaments()
        {
            InitializeComponent();

            this.parentControls = new List<Control>();
            this.connection = null;
            this.tournamentRepo = null;
            this.tournamentManager = null;
        }

        public void Setup(IConnection connection, List<Control> parentControls)
        {
            this.connection = connection;
            this.matchRepo = new MatchRepo(connection);
            this.matchManager = new MatchManager(matchRepo);
            this.tournamentRepo = new TournamentRepo(connection);
            this.tournamentManager = new TournamentManager(tournamentRepo, matchManager);
            this.parentControls = parentControls;

            if (!DesignMode)
            {
                VisibleChanged += ViewListOfTournaments_VisibleChanged;
                btnCreateTournament.Click += btnCreateTournament_Click;
            }
        }

        public void ViewListOfTournaments_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing && !DesignMode)
            {
                dgvTournaments.DataSource = tournamentManager.GetAllTournaments();
            }
        }

        public void RefreshTournaments()
        {
            dgvTournaments.DataSource = tournamentManager.GetAllTournaments();
        }

        private void btnCreateTournament_Click(object sender, EventArgs e)
        {
            TabPage currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var addTournament = (AddTournament)currentTab.Controls["addTournament"];

            addTournament.Visible = true;
            this.Hide();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(dgvTournaments.SelectedRows.Count == 0)
    {
                MessageBox.Show("Please select a tournament.");
                return;
            }

            var t = (Tournament)dgvTournaments.CurrentRow.DataBoundItem;

            tournamentManager.RemoveTournament(t);

            MessageBox.Show("Tournament deleted.");
            dgvTournaments.DataSource = tournamentManager.GetAllTournaments();
        }

        private void dgvTournaments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow selectedRow = dgvTournaments.Rows[e.RowIndex];

            int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);

            Form tournamentDetails = new TournamentDetails(connection, tournamentManager.GetTournamentById(id));
            tournamentDetails.Show();
        }
    }
}
