using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Enums;
using System;
using System.Windows.Forms;

namespace DesktopApp
{
    public partial class TeamMatchEditor : Form
    {
        private TeamMatch match;
        private TeamMatchManager manager;

        public TeamMatchEditor(TeamMatch match, TeamMatchManager manager)
        {
            InitializeComponent();

            this.match = match;
            this.manager = manager;

            LoadMatch();
        }

        private void LoadMatch()
        {
            lblTitle.Text = $"{match.Team1.Name} vs {match.Team2.Name}";

            lblTeamA.Text = match.Team1.Name + ":";
            lblTeamB.Text = match.Team2.Name + ":";

            nudScoreA.Value = match.Team1Score;
            nudScoreB.Value = match.Team2Score;

            cbStatus.SelectedItem = match.Status.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int scoreA = (int)nudScoreA.Value;
                int scoreB = (int)nudScoreB.Value;

                string selectedStatus = cbStatus.SelectedItem.ToString();

                // Auto-status logic
                Status newStatus;

                if (scoreA == 0 && scoreB == 0 && selectedStatus == "Closed")
                {
                    MessageBox.Show("A match cannot be 'Closed' with score 0-0.");
                    return;
                }

                // Status mapping
                newStatus = selectedStatus switch
                {
                    "Open" => Status.Open,
                    "InProgress" => Status.InProgress,
                    "Closed" => Status.Closed,
                    _ => Status.Open
                };

                // Update match model
                match.Team1Score = scoreA;
                match.Team2Score = scoreB;
                match.Status = newStatus;

                // UPDATE IN DATABASE
                manager.UpdateTeamMatch(match);

                MessageBox.Show("Match updated successfully!");

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
