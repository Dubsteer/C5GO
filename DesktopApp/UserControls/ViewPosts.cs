using LogicLayer;
using DataLayer.Repos;
using LogicLayer.Managers;
using LogicLayer.Models;
using System.Diagnostics;

namespace DesktopApp.UserControls
{
    public partial class ViewPosts : UserControl
    {
        private IConnection connection;
        private PostRepo postRepo;
        private PostManager postManager;
        private List<Control> parentControls;

        public ViewPosts()
        {
            InitializeComponent();

            this.connection = null;
            this.postRepo = null;
            this.postManager = null;
            this.parentControls = new List<Control>();

        }

        public void Setup(IConnection connection, List<Control> parentControls)
        {
            this.connection = connection;
            this.postRepo = new PostRepo(connection);
            this.postManager = new PostManager(postRepo);
            this.parentControls = parentControls;


            if (!DesignMode)
            {
                VisibleChanged += new EventHandler(ViewPosts_VisibleChanged);
                BtnCreate.Click += new EventHandler(BtnCreate_Click);
                btnUpdate.Click += new EventHandler(BtnUpdate_Click);
                btnDelete.Click += new EventHandler(BtnDelete_Click);
            }
        }

        public void ViewPosts_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !Disposing && !DesignMode)
            {
                dgvPosts.DataSource = postManager.GetAllPosts();
            }
        }

        public void RefreshPosts()
        {
            dgvPosts.DataSource = postManager.GetAllPosts();
        }

        public int SelectedPostId
        {
            get
            {
                if (dgvPosts.SelectedRows.Count > 0)
                {
                    var post = (Post)dgvPosts.CurrentRow.DataBoundItem;
                    return (int)post.Id;
                }
                return -1;
            }
        }

        public void BtnCreate_Click(object sender, EventArgs e)
        {
            TabPage currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var createPost = currentTab.Controls.OfType<CreatePost>().FirstOrDefault();

            createPost.Visible = true;
            this.Hide();
        }

        public void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvPosts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a post you want to update.",
                    "Update category",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var post = (Post)dgvPosts.CurrentRow.DataBoundItem;

            var currentTab = parentControls.OfType<TabControl>().First().SelectedTab;
            var updatePost = currentTab.Controls.OfType<UpdatePost>().FirstOrDefault();

            Debug.WriteLine(updatePost);

            updatePost.editingPost = post;
            updatePost.Visible = true;
            this.Hide();
        }

        public void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPosts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a post you want to delete.",
                    "Delete post",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var post = (Post)dgvPosts.CurrentRow.DataBoundItem;

            postManager.DeletePost(post);

            MessageBox.Show("Selected category deleted.",
                    "Delete post",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            RefreshPosts();
        }
    }
}

