namespace DesktopApp
{
    partial class AdminPanel
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLogout = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            viewUsers = new UserControls.ViewUsers();
            tabPage2 = new TabPage();
            viewPosts = new UserControls.ViewPosts();
            createPost = new UserControls.CreatePost();
            updatePost = new UserControls.UpdatePost();
            tabPage3 = new TabPage();
            viewListOfTournaments1 = new UserControls.ViewListOfTournaments();
            addTournament = new UserControls.AddTournament();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(1246, 12);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(75, 23);
            btnLogout.TabIndex = 1;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1228, 718);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(viewUsers);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1220, 690);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "List of Users";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // viewUsers
            // 
            viewUsers.Location = new Point(6, 6);
            viewUsers.Margin = new Padding(3, 4, 3, 4);
            viewUsers.Name = "viewUsers";
            viewUsers.Size = new Size(1208, 676);
            viewUsers.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(viewPosts);
            tabPage2.Controls.Add(createPost);
            tabPage2.Controls.Add(updatePost);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1220, 690);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "List of Posts";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // viewPosts
            // 
            viewPosts.Location = new Point(0, 0);
            viewPosts.Name = "viewPosts";
            viewPosts.Size = new Size(1224, 694);
            viewPosts.TabIndex = 2;
            // 
            // createPost
            // 
            createPost.Location = new Point(-4, 0);
            createPost.Name = "createPost";
            createPost.Size = new Size(1224, 722);
            createPost.TabIndex = 1;
            // 
            // updatePost
            // 
            updatePost.Location = new Point(0, 0);
            updatePost.Name = "updatePost";
            updatePost.Size = new Size(1224, 722);
            updatePost.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(viewListOfTournaments1);
            tabPage3.Controls.Add(addTournament);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1220, 690);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "List of Tournaments";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // viewListOfTournaments1
            // 
            viewListOfTournaments1.Location = new Point(0, 0);
            viewListOfTournaments1.Name = "viewListOfTournaments1";
            viewListOfTournaments1.Size = new Size(1116, 801);
            viewListOfTournaments1.TabIndex = 1;
            // 
            // addTournament
            // 
            addTournament.Location = new Point(0, 0);
            addTournament.Name = "addTournament";
            addTournament.Size = new Size(1224, 690);
            addTournament.TabIndex = 0;
            // 
            // AdminPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1333, 770);
            Controls.Add(tabControl1);
            Controls.Add(btnLogout);
            Name = "AdminPanel";
            Text = "C5G0 app";
            FormClosing += AdminPanel_FormClosing_1;
            Load += AdminPanel_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btnLogout;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private UserControls.ViewUsers viewUsers;
        private UserControls.ViewPosts viewPosts;
        private UserControls.CreatePost createPost;
        private UserControls.UpdatePost updatePost;
        private TabPage tabPage3;
        private UserControls.ViewListOfTournaments viewListOfTournaments1;
        private UserControls.AddTournament addTournament;
    }
}