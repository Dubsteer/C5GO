using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using DesktopApp.UserControls;
using System.Diagnostics;
using System.Xml;
using System.Diagnostics.Eventing.Reader;

namespace DesktopApp
{
    public partial class ListOfMatches : Form
    {
        private MatchManager MatchManager;
        private TournamentManager TournamentManager;
        private Tournament CurrentTournament;

        public ListOfMatches(MatchManager matchManager, TournamentManager tournamentManager, Tournament currentTournament)
        {
            InitializeComponent();

            MatchManager = matchManager;
            TournamentManager = tournamentManager;
            CurrentTournament = currentTournament;

            RefreshDGV();
        }

        public void RefreshDGV()
        {
            dgvListOfMatches.DataSource = TournamentManager.GetAllMatchesInTournament(CurrentTournament);
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            var allMatches = TournamentManager.GetAllMatchesInTournament(CurrentTournament);

            foreach (DataGridViewRow row in dgvListOfMatches.SelectedRows)
            {
                var selectedMatchId = row.Cells[0].Value.ToString();
                Match selectedMatch = allMatches.FirstOrDefault(match => match.Id.ToString() == selectedMatchId);

                if (selectedMatch != null)
                {
                    if (selectedMatch.Status == Status.Closed)
                    {
                        MessageBox.Show("Match is closed. Score cannot be changed.");
                        return; 
                    }

                    selectedMatch.Player1Score = (int)nudPlayer1.Value;
                    selectedMatch.Player2Score = (int)nudPlayer2.Value;

                    bool updateScores = true; 

                    if (selectedMatch.Player1Score >= 16 && selectedMatch.Player2Score >= 16)
                    {
                        
                        MessageBox.Show("Invalid score. Only one player can have a score of 16.");

                        
                        updateScores = false; 
                        if (selectedMatch.Player1Score > selectedMatch.Player2Score)
                        {
                            nudPlayer1.Value = 16;
                            nudPlayer2.Value = 15;
                        }
                        else
                        {
                            nudPlayer1.Value = 15;
                            nudPlayer2.Value = 16;
                        }
                    }
                    else if (selectedMatch.Player1Score >= 16)
                    {
                        selectedMatch.Player1Score = 16;
                        selectedMatch.Player2Score = Math.Min(selectedMatch.Player2Score, 15);
                    }
                    else if (selectedMatch.Player2Score >= 16)
                    {
                        selectedMatch.Player1Score = Math.Min(selectedMatch.Player1Score, 15);
                        selectedMatch.Player2Score = 16;
                    }

                    if (updateScores)
                    {
                        if (selectedMatch.Player1Score >= 16 || selectedMatch.Player2Score >= 16)
                        {
                            selectedMatch.Status = Status.Closed;
                        }
                        else
                        {
                            selectedMatch.Status = Status.InProgress;
                        }

                        MatchManager.UpdateMatch(selectedMatch);
                        MessageBox.Show("Update successful");
                    }
                }
            }

            RefreshDGV(); 
        }
    }
}
