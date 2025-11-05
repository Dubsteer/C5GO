using LogicLayer;
using DataLayer.Repos;
using LogicLayer.Managers;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using LogicLayer.FormModels;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

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
                VisibleChanged += new EventHandler(AddTournament_VisibleChanged);
                btnCreate.Click += new EventHandler(btnCreate_Click);
                btnBack.Click += new EventHandler(btnBack_Click);
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
            var name = tbName.Text.Trim();
            var description = tbDescription.Text.Trim();

            var tournament = new Tournament(null, name, description);

            var context = new ValidationContext(tournament, null, null);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(tournament, context, errors))
            {
                if (errors.Count > 0)
                {
                    MessageBox.Show(
                        errors[0].ErrorMessage,
                        "Incorrect data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                tournamentManager.AddTournament(
                    new Tournament(
                        null,
                        name,
                        description
                        ));
            }
            catch (TournamentNotFoundException ex)
            {
                MessageBox.Show(ex.Message,
                    "Create tournament",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Create tournament",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Tournament created successfully.",
                    "Create tournament",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);


            var currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var addTournament = (AddTournament)currentTab.Controls["addTournamet"];
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            var currentTab = parentControls.OfType<TabControl>().FirstOrDefault().SelectedTab;
            var viewListOfTournaments1 = (ViewListOfTournaments)currentTab.Controls["viewListOfTournaments1"];

            viewListOfTournaments1.Visible = true;
            this.Hide();
        }
    }
}
