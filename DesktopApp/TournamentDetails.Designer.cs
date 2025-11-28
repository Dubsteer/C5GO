namespace DesktopApp
{
    partial class TournamentDetails
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTournamentName = new System.Windows.Forms.Label();
            this.lblTournamentStatus = new System.Windows.Forms.Label();
            this.lblTournamentType = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPlayersTeams = new System.Windows.Forms.TabPage();
            this.dgvTeams = new System.Windows.Forms.DataGridView();
            this.dgvListOfPlayers = new System.Windows.Forms.DataGridView();
            this.rdZA = new System.Windows.Forms.RadioButton();
            this.rdAZ = new System.Windows.Forms.RadioButton();
            this.lblFilterPlayers = new System.Windows.Forms.Label();
            this.tabMatches = new System.Windows.Forms.TabPage();
            this.dgvTeamMatches = new System.Windows.Forms.DataGridView();
            this.dgvListOfMatches = new System.Windows.Forms.DataGridView();
            this.cbMatchStatus = new System.Windows.Forms.ComboBox();
            this.lblFilterMatches = new System.Windows.Forms.Label();
            this.btnGenerateBracket = new System.Windows.Forms.Button();
            this.btnEditMatch = new System.Windows.Forms.Button();
            this.btnViewBracket = new System.Windows.Forms.Button();

            // 🔥 NEW BUTTON
            this.btnManageTeams = new System.Windows.Forms.Button();

            this.tabControl.SuspendLayout();
            this.tabPlayersTeams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListOfPlayers)).BeginInit();
            this.tabMatches.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeamMatches)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListOfMatches)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTournamentName
            // 
            this.lblTournamentName.AutoSize = true;
            this.lblTournamentName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTournamentName.Location = new System.Drawing.Point(30, 20);
            this.lblTournamentName.Name = "lblTournamentName";
            this.lblTournamentName.Size = new System.Drawing.Size(139, 21);
            this.lblTournamentName.TabIndex = 0;
            this.lblTournamentName.Text = "Tournament: ---";
            // 
            // lblTournamentStatus
            // 
            this.lblTournamentStatus.AutoSize = true;
            this.lblTournamentStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTournamentStatus.Location = new System.Drawing.Point(350, 20);
            this.lblTournamentStatus.Name = "lblTournamentStatus";
            this.lblTournamentStatus.Size = new System.Drawing.Size(88, 21);
            this.lblTournamentStatus.TabIndex = 1;
            this.lblTournamentStatus.Text = "Status: ---";
            // 
            // lblTournamentType
            // 
            this.lblTournamentType.AutoSize = true;
            this.lblTournamentType.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTournamentType.Location = new System.Drawing.Point(600, 20);
            this.lblTournamentType.Name = "lblTournamentType";
            this.lblTournamentType.Size = new System.Drawing.Size(78, 21);
            this.lblTournamentType.TabIndex = 2;
            this.lblTournamentType.Text = "Type: ---";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPlayersTeams);
            this.tabControl.Controls.Add(this.tabMatches);
            this.tabControl.Location = new System.Drawing.Point(20, 60);
            this.tabControl.Name = "tabControl";
            this.tabControl.Size = new System.Drawing.Size(1050, 450);
            this.tabControl.TabIndex = 3;
            // 
            // tabPlayersTeams
            // 
            this.tabPlayersTeams.Controls.Add(this.dgvTeams);
            this.tabPlayersTeams.Controls.Add(this.dgvListOfPlayers);
            this.tabPlayersTeams.Controls.Add(this.rdZA);
            this.tabPlayersTeams.Controls.Add(this.rdAZ);
            this.tabPlayersTeams.Controls.Add(this.lblFilterPlayers);
            this.tabPlayersTeams.Location = new System.Drawing.Point(4, 24);
            this.tabPlayersTeams.Name = "tabPlayersTeams";
            this.tabPlayersTeams.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlayersTeams.Size = new System.Drawing.Size(1042, 422);
            this.tabPlayersTeams.TabIndex = 0;
            this.tabPlayersTeams.Text = "Players / Teams";
            this.tabPlayersTeams.UseVisualStyleBackColor = true;
            // 
            // dgvTeams
            // 
            this.dgvTeams.AllowUserToAddRows = false;
            this.dgvTeams.AllowUserToDeleteRows = false;
            this.dgvTeams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTeams.Location = new System.Drawing.Point(20, 15);
            this.dgvTeams.Name = "dgvTeams";
            this.dgvTeams.ReadOnly = true;
            this.dgvTeams.RowTemplate.Height = 25;
            this.dgvTeams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTeams.Size = new System.Drawing.Size(1000, 380);
            this.dgvTeams.TabIndex = 6;
            // 
            // dgvListOfPlayers
            // 
            this.dgvListOfPlayers.AllowUserToAddRows = false;
            this.dgvListOfPlayers.AllowUserToDeleteRows = false;
            this.dgvListOfPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListOfPlayers.Location = new System.Drawing.Point(20, 50);
            this.dgvListOfPlayers.Name = "dgvListOfPlayers";
            this.dgvListOfPlayers.ReadOnly = true;
            this.dgvListOfPlayers.RowTemplate.Height = 25;
            this.dgvListOfPlayers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvListOfPlayers.Size = new System.Drawing.Size(1000, 350);
            this.dgvListOfPlayers.TabIndex = 5;
            // 
            // rdZA
            // 
            this.rdZA.AutoSize = true;
            this.rdZA.Location = new System.Drawing.Point(180, 12);
            this.rdZA.Name = "rdZA";
            this.rdZA.Size = new System.Drawing.Size(47, 19);
            this.rdZA.TabIndex = 4;
            this.rdZA.TabStop = true;
            this.rdZA.Text = "Z - A";
            this.rdZA.UseVisualStyleBackColor = true;
            this.rdZA.CheckedChanged += new System.EventHandler(this.rdZA_CheckedChanged);
            // 
            // rdAZ
            // 
            this.rdAZ.AutoSize = true;
            this.rdAZ.Location = new System.Drawing.Point(120, 12);
            this.rdAZ.Name = "rdAZ";
            this.rdAZ.Size = new System.Drawing.Size(47, 19);
            this.rdAZ.TabIndex = 3;
            this.rdAZ.TabStop = true;
            this.rdAZ.Text = "A - Z";
            this.rdAZ.UseVisualStyleBackColor = true;
            this.rdAZ.CheckedChanged += new System.EventHandler(this.rdAZ_CheckedChanged);
            // 
            // lblFilterPlayers
            // 
            this.lblFilterPlayers.AutoSize = true;
            this.lblFilterPlayers.Location = new System.Drawing.Point(20, 14);
            this.lblFilterPlayers.Name = "lblFilterPlayers";
            this.lblFilterPlayers.Size = new System.Drawing.Size(79, 15);
            this.lblFilterPlayers.TabIndex = 2;
            this.lblFilterPlayers.Text = "Filter Players:";
            // 
            // tabMatches
            // 
            this.tabMatches.Controls.Add(this.dgvTeamMatches);
            this.tabMatches.Controls.Add(this.dgvListOfMatches);
            this.tabMatches.Controls.Add(this.cbMatchStatus);
            this.tabMatches.Controls.Add(this.lblFilterMatches);
            this.tabMatches.Location = new System.Drawing.Point(4, 24);
            this.tabMatches.Name = "tabMatches";
            this.tabMatches.Padding = new System.Windows.Forms.Padding(3);
            this.tabMatches.Size = new System.Drawing.Size(1042, 422);
            this.tabMatches.TabIndex = 1;
            this.tabMatches.Text = "Matches";
            this.tabMatches.UseVisualStyleBackColor = true;
            // 
            // dgvTeamMatches
            // 
            this.dgvTeamMatches.AllowUserToAddRows = false;
            this.dgvTeamMatches.AllowUserToDeleteRows = false;
            this.dgvTeamMatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTeamMatches.Location = new System.Drawing.Point(20, 15);
            this.dgvTeamMatches.Name = "dgvTeamMatches";
            this.dgvTeamMatches.ReadOnly = true;
            this.dgvTeamMatches.RowTemplate.Height = 25;
            this.dgvTeamMatches.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTeamMatches.Size = new System.Drawing.Size(1000, 380);
            this.dgvTeamMatches.TabIndex = 7;
            // 
            // dgvListOfMatches
            // 
            this.dgvListOfMatches.AllowUserToAddRows = false;
            this.dgvListOfMatches.AllowUserToDeleteRows = false;
            this.dgvListOfMatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListOfMatches.Location = new System.Drawing.Point(20, 50);
            this.dgvListOfMatches.Name = "dgvListOfMatches";
            this.dgvListOfMatches.ReadOnly = true;
            this.dgvListOfMatches.RowTemplate.Height = 25;
            this.dgvListOfMatches.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvListOfMatches.Size = new System.Drawing.Size(1000, 350);
            this.dgvListOfMatches.TabIndex = 6;
            // 
            // cbMatchStatus
            // 
            this.cbMatchStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMatchStatus.FormattingEnabled = true;
            this.cbMatchStatus.Items.AddRange(new object[] {
            "All",
            "Open",
            "InProgress",
            "Closed"});
            this.cbMatchStatus.Location = new System.Drawing.Point(120, 12);
            this.cbMatchStatus.Name = "cbMatchStatus";
            this.cbMatchStatus.Size = new System.Drawing.Size(121, 23);
            this.cbMatchStatus.TabIndex = 5;
            this.cbMatchStatus.SelectedIndexChanged += new System.EventHandler(this.cbMatchStatus_SelectedIndexChanged);
            // 
            // lblFilterMatches
            // 
            this.lblFilterMatches.AutoSize = true;
            this.lblFilterMatches.Location = new System.Drawing.Point(20, 14);
            this.lblFilterMatches.Name = "lblFilterMatches";
            this.lblFilterMatches.Size = new System.Drawing.Size(83, 15);
            this.lblFilterMatches.TabIndex = 4;
            this.lblFilterMatches.Text = "Filter Matches:";
            // 
            // btnGenerateBracket
            // 
            this.btnGenerateBracket.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGenerateBracket.Location = new System.Drawing.Point(250, 520);
            this.btnGenerateBracket.Name = "btnGenerateBracket";
            this.btnGenerateBracket.Size = new System.Drawing.Size(150, 40);
            this.btnGenerateBracket.TabIndex = 4;
            this.btnGenerateBracket.Text = "Generate Bracket";
            this.btnGenerateBracket.UseVisualStyleBackColor = true;
            this.btnGenerateBracket.Click += new System.EventHandler(this.btnGenerateBracket_Click);
            // 
            // btnViewBracket
            // 
            this.btnViewBracket.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnViewBracket.Location = new System.Drawing.Point(410, 520);
            this.btnViewBracket.Name = "btnViewBracket";
            this.btnViewBracket.Size = new System.Drawing.Size(140, 40);
            this.btnViewBracket.TabIndex = 6;
            this.btnViewBracket.Text = "View Bracket";
            this.btnViewBracket.UseVisualStyleBackColor = true;
            this.btnViewBracket.Click += new System.EventHandler(this.btnViewBracket_Click);
            // 
            // btnEditMatch
            // 
            this.btnEditMatch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnEditMatch.Location = new System.Drawing.Point(560, 520);
            this.btnEditMatch.Name = "btnEditMatch";
            this.btnEditMatch.Size = new System.Drawing.Size(150, 40);
            this.btnEditMatch.TabIndex = 5;
            this.btnEditMatch.Text = "Edit Match";
            this.btnEditMatch.UseVisualStyleBackColor = true;
            this.btnEditMatch.Click += new System.EventHandler(this.btnEditMatch_Click);
            // 
            // btnManageTeams  🔥 NEW BUTTON
            // 
            this.btnManageTeams.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnManageTeams.Location = new System.Drawing.Point(720, 520);
            this.btnManageTeams.Name = "btnManageTeams";
            this.btnManageTeams.Size = new System.Drawing.Size(170, 40);
            this.btnManageTeams.TabIndex = 7;
            this.btnManageTeams.Text = "Manage Teams";
            this.btnManageTeams.UseVisualStyleBackColor = true;
            this.btnManageTeams.Click += new System.EventHandler(this.btnManageTeams_Click);

            // 
            // TournamentDetails
            // 
            this.ClientSize = new System.Drawing.Size(1100, 600);
            this.Controls.Add(this.btnManageTeams);
            this.Controls.Add(this.btnViewBracket);
            this.Controls.Add(this.btnEditMatch);
            this.Controls.Add(this.btnGenerateBracket);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblTournamentType);
            this.Controls.Add(this.lblTournamentStatus);
            this.Controls.Add(this.lblTournamentName);
            this.Name = "TournamentDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tournament Details";

            this.tabControl.ResumeLayout(false);
            this.tabPlayersTeams.ResumeLayout(false);
            this.tabPlayersTeams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListOfPlayers)).EndInit();
            this.tabMatches.ResumeLayout(false);
            this.tabMatches.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeamMatches)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListOfMatches)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTournamentName;
        private System.Windows.Forms.Label lblTournamentStatus;
        private System.Windows.Forms.Label lblTournamentType;

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPlayersTeams;
        private System.Windows.Forms.TabPage tabMatches;

        private System.Windows.Forms.DataGridView dgvTeams;
        private System.Windows.Forms.DataGridView dgvListOfPlayers;
        private System.Windows.Forms.RadioButton rdZA;
        private System.Windows.Forms.RadioButton rdAZ;
        private System.Windows.Forms.Label lblFilterPlayers;

        private System.Windows.Forms.DataGridView dgvTeamMatches;
        private System.Windows.Forms.DataGridView dgvListOfMatches;
        private System.Windows.Forms.ComboBox cbMatchStatus;
        private System.Windows.Forms.Label lblFilterMatches;

        private System.Windows.Forms.Button btnGenerateBracket;
        private System.Windows.Forms.Button btnEditMatch;
        private System.Windows.Forms.Button btnViewBracket;

        // 🔥 NEW BUTTON FIELD
        private System.Windows.Forms.Button btnManageTeams;
    }
}
