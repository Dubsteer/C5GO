namespace DesktopApp
{
    partial class TeamTournamentEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvTeamsInTournament = new System.Windows.Forms.DataGridView();
            this.dgvTeamsAvailable = new System.Windows.Forms.DataGridView();
            this.btnAddTeam = new System.Windows.Forms.Button();
            this.btnRemoveTeam = new System.Windows.Forms.Button();
            this.btnAutoFill = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // dgvTeamsInTournament
            this.dgvTeamsInTournament.Location = new System.Drawing.Point(20, 20);
            this.dgvTeamsInTournament.Size = new System.Drawing.Size(350, 400);
            this.dgvTeamsInTournament.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTeamsInTournament.ReadOnly = true;

            // dgvTeamsAvailable
            this.dgvTeamsAvailable.Location = new System.Drawing.Point(430, 20);
            this.dgvTeamsAvailable.Size = new System.Drawing.Size(350, 400);
            this.dgvTeamsAvailable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTeamsAvailable.ReadOnly = true;

            // btnAddTeam
            this.btnAddTeam.Text = "Add →";
            this.btnAddTeam.Location = new System.Drawing.Point(380, 120);
            this.btnAddTeam.Size = new System.Drawing.Size(40, 40);
            this.btnAddTeam.Click += new System.EventHandler(this.btnAddTeam_Click);

            // btnRemoveTeam
            this.btnRemoveTeam.Text = "← Remove";
            this.btnRemoveTeam.Location = new System.Drawing.Point(380, 180);
            this.btnRemoveTeam.Size = new System.Drawing.Size(40, 40);
            this.btnRemoveTeam.Click += new System.EventHandler(this.btnRemoveTeam_Click);

            // btnAutoFill
            this.btnAutoFill.Text = "Auto Fill Teams";
            this.btnAutoFill.Location = new System.Drawing.Point(20, 430);
            this.btnAutoFill.Size = new System.Drawing.Size(150, 40);
            this.btnAutoFill.Click += new System.EventHandler(this.btnAutoFill_Click);

            // btnClose
            this.btnClose.Text = "Close";
            this.btnClose.Location = new System.Drawing.Point(630, 430);
            this.btnClose.Size = new System.Drawing.Size(150, 40);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // FORM
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.dgvTeamsInTournament);
            this.Controls.Add(this.dgvTeamsAvailable);
            this.Controls.Add(this.btnAddTeam);
            this.Controls.Add(this.btnRemoveTeam);
            this.Controls.Add(this.btnAutoFill);
            this.Controls.Add(this.btnClose);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Tournament Teams";

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvTeamsInTournament;
        private System.Windows.Forms.DataGridView dgvTeamsAvailable;
        private System.Windows.Forms.Button btnAddTeam;
        private System.Windows.Forms.Button btnRemoveTeam;
        private System.Windows.Forms.Button btnAutoFill;
        private System.Windows.Forms.Button btnClose;
    }
}
