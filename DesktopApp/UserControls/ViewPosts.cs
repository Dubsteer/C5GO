using LogicLayer;
using DataLayer.Repos;
using LogicLayer.Managers;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

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
        }
        public void RefreshPosts()
        {
            dgvPosts.DataSource = postManager.GetAllPosts();
        }

        public void Setup(IConnection connection, List<Control> parentControls)
        {
            this.connection = connection;
            this.parentControls = parentControls;

            postRepo = new PostRepo(connection);
            postManager = new PostManager(postRepo);

            VisibleChanged += (s, e) =>
            {
                if (Visible)
                    dgvPosts.DataSource = postManager.GetAllPosts();
            };

            BtnCreate.Click += BtnCreate_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            var tab = parentControls.OfType<TabControl>().First().SelectedTab;
            var cp = tab.Controls.OfType<CreatePost>().First();

            cp.Visible = true;
            this.Visible = false;
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvPosts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a post.");
                return;
            }

            var post = (Post)dgvPosts.CurrentRow.DataBoundItem;

            var tab = parentControls.OfType<TabControl>().First().SelectedTab;
            var up = tab.Controls.OfType<UpdatePost>().First();

            up.editingPost = post;

            up.Visible = true;
            this.Visible = false;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPosts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a post.");
                return;
            }

            var post = (Post)dgvPosts.CurrentRow.DataBoundItem;

            postManager.DeletePost(post);

            dgvPosts.DataSource = postManager.GetAllPosts();
            MessageBox.Show("Post deleted.");
        }
    }
}
