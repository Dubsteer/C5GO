namespace DesktopApp.UserControls
{
    partial class AddTournament
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
            tbDescription = new TextBox();
            tbName = new TextBox();
            btnCreate = new Button();
            btnBack = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // tbDescription
            // 
            tbDescription.Location = new Point(35, 173);
            tbDescription.Multiline = true;
            tbDescription.Name = "tbDescription";
            tbDescription.Size = new Size(471, 282);
            tbDescription.TabIndex = 0;
            // 
            // tbName
            // 
            tbName.Location = new Point(35, 78);
            tbName.Name = "tbName";
            tbName.Size = new Size(172, 23);
            tbName.TabIndex = 1;
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(616, 173);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(182, 63);
            btnCreate.TabIndex = 2;
            btnCreate.Text = "Create Tournament";
            btnCreate.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(616, 254);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(182, 59);
            btnBack.TabIndex = 3;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(35, 47);
            label1.Name = "label1";
            label1.Size = new Size(140, 15);
            label1.TabIndex = 4;
            label1.Text = "Name of the Tournament";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(35, 138);
            label2.Name = "label2";
            label2.Size = new Size(168, 15);
            label2.TabIndex = 5;
            label2.Text = "Description of the Tournament";
            // 
            // AddTournament
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnBack);
            Controls.Add(btnCreate);
            Controls.Add(tbName);
            Controls.Add(tbDescription);
            Name = "AddTournament";
            Size = new Size(863, 533);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbDescription;
        private TextBox tbName;
        private Button btnCreate;
        private Button btnBack;
        private Label label1;
        private Label label2;
    }
}
