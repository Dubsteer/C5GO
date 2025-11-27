namespace DesktopApp.UserControls
{
    partial class AddTournament
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.lblName = new Label();
            this.tbName = new TextBox();
            this.lblDescription = new Label();
            this.tbDescription = new TextBox();
            this.lblMode = new Label();
            this.cbMode = new ComboBox();
            this.lblTeamSize = new Label();
            this.nudTeamSize = new NumericUpDown();
            this.btnCreate = new Button();
            this.btnBack = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudTeamSize)).BeginInit();
            this.SuspendLayout();

            // LABEL: Tournament Name
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(40, 30);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(136, 20);
            this.lblName.Text = "Tournament Name:";

            // TEXTBOX: Name
            this.tbName.Location = new System.Drawing.Point(40, 55);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(300, 27);

            // LABEL: Description
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(40, 105);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(85, 20);
            this.lblDescription.Text = "Description:";

            // TEXTBOX: Description
            this.tbDescription.Location = new System.Drawing.Point(40, 130);
            this.tbDescription.Multiline = true;
            this.tbDescription.Size = new System.Drawing.Size(300, 90);
            this.tbDescription.Name = "tbDescription";

            // LABEL: Type (Solo / Team)
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(40, 245);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(42, 20);
            this.lblMode.Text = "Type:";

            // COMBOBOX: Type
            this.cbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbMode.Location = new System.Drawing.Point(40, 270);
            this.cbMode.Size = new System.Drawing.Size(150, 28);
            this.cbMode.Name = "cbMode";

            // LABEL: Team Size
            this.lblTeamSize.AutoSize = true;
            this.lblTeamSize.Location = new System.Drawing.Point(40, 320);
            this.lblTeamSize.Name = "lblTeamSize";
            this.lblTeamSize.Size = new System.Drawing.Size(150, 20);
            this.lblTeamSize.Text = "Team size (5v5 only):";

            // NUMERIC: Team Size
            this.nudTeamSize.Location = new System.Drawing.Point(40, 345);
            this.nudTeamSize.Minimum = 5;
            this.nudTeamSize.Maximum = 5;
            this.nudTeamSize.Value = 5;
            this.nudTeamSize.Name = "nudTeamSize";
            this.nudTeamSize.Size = new System.Drawing.Size(80, 27);

            // BUTTON: Create
            this.btnCreate.Location = new System.Drawing.Point(40, 400);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(200, 40);
            this.btnCreate.Text = "Create Tournament";

            // BUTTON: Back
            this.btnBack.Location = new System.Drawing.Point(260, 400);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(80, 40);
            this.btnBack.Text = "Back";

            // CONTROL ROOT
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.cbMode);
            this.Controls.Add(this.lblTeamSize);
            this.Controls.Add(this.nudTeamSize);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnBack);

            this.Size = new System.Drawing.Size(400, 500);

            ((System.ComponentModel.ISupportInitialize)(this.nudTeamSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblName;
        private TextBox tbName;
        private Label lblDescription;
        private TextBox tbDescription;
        private Label lblMode;
        private ComboBox cbMode;
        private Label lblTeamSize;
        private NumericUpDown nudTeamSize;
        private Button btnCreate;
        private Button btnBack;
    }
}
