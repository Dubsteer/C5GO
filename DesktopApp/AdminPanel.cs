using System;
using LogicLayer;
using LogicLayer.Models;
using LogicLayer.Managers;
using DesktopApp.UserControls;

namespace DesktopApp
{
    public partial class AdminPanel : Form
    {
        private IConnection connection;
        public User currentUser;

        public AdminPanel(IConnection connection, User currentUser)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            this.connection = connection;

            var controls = this.Controls.Cast<Control>().ToList();


            viewUsers.Setup(connection, controls);
            viewListOfTournaments1.Setup(connection, controls);
            addTournament.Setup(connection, controls);
            viewPosts.Setup(connection, controls);
            createPost.Setup(connection, controls, currentUser);
            updatePost.Setup(connection, controls, currentUser);


        }

        private void AdminPanel_Load(object sender, EventArgs e)
        {
            viewUsers.Visible = true;

            viewListOfTournaments1.Visible = true;
            addTournament.Visible = false;

            viewPosts.Visible = true;
            createPost.Visible = false;
            updatePost.Visible = true;
        }
        private void AdminPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Are you sure you want to exit?",
                    "Exit application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                    Application.Exit();
                else
                    e.Cancel = true;
            }
        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to log out?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                this.currentUser = null;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void AdminPanel_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Terminate the application
                Application.Exit();
            }
        }
    }
}