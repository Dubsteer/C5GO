using LogicLayer;
using DataLayer.Repos;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Enums;
using System;
using System.Windows.Forms;

namespace DesktopApp.UserControls
{
    public partial class AddTournament : UserControl
    {
        private TournamentManager tournamentManager;

        public event Action TournamentCreated;

        public AddTournament()
        {
            InitializeComponent();
        }

        public void Setup(IConnection connection)
        {
            var matchRepo = new MatchRepo(connection);
            var matchManager = new MatchManager(matchRepo);

            var tournamentRepo = new TournamentRepo(connection);
            tournamentManager = new TournamentManager(tournamentRepo, matchManager);

            btnCreate.Click += btnCreate_Click;
            btnBack.Click += btnBack_Click;

            VisibleChanged += AddTournament_VisibleChanged;
        }

        private void AddTournament_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                tbName.Clear();
                tbDescription.Clear();
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string name = tbName.Text.Trim();
            string description = tbDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Name and description must not be empty.",
                    "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Tournament t = new Tournament
                {
                    Name = name,
                    Description = description,
                    Status = Status.Open
                };

                tournamentManager.AddTournament(t);

                MessageBox.Show("Tournament created!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                TournamentCreated?.Invoke();

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
