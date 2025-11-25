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

        // ?? VAŽNO — detektuje da li je kliknut Logout
        private bool isLoggingOut = false;

        public AdminPanel(IConnection connection, User currentUser)
        {
            InitializeComponent();

            this.connection = connection;
            this.currentUser = currentUser;

            var controls = this.Controls.Cast<Control>().ToList();

            viewUsers.Setup(connection, controls);
            viewPosts.Setup(connection, controls);
            createPost.Setup(connection, controls, currentUser);
            updatePost.Setup(connection, controls, currentUser);

            addTournament.Setup(connection);
            viewListOfTournaments1.Setup(connection);

            addTournament.TournamentCreated += () =>
            {
                addTournament.Hide();
                viewListOfTournaments1.Show();
                viewListOfTournaments1.RefreshTournaments();
            };

            tabControl1.SelectedIndex = 0;
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            addTournament.Hide();
            viewListOfTournaments1.Hide();
        }

        private void btnPosts_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            addTournament.Hide();
            viewListOfTournaments1.Hide();
        }

        private void btnTournaments_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            addTournament.Hide();
            viewListOfTournaments1.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Logout?", "Logout", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // ?? OVO SPRE?AVA EXIT POPUP
                isLoggingOut = true;

                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void AdminPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ?? AKO JE LOGOUT ? PRESKO?I EXIT PORUKU
            if (isLoggingOut)
                return;

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
