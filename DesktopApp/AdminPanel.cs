using System;
using System.Linq;
using System.Windows.Forms;
using LogicLayer;
using LogicLayer.Models;
using LogicLayer.Managers;
using DataLayer.Repos;

namespace DesktopApp
{
    public partial class AdminPanel : Form
    {
        private readonly IConnection connection;

        private readonly UserManager userManager;
        private readonly PostManager postManager;
        private readonly CommentManager commentManager;
        private readonly TournamentManager tournamentManager;
        private readonly MatchManager matchManager;
        private readonly PlayerManager playerManager;

        public User currentUser;

        public AdminPanel(IConnection connection, User currentUser)
        {
            InitializeComponent();

            this.connection = connection;
            this.currentUser = currentUser;

            // === REPOS ===
            var userRepo = new UserRepo(connection);
            var postRepo = new PostRepo(connection);
            var commentRepo = new CommentRepo(connection);
            var tournamentRepo = new TournamentRepo(connection);
            var matchRepo = new MatchRepo(connection);
            var playerRepo = new PlayerRepo(connection);

            // === MANAGERS ===
            userManager = new UserManager(userRepo);
            postManager = new PostManager(postRepo);
            commentManager = new CommentManager(commentRepo);
            matchManager = new MatchManager(matchRepo);
            tournamentManager = new TournamentManager(tournamentRepo, matchManager);
            playerManager = new PlayerManager(playerRepo);

            // === SETUP USER CONTROLS ===
            var controls = this.Controls.Cast<Control>().ToList();

            viewUsers.Setup(connection, controls);
            viewPosts.Setup(connection, controls);
            viewListOfTournaments1.Setup(connection, controls);
            addTournament.Setup(connection, controls);
            createPost.Setup(connection, controls, currentUser);
            updatePost.Setup(connection, controls, currentUser);

            // Default tab = Users
            tabControl1.SelectedIndex = 0;
        }

        // =====================
        // NAVIGATION (VARIJANTA 2)
        // =====================

        private void btnUsers_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void btnPosts_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void btnTournaments_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        // =====================
        // LOGOUT / EXIT
        // =====================

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Logout?", "Logout", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void AdminPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var d = MessageBox.Show(
                    "Exit application?",
                    "Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (d == DialogResult.Yes)
                    Application.Exit();
                else
                    e.Cancel = true;
            }
        }
    }
}
