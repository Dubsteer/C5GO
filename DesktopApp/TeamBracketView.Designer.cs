namespace DesktopApp
{
    partial class TeamBracketView
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelBracket;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelBracket = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelBracket
            // 
            this.panelBracket.AutoScroll = true;
            this.panelBracket.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBracket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBracket.Location = new System.Drawing.Point(0, 0);
            this.panelBracket.Name = "panelBracket";
            this.panelBracket.Size = new System.Drawing.Size(1200, 700);
            this.panelBracket.TabIndex = 0;
            // 
            // TeamBracketView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.panelBracket);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TeamBracketView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Team Bracket";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
