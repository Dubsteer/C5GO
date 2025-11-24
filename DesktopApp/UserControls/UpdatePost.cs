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
    public partial class UpdatePost : UserControl
    {
        public Post editingPost;
        private PostRepo postRepo;
        private PostManager postManager;
        private IConnection connection;
        private User currentUser;
        private List<Control> parentControls;

        public UpdatePost()
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
                VisibleChanged += UpdatePost_VisibleChanged;
                btnUpdate.Click += BtnUpdate_Click;
                btnBack.Click += BtnBack_Click;
            }
        }

        private void UpdatePost_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible || editingPost == null) return;

            // učitaj trenutni post
            var fresh = postManager.GetPostById(editingPost.Id);
            tbUpdatePost.Text = fresh.Content;
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var content = tbUpdatePost.Text.Trim();

            var postFormModel = new PostFormModel(content);
            var context = new ValidationContext(postFormModel);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(postFormModel, context, errors, true))
            {
                MessageBox.Show(errors[0].ErrorMessage,
                    "Validation error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            editingPost.Content = content;
            editingPost.User = currentUser;

            try
            {
                postManager.UpdatePost(editingPost);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update post",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Post updated successfully!", "Update post",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 👉 Vrati nazad na listu
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
