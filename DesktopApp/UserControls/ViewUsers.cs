using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicLayer.Managers;
using LogicLayer;
using DataLayer.Repos;
using LogicLayer.Models;

namespace DesktopApp.UserControls
{
    public partial class ViewUsers : UserControl
    {
        private IConnection connection;
        private UserRepo userRepo;
        private UserManager userManager;
        private PlayerRepo playerRepo;
        private PlayerManager playerManager;
        private List<Control> parentControls;

        public ViewUsers()
        {
            InitializeComponent();

            this.parentControls = new List<Control>();
            this.connection = null;
            this.userRepo = null;
            this.userManager = null;
            this.playerRepo = null;
            this.playerManager = null;

        }

        public void Setup(IConnection connection, List<Control> parentControls)
        {
            this.connection = connection;
            this.userRepo = new UserRepo(connection);
            this.userManager = new UserManager(userRepo);
            this.playerRepo = new PlayerRepo(connection);
            this.playerManager = new PlayerManager(playerRepo);
            this.parentControls = parentControls;

            if (!DesignMode)
            {
                VisibleChanged += new EventHandler(ViewUsers_VisibleChanged);
            }
        }

        public void ViewUsers_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing && !DesignMode)
            {
                dgvUsers.DataSource = userManager.GetAllUsers();
            }
        }
        private void AddUserToListView(User user)
        {
            ListViewItem item = new ListViewItem(user.Id.ToString());

            item.Tag = user;
            item.SubItems.Add(user.Username);
            item.SubItems.Add(user.Firstname);
            item.SubItems.Add(user.Lastname);

            dgvUsers.RowCount.ToString();
        }

        public void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user you want to delete.",
                   "Delete product",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Warning);
                return;
            }

            var user = (User)dgvUsers.CurrentRow.DataBoundItem;

            userManager.DeleteUser(user);

            MessageBox.Show("Selected category deleted.",
                    "Delete user",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            dgvUsers.DataSource = userManager.GetAllUsers();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = tbSearchUser.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a word for search");
                return;
            }

            dgvUsers.ClearSelection();

            try
            {
                IList<User> foundUsers = userManager.SearchUser(searchTerm);

                dgvUsers.DataSource = null;

                if (foundUsers.Count == 0)
                {
                    MessageBox.Show("No results found");
                }
                else
                {
                    dgvUsers.DataSource = foundUsers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching for users: " + ex.Message);
            }
        }

        private void btnRemovePlayer_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a player you want to delete role for.",
                    "Delete player role",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var currentUser = (User)dgvUsers.CurrentRow.DataBoundItem;
            var currentPlayer = new Player((int)currentUser.Id, currentUser.Firstname, currentUser.Lastname, currentUser.Age, currentUser.Username, currentUser.Gmail, currentUser.Password, "0", currentUser.IsAdmin);

            try
            {
                playerRepo.DeletePlayerRole(currentPlayer);

                MessageBox.Show("Player role deleted.",
                    "Delete user role",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Delete user role",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            dgvUsers.DataSource = userManager.GetAllUsers();
        }
    }
}
