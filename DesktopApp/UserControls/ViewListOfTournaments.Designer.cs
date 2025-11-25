namespace DesktopApp.UserControls
{
    partial class ViewListOfTournaments
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
            dgvTournaments = new DataGridView();
            btnCreateTournament = new Button();
            label1 = new Label();
            btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvTournaments).BeginInit();
            SuspendLayout();
            // 
            // dgvTournaments
            // 
            dgvTournaments.AllowUserToAddRows = false;
            dgvTournaments.AllowUserToDeleteRows = false;
            dgvTournaments.AllowUserToResizeColumns = false;
            dgvTournaments.AllowUserToResizeRows = false;
            dgvTournaments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvTournaments.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTournaments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTournaments.Location = new Point(61, 157);
            dgvTournaments.Margin = new Padding(3, 4, 3, 4);
            dgvTournaments.Name = "dgvTournaments";
            dgvTournaments.ReadOnly = true;
            dgvTournaments.RowHeadersWidth = 51;
            dgvTournaments.RowTemplate.Height = 25;
            dgvTournaments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTournaments.Size = new Size(1085, 631);
            dgvTournaments.TabIndex = 0;
            /*dgvTournaments.CellDoubleClick += dgvTournaments_CellDoubleClick;*/
            // 
            // btnCreateTournament
            // 
            btnCreateTournament.Location = new Point(1178, 308);
            btnCreateTournament.Margin = new Padding(3, 4, 3, 4);
            btnCreateTournament.Name = "btnCreateTournament";
            btnCreateTournament.Size = new Size(186, 107);
            btnCreateTournament.TabIndex = 1;
            btnCreateTournament.Text = "Create Tournament";
            btnCreateTournament.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(97, 69);
            label1.Name = "label1";
            label1.Size = new Size(221, 32);
            label1.TabIndex = 2;
            label1.Text = "List of tournaments";
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(1178, 453);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(186, 96);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete Tournament";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDeleteTournament_Click;

            // 
            // ViewListOfTournaments
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnDelete);
            Controls.Add(label1);
            Controls.Add(btnCreateTournament);
            Controls.Add(dgvTournaments);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ViewListOfTournaments";
            Size = new Size(1423, 908);
            ((System.ComponentModel.ISupportInitialize)dgvTournaments).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvTournaments;
        private Button btnCreateTournament;
        private Label label1;
        private Button btnDelete;
    }
}
