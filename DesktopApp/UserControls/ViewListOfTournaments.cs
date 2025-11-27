using System;
using System.Linq;
using System.Windows.Forms;
using LogicLayer;
using LogicLayer.Models;
using LogicLayer.Managers;
using DataLayer.Repos;

namespace DesktopApp.UserControls
{
    public partial class ViewListOfTournaments : UserControl
    {
        private IConnection connection;
        private MatchRepo matchRepo;
        private MatchManager matchManager;
        private TournamentRepo tournamentRepo;
        private TournamentManager tournamentManager;

        public ViewListOfTournaments()
        {
            InitializeComponent();
        }

        public void Setup(IConnection connection)
        {
            this.connection = connection;

            matchRepo = new MatchRepo(connection);
            matchManager = new MatchManager(matchRepo);

            tournamentRepo = new TournamentRepo(connection);
            tournamentManager = new TournamentManager(tournamentRepo, matchManager);

            if (!DesignMode)
                VisibleChanged += ViewListOfTournaments_VisibleChanged;

            btnCreateTournament.Click += btnCreateTournament_Click;
            btnDelete.Click += btnDeleteTournament_Click;

            dgvTournaments.CellDoubleClick += dgvTournaments_CellDoubleClick;
        }

        private void ViewListOfTournaments_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing && !DesignMode)
                RefreshTournaments();
        }

        public void RefreshTournaments()
        {
            dgvTournaments.DataSource = null;
            dgvTournaments.DataSource = tournamentManager.GetAllTournaments();
        }

        private void btnCreateTournament_Click(object sender, EventArgs e)
        {
            // SAKRIVAMO LIstOfTournaments
            this.Hide();

            // TRAŽIMO AddTournament kontrolu unutar istog TabPage
            var parent = this.Parent.Controls
                .OfType<AddTournament>()
                .FirstOrDefault();

            if (parent != null)
            {
                parent.Show();
                parent.BringToFront();
            }
        }

        private void btnDeleteTournament_Click(object sender, EventArgs e)
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
            if (e.RowIndex < 0)
                return;

            int id = Convert.ToInt32(
                dgvTournaments.Rows[e.RowIndex].Cells["Id"].Value
            );

            var t = tournamentManager.GetTournamentById(id);

            new TournamentDetails(connection, t).Show();
        }
    }
}
