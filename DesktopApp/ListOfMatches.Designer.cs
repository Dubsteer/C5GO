namespace DesktopApp
{
    partial class ListOfMatches
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            dgvListOfMatches = new DataGridView();
            btnSaveChanges = new Button();
            nudPlayer1 = new NumericUpDown();
            nudPlayer2 = new NumericUpDown();
            label2 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvListOfMatches).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPlayer1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPlayer2).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(63, 22);
            label1.Name = "label1";
            label1.Size = new Size(138, 25);
            label1.TabIndex = 0;
            label1.Text = "List of Matches";
            // 
            // dgvListOfMatches
            // 
            dgvListOfMatches.AllowUserToAddRows = false;
            dgvListOfMatches.AllowUserToDeleteRows = false;
            dgvListOfMatches.AllowUserToResizeColumns = false;
            dgvListOfMatches.AllowUserToResizeRows = false;
            dgvListOfMatches.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvListOfMatches.Location = new Point(49, 92);
            dgvListOfMatches.Name = "dgvListOfMatches";
            dgvListOfMatches.ReadOnly = true;
            dgvListOfMatches.RowTemplate.Height = 25;
            dgvListOfMatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvListOfMatches.Size = new Size(458, 356);
            dgvListOfMatches.TabIndex = 1;
            // 
            // btnSaveChanges
            // 
            btnSaveChanges.Location = new Point(617, 270);
            btnSaveChanges.Name = "btnSaveChanges";
            btnSaveChanges.Size = new Size(119, 59);
            btnSaveChanges.TabIndex = 2;
            btnSaveChanges.Text = "Save Changes";
            btnSaveChanges.UseVisualStyleBackColor = true;
            btnSaveChanges.Click += btnSaveChanges_Click;
            // 
            // nudPlayer1
            // 
            nudPlayer1.Location = new Point(538, 201);
            nudPlayer1.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            nudPlayer1.Name = "nudPlayer1";
            nudPlayer1.Size = new Size(120, 23);
            nudPlayer1.TabIndex = 3;
            // 
            // nudPlayer2
            // 
            nudPlayer2.Location = new Point(693, 201);
            nudPlayer2.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            nudPlayer2.Name = "nudPlayer2";
            nudPlayer2.Size = new Size(120, 23);
            nudPlayer2.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(538, 162);
            label2.Name = "label2";
            label2.Size = new Size(80, 15);
            label2.TabIndex = 5;
            label2.Text = "Player 1 Score";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(708, 162);
            label3.Name = "label3";
            label3.Size = new Size(80, 15);
            label3.TabIndex = 6;
            label3.Text = "Player 2 Score";
            // 
            // ListOfMatches
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(861, 508);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(nudPlayer2);
            Controls.Add(nudPlayer1);
            Controls.Add(btnSaveChanges);
            Controls.Add(dgvListOfMatches);
            Controls.Add(label1);
            Name = "ListOfMatches";
            Text = "ListOfMatches";
            ((System.ComponentModel.ISupportInitialize)dgvListOfMatches).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPlayer1).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPlayer2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView dgvListOfMatches;
        private Button btnSaveChanges;
        private NumericUpDown nudPlayer1;
        private NumericUpDown nudPlayer2;
        private Label label2;
        private Label label3;
    }
}