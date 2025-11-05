namespace DesktopApp.UserControls
{
    partial class UpdatePost
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
            tbUpdatePost = new TextBox();
            btnUpdate = new Button();
            btnBack = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(43, 9);
            label1.Name = "label1";
            label1.Size = new Size(152, 32);
            label1.TabIndex = 0;
            label1.Text = "Update Post";
            // 
            // tbUpdatePost
            // 
            tbUpdatePost.Location = new Point(53, 64);
            tbUpdatePost.Multiline = true;
            tbUpdatePost.Name = "tbUpdatePost";
            tbUpdatePost.Size = new Size(623, 442);
            tbUpdatePost.TabIndex = 1;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(751, 303);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(105, 47);
            btnUpdate.TabIndex = 3;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(751, 371);
            btnBack.Margin = new Padding(3, 2, 3, 2);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(105, 47);
            btnBack.TabIndex = 4;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += BtnBack_Click;
            // 
            // UpdatePost
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnBack);
            Controls.Add(btnUpdate);
            Controls.Add(tbUpdatePost);
            Controls.Add(label1);
            Name = "UpdatePost";
            Size = new Size(914, 560);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox tbUpdatePost;
        private Button btnUpdate;
        private Button btnBack;
    }
}
