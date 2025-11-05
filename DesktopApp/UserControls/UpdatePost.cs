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
        private Post post;
        public User currentUser;

        private IConnection connection;
        private UserRepo userRepo;
        private UserManager userManager;
        private PostRepo postRepo;
        private PostManager postManager;
        private List<Control> parentControls;
        public UpdatePost()
        {
            InitializeComponent();

            this.connection = null;
            this.postManager = null;
            this.postRepo = null;
            this.parentControls = new List<Control>();
        }

        public void Setup(IConnection connection, List<Control> parentControls, User currentUser)
        {
            this.currentUser = currentUser;

            this.connection = connection;
            this.parentControls = parentControls;
            this.postRepo = new PostRepo(connection);
            this.postManager = new PostManager(postRepo);

            if (!DesignMode)
            {
                VisibleChanged += new EventHandler(UpdatePost_VisibleChanged);
                btnUpdate.Click += new EventHandler(BtnUpdate_Click);
                btnBack.Click += new EventHandler(BtnBack_Click);

            }
        }

        public void UpdatePost_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing && !DesignMode)
            {
                if (editingPost is not null)
                {
                    post = postManager.GetPostById(editingPost.Id.Value);
                    tbUpdatePost.Text = editingPost.Content;
                }
            }
        }

        public void BtnUpdate_Click(object sender, EventArgs e)
        {
            var content = tbUpdatePost.Text.Trim();

            var postFormModel = new PostFormModel(content);

            var context = new ValidationContext(postFormModel, null, null);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(postFormModel, context, errors, true))
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

            var changedPost = editingPost;
            changedPost.Content = content;
            changedPost.User = currentUser;

            try
            {
                postManager.UpdatePost(changedPost);
            }
            catch (PostNameAlreadyInUseExepction ex)
            {
                MessageBox.Show(ex.Message,
                    "Update post",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Update post",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Post updated successfully.",
                    "Update post",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            var currentTab = parentControls.OfType<TabControl>().FirstOrDefault().SelectedTab;
            var viewPosts = (ViewPosts)currentTab.Controls["viewPosts"];

                viewPosts.Visible = true;
                this.Hide();
            
            
        }

        public void BtnBack_Click(object sender, EventArgs e)
        {
            var currentTab = parentControls.OfType<TabControl>().FirstOrDefault().SelectedTab;
            var viewPosts = (ViewPosts)currentTab.Controls["viewPosts"];

            viewPosts.Visible = true;
            this.Hide();

        }
    }
}

