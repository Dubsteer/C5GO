using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using LogicLayer.Models;
using LogicLayer.Managers;
using LogicLayer.FormModels;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace DesktopApp
{
    public partial class Login : Form
    {
        private readonly IConnection connection;
        private readonly UserRepo userRepo;
        private readonly UserManager userManager;

        public Login()
        {
            InitializeComponent();

            this.connection = new MySQLConnection("server=127.0.0.1;port=3306;user id=root;password=1234;database=local_dtb;SslMode=none;");

            try
            {
                connection.Open();
                Debug.WriteLine("Connected to database successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        "Connection to database cannot be established. Check your internet connection and try again.",
                        "C5G0 news ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                Debug.WriteLine(ex.Message);
                // exit the app with exit code that doesn't correspond to successful exit
                Environment.Exit(1);
            }
            userRepo = new UserRepo(connection);
            userManager = new UserManager(userRepo);
            tbPassword.PasswordChar = '*';
        }

        public void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbUsername.Text.Trim();
            string password = tbPassword.Text.Trim();


            var loginFormModel = new LoginFormModel(username, password);

            var context = new ValidationContext(loginFormModel, null, null);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(loginFormModel, context, errors, true))
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
            User? currentUser = userManager.GetLoginUser(username, password);
            try
            {
                if (currentUser is null || !currentUser.IsAdmin)
                {
                    MessageBox.Show(
                        "No user found with provided credentials. Please check and try again.",
                        "Incorrect credentials",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "No user found with provided credentials. Please check and try again.",
                       "Incorrect credentials",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                Debug.WriteLine(ex.Message);
                return;

            }


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
        private void Login_FormClosing(object sender, FormClosingEventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            string username = "admin";
            string password = "admin";


            var loginFormModel = new LoginFormModel(username, password);

            var context = new ValidationContext(loginFormModel, null, null);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(loginFormModel, context, errors, true))
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
            User? currentUser = userManager.GetLoginUser(username, password);
            try
            {
                if (currentUser is null || !currentUser.IsAdmin)
                {
                    MessageBox.Show(
                        "No user found with provided credentials. Please check and try again.",
                        "Incorrect credentials",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "No user found with provided credentials. Please check and try again.",
                       "Incorrect credentials",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                Debug.WriteLine(ex.Message);
                return;

            }


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
