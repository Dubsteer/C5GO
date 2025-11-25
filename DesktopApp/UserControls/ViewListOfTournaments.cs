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
        private MatchManager matchManager;
        private TournamentManager tournamentManager;

        public ViewListOfTournaments()
        {
            InitializeComponent();
        }

        public void Setup(IConnection connection)
        {
            this.connection = connection;

            var matchRepo = new MatchRepo(connection);
            matchManager = new MatchManager(matchRepo);

            var tournamentRepo = new TournamentRepo(connection);
            tournamentManager = new TournamentManager(tournamentRepo, matchManager);

            // Fix: event se dodaje SAMO JEDNOM
            btnCreateTournament.Click -= btnCreateTournament_Click;
            btnDelete.Click -= btnDeleteTournament_Click;
            dgvTournaments.CellDoubleClick -= dgvTournaments_CellDoubleClick;

            btnCreateTournament.Click += btnCreateTournament_Click;
            btnDelete.Click += btnDeleteTournament_Click;
            dgvTournaments.CellDoubleClick += dgvTournaments_CellDoubleClick;

            if (!DesignMode)
                VisibleChanged += ViewListOfTournaments_VisibleChanged;
        }

        private void ViewListOfTournaments_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing)
                RefreshTournaments();
        }

        public void RefreshTournaments()
        {
            dgvTournaments.DataSource = null;
            dgvTournaments.DataSource = tournamentManager.GetAllTournaments();
        }

        private void btnCreateTournament_Click(object sender, EventArgs e)
        {
            var add = this.Parent.Controls.OfType<AddTournament>().FirstOrDefault();

            if (add != null)
            {
                add.Visible = true;
                add.BringToFront();
            }
        }

        private void btnDeleteTournament_Click(object sender, EventArgs e)
        {
            if (dgvTournaments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a tournament.");
                return;
            }

            var t = (Tournament)dgvTournaments.SelectedRows[0].DataBoundItem;

            tournamentManager.RemoveTournament(t);

            MessageBox.Show("Tournament deleted.");
            RefreshTournaments();
        }

        private void dgvTournaments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dgvTournaments.Rows[e.RowIndex].Cells["Id"].Value);
            var t = tournamentManager.GetTournamentById(id);

            new TournamentDetails(connection, t).Show();
        }
    }
}
