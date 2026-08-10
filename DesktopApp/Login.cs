using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace DesktopApp
{
    public partial class Login : Form
    {
        private readonly IConnection connection;
        private readonly UserManager userManager;

        public Login(IConnection connection, UserManager userManager)
        {
            InitializeComponent();

            this.connection = connection;
            this.userManager = userManager;

            tbPassword.PasswordChar = '*';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbUsername.Text.Trim();
            string password = tbPassword.Text.Trim();

            if (!ValidateInput(username, password))
                return;

            User? currentUser = userManager.GetLoginUser(username, password);

            if (currentUser == null || !currentUser.IsAdmin)
            {
                ShowLoginError();
                return;
            }

            OpenAdminPanel(currentUser);
        }

        private bool ValidateInput(string user, string pass)
        {
            var model = new LoginFormModel(user, pass);
            var ctx = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, ctx, errors, true))
            {
                MessageBox.Show(errors[0].ErrorMessage, "Incorrect data",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void ShowLoginError()
        {
            MessageBox.Show("Invalid credentials.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OpenAdminPanel(User currentUser)
        {
            var adminPanel = new AdminPanel(connection, currentUser);
            this.Visible = false;

            adminPanel.ShowDialog(this);

            if (adminPanel.DialogResult == DialogResult.OK)
            {
                this.Visible = true;
                tbUsername.Clear();
                tbPassword.Clear();
            }
        }
    }
}
