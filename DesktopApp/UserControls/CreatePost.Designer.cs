namespace DesktopApp.UserControls
{
    partial class CreatePost
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
            label1 = new Label();
            tbPost = new TextBox();
            BtnCreate = new Button();
            dgvPost = new DataGridView();
            btnBack = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvPost).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(64, 39);
            label1.Name = "label1";
            label1.Size = new Size(143, 32);
            label1.TabIndex = 0;
            label1.Text = "Create Post";
            // 
            // tbPost
            // 
            tbPost.Location = new Point(34, 83);
            tbPost.Multiline = true;
            tbPost.Name = "tbPost";
            tbPost.Size = new Size(603, 464);
            tbPost.TabIndex = 1;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(643, 239);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(86, 26);
            BtnCreate.TabIndex = 2;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            // 
            // dgvPost
            // 
            dgvPost.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPost.Location = new Point(735, 83);
            dgvPost.Name = "dgvPost";
            dgvPost.RowTemplate.Height = 25;
            dgvPost.Size = new Size(341, 464);
            dgvPost.TabIndex = 4;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(643, 294);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(86, 30);
            btnBack.TabIndex = 5;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += BtnBack_Click;
            // 
            // CreatePost
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnBack);
            Controls.Add(dgvPost);
            Controls.Add(BtnCreate);
            Controls.Add(tbPost);
            Controls.Add(label1);
            Name = "CreatePost";
            Size = new Size(1117, 649);
            ((System.ComponentModel.ISupportInitialize)dgvPost).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox tbPost;
        private Button BtnCreate;
        private DataGridView dgvPost;
        private Button btnBack;
    }
}
