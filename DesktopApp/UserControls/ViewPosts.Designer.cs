namespace DesktopApp.UserControls
{
    partial class ViewPosts
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
            dgvPosts = new DataGridView();
            BtnCreate = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvPosts).BeginInit();
            SuspendLayout();
            // 
            // dgvPosts
            // 
            dgvPosts.AllowUserToAddRows = false;
            dgvPosts.AllowUserToDeleteRows = false;
            dgvPosts.AllowUserToResizeColumns = false;
            dgvPosts.AllowUserToResizeRows = false;
            dgvPosts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvPosts.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvPosts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPosts.Location = new Point(14, 54);
            dgvPosts.MultiSelect = false;
            dgvPosts.Name = "dgvPosts";
            dgvPosts.ReadOnly = true;
            dgvPosts.RowTemplate.Height = 25;
            dgvPosts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPosts.Size = new Size(746, 335);
            dgvPosts.TabIndex = 0;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(151, 407);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(85, 37);
            BtnCreate.TabIndex = 1;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(300, 407);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(85, 42);
            btnUpdate.TabIndex = 2;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(445, 404);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(88, 42);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(300, 9);
            label1.Name = "label1";
            label1.Size = new Size(136, 32);
            label1.TabIndex = 4;
            label1.Text = "View Posts";
            // 
            // ViewPosts
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label1);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(BtnCreate);
            Controls.Add(dgvPosts);
            Name = "ViewPosts";
            Size = new Size(774, 465);
            ((System.ComponentModel.ISupportInitialize)dgvPosts).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvPosts;
        private Button BtnCreate;
        private Button btnUpdate;
        private Button btnDelete;
        private Label label1;
    }
}
