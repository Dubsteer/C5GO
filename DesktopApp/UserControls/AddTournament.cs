using LogicLayer;
using DataLayer.Repos;
using LogicLayer.Managers;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DesktopApp.UserControls
{
    public partial class AddTournament : UserControl
    {
        private IConnection connection;
        private MatchRepo matchRepo;
        private MatchManager matchManager;
        private TournamentRepo tournamentRepo;
        private TournamentManager tournamentManager;
        private List<Control> parentControls;

        public AddTournament()
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
                VisibleChanged += AddTournament_VisibleChanged;
                btnCreate.Click += btnCreate_Click;
                btnBack.Click += btnBack_Click;
            }
        }

        public void AddTournament_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing && !DesignMode)
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
                    "Invalid input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 👑 KREIRAJ TURNIR – status je Open
            Tournament tournament = new Tournament
            {
                Name = name,
                Description = description,
                Status = Status.Open
            };

            try
            {
                tournamentManager.AddTournament(tournament);
                MessageBox.Show("Tournament created successfully.",
                    "Create tournament",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // 🔄 REFRESH VIEW LIST
                var currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
                var viewList = (ViewListOfTournaments)currentTab.Controls["viewListOfTournaments1"];
                viewList.RefreshTournaments();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Create tournament",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            var currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var viewList = (ViewListOfTournaments)currentTab.Controls["viewListOfTournaments1"];

            viewList.Visible = true;
            this.Hide();
        }
    }
}
