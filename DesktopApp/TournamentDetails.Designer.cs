namespace DesktopApp
{
    partial class TournamentDetails
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
            dgvListOfPlayers = new DataGridView();
            dgvListOfMatches = new DataGridView();
            btnEditMatch = new Button();
            btnRunTournament = new Button();
            cbMatchStatus = new ComboBox();
            rdAZ = new RadioButton();
            rdZA = new RadioButton();
            label1 = new Label();
            label2 = new Label();
            datePicker = new DateTimePicker();
            label3 = new Label();
            label4 = new Label();
            nUD = new NumericUpDown();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvListOfPlayers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvListOfMatches).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nUD).BeginInit();
            SuspendLayout();
            // 
            // dgvListOfPlayers
            // 
            dgvListOfPlayers.AllowUserToAddRows = false;
            dgvListOfPlayers.AllowUserToDeleteRows = false;
            dgvListOfPlayers.AllowUserToResizeColumns = false;
            dgvListOfPlayers.AllowUserToResizeRows = false;
            dgvListOfPlayers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvListOfPlayers.Location = new Point(75, 104);
            dgvListOfPlayers.Name = "dgvListOfPlayers";
            dgvListOfPlayers.ReadOnly = true;
            dgvListOfPlayers.RowTemplate.Height = 25;
            dgvListOfPlayers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvListOfPlayers.Size = new Size(433, 379);
            dgvListOfPlayers.TabIndex = 0;
            // 
            // dgvListOfMatches
            // 
            dgvListOfMatches.AllowUserToAddRows = false;
            dgvListOfMatches.AllowUserToDeleteRows = false;
            dgvListOfMatches.AllowUserToResizeColumns = false;
            dgvListOfMatches.AllowUserToResizeRows = false;
            dgvListOfMatches.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            dgvListOfMatches.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvListOfMatches.Location = new Point(562, 104);
            dgvListOfMatches.Name = "dgvListOfMatches";
            dgvListOfMatches.ReadOnly = true;
            dgvListOfMatches.RowTemplate.Height = 25;
            dgvListOfMatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvListOfMatches.Size = new Size(468, 379);
            dgvListOfMatches.TabIndex = 1;
            // 
            // btnEditMatch
            // 
            btnEditMatch.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnEditMatch.Location = new Point(746, 496);
            btnEditMatch.Name = "btnEditMatch";
            btnEditMatch.Size = new Size(125, 54);
            btnEditMatch.TabIndex = 2;
            btnEditMatch.Text = "Edit Match";
            btnEditMatch.UseVisualStyleBackColor = true;
            btnEditMatch.Click += btnEditMatch_Click;
            // 
            // btnRunTournament
            // 
            btnRunTournament.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnRunTournament.Location = new Point(366, 492);
            btnRunTournament.Name = "btnRunTournament";
            btnRunTournament.Size = new Size(125, 57);
            btnRunTournament.TabIndex = 3;
            btnRunTournament.Text = "Run Tournament";
            btnRunTournament.UseVisualStyleBackColor = true;
            btnRunTournament.Click += btnRunTournament_Click;
            // 
            // cbMatchStatus
            // 
            cbMatchStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMatchStatus.Items.AddRange(new object[] { "Open", "InProgress", "Closed", "All" });
            cbMatchStatus.Location = new Point(746, 59);
            cbMatchStatus.Name = "cbMatchStatus";
            cbMatchStatus.Size = new Size(121, 23);
            cbMatchStatus.TabIndex = 4;
            cbMatchStatus.SelectedIndexChanged += cbMatchStatus_SelectedIndexChanged;
            // 
            // rdAZ
            // 
            rdAZ.AutoSize = true;
            rdAZ.Checked = true;
            rdAZ.Location = new Point(143, 59);
            rdAZ.Name = "rdAZ";
            rdAZ.Size = new Size(115, 19);
            rdAZ.TabIndex = 5;
            rdAZ.TabStop = true;
            rdAZ.Text = "Filter Player A - Z";
            rdAZ.UseVisualStyleBackColor = true;
            rdAZ.CheckedChanged += rdAZ_CheckedChanged;
            // 
            // rdZA
            // 
            rdZA.AutoSize = true;
            rdZA.Location = new Point(290, 59);
            rdZA.Name = "rdZA";
            rdZA.Size = new Size(115, 19);
            rdZA.TabIndex = 6;
            rdZA.Text = "Filter Player Z - A";
            rdZA.UseVisualStyleBackColor = true;
            rdZA.CheckedChanged += rdZA_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(192, 20);
            label1.MaximumSize = new Size(175, 15);
            label1.MinimumSize = new Size(175, 15);
            label1.Name = "label1";
            label1.Size = new Size(175, 15);
            label1.TabIndex = 7;
            label1.Text = "Filter Player by Alphabetic order";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(745, 20);
            label2.Name = "label2";
            label2.Size = new Size(126, 15);
            label2.TabIndex = 8;
            label2.Text = "Filter Matchs by Status\r\n";
            // 
            // datePicker
            // 
            datePicker.Location = new Point(144, 492);
            datePicker.Name = "datePicker";
            datePicker.Size = new Size(200, 23);
            datePicker.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(86, 496);
            label3.Name = "label3";
            label3.Size = new Size(52, 15);
            label3.TabIndex = 10;
            label3.Text = "Starts at:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(111, 528);
            label4.Name = "label4";
            label4.Size = new Size(99, 15);
            label4.TabIndex = 11;
            label4.Text = "New match every";
            // 
            // nUD
            // 
            nUD.Location = new Point(217, 526);
            nUD.Maximum = new decimal(new int[] { 70, 0, 0, 0 });
            nUD.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nUD.Name = "nUD";
            nUD.ReadOnly = true;
            nUD.Size = new Size(41, 23);
            nUD.TabIndex = 12;
            nUD.TextAlign = HorizontalAlignment.Center;
            nUD.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(267, 528);
            label5.Name = "label5";
            label5.Size = new Size(50, 15);
            label5.TabIndex = 13;
            label5.Text = "minutes";
            // 
            // TournamentDetails
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1096, 574);
            Controls.Add(label5);
            Controls.Add(nUD);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(datePicker);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(rdZA);
            Controls.Add(rdAZ);
            Controls.Add(cbMatchStatus);
            Controls.Add(btnRunTournament);
            Controls.Add(btnEditMatch);
            Controls.Add(dgvListOfMatches);
            Controls.Add(dgvListOfPlayers);
            Name = "TournamentDetails";
            Text = "TournamentDetails";
            ((System.ComponentModel.ISupportInitialize)dgvListOfPlayers).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvListOfMatches).EndInit();
            ((System.ComponentModel.ISupportInitialize)nUD).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvListOfPlayers;
        private DataGridView dgvListOfMatches;
        private Button btnEditMatch;
        private Button btnRunTournament;
        private ComboBox cbMatchStatus;
        private RadioButton rdAZ;
        private RadioButton rdZA;
        private Label label1;
        private Label label2;
        private DateTimePicker datePicker;
        private Label label3;
        private Label label4;
        private NumericUpDown nUD;
        private Label label5;
    }
}