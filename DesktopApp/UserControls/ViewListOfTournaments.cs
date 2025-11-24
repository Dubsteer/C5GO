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

            parentControls = new List<Control>();
        }

        public void Setup(IConnection connection, List<Control> parentControls)
        {
            this.connection = connection;
            this.parentControls = parentControls;

            matchRepo = new MatchRepo(connection);
            matchManager = new MatchManager(matchRepo);

            tournamentRepo = new TournamentRepo(connection);
            tournamentManager = new TournamentManager(tournamentRepo, matchManager);

            if (!DesignMode)
                VisibleChanged += ViewListOfTournaments_VisibleChanged;
        }

        private void ViewListOfTournaments_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing && !DesignMode)
                dgvTournaments.DataSource = tournamentManager.GetAllTournaments();
        }

        // ✅ FIX — METHOD THAT WAS MISSING
        public void RefreshTournaments()
        {
            dgvTournaments.DataSource = tournamentManager.GetAllTournaments();
        }

        private void btnCreateTournament_Click(object sender, EventArgs e)
        {
            var currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var addTournament = currentTab.Controls.OfType<AddTournament>().First();

            addTournament.Visible = true;
            this.Hide();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTournaments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a tournament.");
                return;
            }

            var t = (Tournament)dgvTournaments.CurrentRow.DataBoundItem;

            tournamentManager.RemoveTournament(t);
            MessageBox.Show("Tournament deleted.");

            RefreshTournaments();
        }

        private void dgvTournaments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = Convert.ToInt32(dgvTournaments.Rows[e.RowIndex].Cells["Id"].Value);

            new TournamentDetails(connection, tournamentManager.GetTournamentById(id)).Show();
        }
    }
}
