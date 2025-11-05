namespace DesktopApp.UserControls
{
    partial class ViewUsers
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvUsers = new DataGridView();
            btnDelete = new Button();
            btnSearch = new Button();
            tbSearchUser = new TextBox();
            label1 = new Label();
            btnRemovePlayer = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            SuspendLayout();
            // 
            // dgvUsers
            // 
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.AllowUserToDeleteRows = false;
            dgvUsers.AllowUserToResizeColumns = false;
            dgvUsers.AllowUserToResizeRows = false;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvUsers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsers.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvUsers.Location = new Point(17, 68);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.ReadOnly = true;
            dgvUsers.RowTemplate.Height = 25;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.Size = new Size(856, 569);
            dgvUsers.TabIndex = 0;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(910, 196);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(130, 50);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(239, 24);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(94, 36);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "Search Users";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // tbSearchUser
            // 
            tbSearchUser.Location = new Point(36, 32);
            tbSearchUser.Name = "tbSearchUser";
            tbSearchUser.Size = new Size(183, 23);
            tbSearchUser.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(52, 14);
            label1.Name = "label1";
            label1.Size = new Size(138, 15);
            label1.TabIndex = 5;
            label1.Text = "Search user by username";
            // 
            // btnRemovePlayer
            // 
            btnRemovePlayer.Location = new Point(914, 261);
            btnRemovePlayer.Name = "btnRemovePlayer";
            btnRemovePlayer.Size = new Size(126, 56);
            btnRemovePlayer.TabIndex = 6;
            btnRemovePlayer.Text = "Remove Player Role";
            btnRemovePlayer.UseVisualStyleBackColor = true;
            btnRemovePlayer.Click += btnRemovePlayer_Click;
            // 
            // ViewUsers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnRemovePlayer);
            Controls.Add(label1);
            Controls.Add(tbSearchUser);
            Controls.Add(btnSearch);
            Controls.Add(btnDelete);
            Controls.Add(dgvUsers);
            Name = "ViewUsers";
            Size = new Size(1078, 720);
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvUsers;
        private Button btnDelete;
        private Button btnSearch;
        private TextBox tbSearchUser;
        private Label label1;
        private Button btnRemovePlayer;
    }
}
