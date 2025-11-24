namespace DesktopApp
{
    partial class AdminPanel
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            btnLogout = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            viewUsers = new DesktopApp.UserControls.ViewUsers();
            tabPage2 = new TabPage();
            viewPosts = new DesktopApp.UserControls.ViewPosts();
            createPost = new DesktopApp.UserControls.CreatePost();
            updatePost = new DesktopApp.UserControls.UpdatePost();
            tabPage3 = new TabPage();
            viewListOfTournaments1 = new DesktopApp.UserControls.ViewListOfTournaments();
            addTournament = new DesktopApp.UserControls.AddTournament();
            btnUsers = new Button();
            btnPosts = new Button();
            btnTournaments = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(1420, 15);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(90, 30);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Logout";
            btnLogout.Click += btnLogout_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(15, 15);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1500, 950);
            tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(viewUsers);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(1492, 917);
            tabPage1.TabIndex = 0;
            // 
            // viewUsers
            // 
            viewUsers.Dock = DockStyle.Fill;
            viewUsers.Location = new Point(0, 0);
            viewUsers.Margin = new Padding(3, 4, 3, 4);
            viewUsers.Name = "viewUsers";
            viewUsers.Size = new Size(1492, 917);
            viewUsers.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(viewPosts);
            tabPage2.Controls.Add(createPost);
            tabPage2.Controls.Add(updatePost);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(1492, 917);
            tabPage2.TabIndex = 1;
            // 
            // viewPosts
            // 
            viewPosts.Dock = DockStyle.Fill;
            viewPosts.Location = new Point(0, 0);
            viewPosts.Margin = new Padding(3, 4, 3, 4);
            viewPosts.Name = "viewPosts";
            viewPosts.Size = new Size(1492, 917);
            viewPosts.TabIndex = 0;
            // 
            // createPost
            // 
            createPost.Location = new Point(0, 0);
            createPost.Margin = new Padding(3, 4, 3, 4);
            createPost.Name = "createPost";
            createPost.Size = new Size(1277, 865);
            createPost.TabIndex = 1;
            // 
            // updatePost
            // 
            updatePost.Location = new Point(0, 0);
            updatePost.Margin = new Padding(3, 4, 3, 4);
            updatePost.Name = "updatePost";
            updatePost.Size = new Size(1045, 747);
            updatePost.TabIndex = 2;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(viewListOfTournaments1);
            tabPage3.Controls.Add(addTournament);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1492, 917);
            tabPage3.TabIndex = 2;
            // 
            // viewListOfTournaments1
            // 
            viewListOfTournaments1.Dock = DockStyle.Fill;
            viewListOfTournaments1.Location = new Point(0, 0);
            viewListOfTournaments1.Margin = new Padding(3, 4, 3, 4);
            viewListOfTournaments1.Name = "viewListOfTournaments1";
            viewListOfTournaments1.Size = new Size(1492, 917);
            viewListOfTournaments1.TabIndex = 0;
            // 
            // addTournament
            // 
            addTournament.Location = new Point(0, 0);
            addTournament.Margin = new Padding(3, 4, 3, 4);
            addTournament.Name = "addTournament";
            addTournament.Size = new Size(986, 711);
            addTournament.TabIndex = 1;
            // 
            // btnUsers
            // 
            btnUsers.Location = new Point(15, 15);
            btnUsers.Name = "btnUsers";
            btnUsers.Size = new Size(150, 35);
            btnUsers.TabIndex = 1;
            btnUsers.Text = "Users";
            btnUsers.Click += btnUsers_Click;
            // 
            // btnPosts
            // 
            btnPosts.Location = new Point(180, 15);
            btnPosts.Name = "btnPosts";
            btnPosts.Size = new Size(150, 35);
            btnPosts.TabIndex = 2;
            btnPosts.Text = "Posts";
            btnPosts.Click += btnPosts_Click;
            // 
            // btnTournaments
            // 
            btnTournaments.Location = new Point(345, 15);
            btnTournaments.Name = "btnTournaments";
            btnTournaments.Size = new Size(150, 35);
            btnTournaments.TabIndex = 3;
            btnTournaments.Text = "Tournaments";
            btnTournaments.Click += btnTournaments_Click;
            // 
            // AdminPanel
            // 
            ClientSize = new Size(1540, 1040);
            Controls.Add(btnLogout);
            Controls.Add(btnUsers);
            Controls.Add(btnPosts);
            Controls.Add(btnTournaments);
            Controls.Add(tabControl1);
            Name = "AdminPanel";
            Text = "C5G0 Admin Panel";
            FormClosing += AdminPanel_FormClosing;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Button btnLogout;
        private Button btnUsers;
        private Button btnPosts;
        private Button btnTournaments;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private UserControls.ViewUsers viewUsers;
        private UserControls.ViewPosts viewPosts;
        private UserControls.CreatePost createPost;
        private UserControls.UpdatePost updatePost;
        private UserControls.ViewListOfTournaments viewListOfTournaments1;
        private UserControls.AddTournament addTournament;
    }
}
