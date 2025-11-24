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
            dgvUsers.Location = new Point(19, 91);
            dgvUsers.Margin = new Padding(3, 4, 3, 4);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.ReadOnly = true;
            dgvUsers.RowHeadersWidth = 51;
            dgvUsers.RowTemplate.Height = 25;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.Size = new Size(1156, 759);
            dgvUsers.TabIndex = 0;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(1220, 249);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(149, 67);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(273, 32);
            btnSearch.Margin = new Padding(3, 4, 3, 4);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(107, 48);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "Search Users";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // tbSearchUser
            // 
            tbSearchUser.Location = new Point(41, 43);
            tbSearchUser.Margin = new Padding(3, 4, 3, 4);
            tbSearchUser.Name = "tbSearchUser";
            tbSearchUser.Size = new Size(209, 27);
            tbSearchUser.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(59, 19);
            label1.Name = "label1";
            label1.Size = new Size(172, 20);
            label1.TabIndex = 5;
            label1.Text = "Search user by username";
            // 
            // btnRemovePlayer
            // 
            btnRemovePlayer.Location = new Point(1220, 338);
            btnRemovePlayer.Margin = new Padding(3, 4, 3, 4);
            btnRemovePlayer.Name = "btnRemovePlayer";
            btnRemovePlayer.Size = new Size(144, 75);
            btnRemovePlayer.TabIndex = 6;
            btnRemovePlayer.Text = "Remove Player Role";
            btnRemovePlayer.UseVisualStyleBackColor = true;
            btnRemovePlayer.Click += btnRemovePlayer_Click;
            // 
            // ViewUsers
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnRemovePlayer);
            Controls.Add(label1);
            Controls.Add(tbSearchUser);
            Controls.Add(btnSearch);
            Controls.Add(btnDelete);
            Controls.Add(dgvUsers);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ViewUsers";
            Size = new Size(1420, 960);
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
