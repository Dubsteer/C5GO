using LogicLayer;
using DataLayer.Repos;
using LogicLayer.Managers;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using LogicLayer.FormModels;
using System.ComponentModel.DataAnnotations;

namespace DesktopApp.UserControls
{
    public partial class CreatePost : UserControl
    {
        private IConnection connection;
        private PostRepo postRepo;
        private PostManager postManager;
        private User currentUser;
        private List<Control> parentControls;

        public CreatePost()
        {
            InitializeComponent();
            parentControls = new List<Control>();
        }

        public void Setup(IConnection connection, List<Control> parentControls, User currentUser)
        {
            this.connection = connection;
            this.parentControls = parentControls;
            this.currentUser = currentUser;

            postRepo = new PostRepo(connection);
            postManager = new PostManager(postRepo);

            if (!DesignMode)
            {
                BtnCreate.Click += BtnCreate_Click;
                btnBack.Click += BtnBack_Click;
                VisibleChanged += CreatePost_VisibleChanged;
            }
        }

        private void CreatePost_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !DesignMode)
            {
                tbPost.Clear();
                dgvPost.DataSource = postManager.GetAllPosts();
            }
        }

        public void BtnCreate_Click(object sender, EventArgs e)
        {
            var content = tbPost.Text.Trim();

            var postFormModel = new PostFormModel(content);
            var context = new ValidationContext(postFormModel);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(postFormModel, context, errors, true))
            {
                MessageBox.Show(errors[0].ErrorMessage, "Incorrect data",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                postManager.AddPost(
                    new Post(
                        0,
                        currentUser,
                        "Post",
                        content,
                        DateTime.Now
                    )
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create post",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Post created successfully!", "Create post",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 👉 Vrati se na listu postova
            var currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var viewPosts = (ViewPosts)currentTab.Controls["viewPosts"];

            viewPosts.Visible = true;
            viewPosts.RefreshPosts();
            this.Hide();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            var currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var viewPosts = (ViewPosts)currentTab.Controls["viewPosts"];

            viewPosts.Visible = true;
            this.Hide();
        }
    }
}
