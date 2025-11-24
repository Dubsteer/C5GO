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
            dgvPosts.Location = new Point(16, 72);
            dgvPosts.Margin = new Padding(3, 4, 3, 4);
            dgvPosts.MultiSelect = false;
            dgvPosts.Name = "dgvPosts";
            dgvPosts.ReadOnly = true;
            dgvPosts.RowHeadersWidth = 51;
            dgvPosts.RowTemplate.Height = 25;
            dgvPosts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPosts.Size = new Size(925, 447);
            dgvPosts.TabIndex = 0;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(173, 543);
            BtnCreate.Margin = new Padding(3, 4, 3, 4);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(97, 49);
            BtnCreate.TabIndex = 1;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(343, 543);
            btnUpdate.Margin = new Padding(3, 4, 3, 4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(97, 56);
            btnUpdate.TabIndex = 2;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(509, 539);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(101, 56);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(343, 12);
            label1.Name = "label1";
            label1.Size = new Size(168, 41);
            label1.TabIndex = 4;
            label1.Text = "View Posts";
            // 
            // ViewPosts
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label1);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(BtnCreate);
            Controls.Add(dgvPosts);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ViewPosts";
            Size = new Size(1295, 711);
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
