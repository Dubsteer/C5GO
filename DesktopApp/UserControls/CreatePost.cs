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
        private List<Control> parentControls;
        public User currentUser;
        

        public CreatePost()
        {
            InitializeComponent();

            this.connection = null;
            this.postRepo = null;
            this.postManager = null;
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
                VisibleChanged += new EventHandler(createPost_VisibleChanged);
                BtnCreate.Click += new EventHandler(BtnCreate_Click);
                btnBack.Click += new EventHandler(BtnBack_Click);
            }
        }

        public void createPost_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing && !DesignMode)
            {
                tbPost.Clear();
                dgvPost.DataSource = postManager.GetAllPosts();
            }
        }

        public void BtnCreate_Click(object sender, EventArgs e)
        {
            var content = tbPost.Text.Trim();

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

            try
            {
                postManager.CreatePost(
                    new Post(
                         null, 
                         this.currentUser,
                         content,
                         DateTime.Now
                         ));
            }
            catch (PostNameAlreadyInUseExepction ex)
            {
                MessageBox.Show(ex.Message,
                    "Create post",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Create post",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Post created successfully.",
                    "Create post",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            var currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var createPost = (CreatePost)currentTab.Controls["createPost"];
            if (createPost != null)
            {
                createPost.Visible = true;
                this.Hide();
            }
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