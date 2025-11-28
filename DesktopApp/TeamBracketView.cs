using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DesktopApp
{
    public partial class TeamBracketView : Form
    {
        private Tournament tournament;
        private TeamMatchManager teamMatchManager;
        private TeamManager teamManager;

        private List<TeamMatch> matches;

        public TeamBracketView(
            Tournament t,
            TeamMatchManager teamMatchManager,
            TeamManager teamManager)
        {
            InitializeComponent();

            this.tournament = t;
            this.teamMatchManager = teamMatchManager;
            this.teamManager = teamManager;

            matches = teamMatchManager.GetTeamMatchesByTournament(t.Id);

            Text = $"Team Bracket — {t.Name}";

            DrawBracket();
        }

        private void DrawBracket()
        {
            panelBracket.Controls.Clear();

            int roundCount = GetRoundCount();
            int panelWidth = panelBracket.Width;
            int panelHeight = panelBracket.Height;

            int roundSpacing = panelWidth / (roundCount + 1);

            // group matches by rounds
            var rounds = GetRounds(matches);

            for (int r = 0; r < rounds.Count; r++)
            {
                int x = roundSpacing * (r + 1);
                var roundMatches = rounds[r];

                int matchSpacing = panelHeight / (roundMatches.Count + 1);

                for (int i = 0; i < roundMatches.Count; i++)
                {
                    int y = matchSpacing * (i + 1);

                    var m = roundMatches[i];
                    Panel box = CreateMatchBox(m);
                    box.Location = new Point(x, y);
                    panelBracket.Controls.Add(box);
                }
            }
        }

        private Panel CreateMatchBox(TeamMatch match)
        {
            Panel p = new Panel();
            p.Size = new Size(180, 85);
            p.BorderStyle = BorderStyle.FixedSingle;
            p.BackColor = GetStatusColor(match.Status);

            p.Cursor = Cursors.Hand;
            p.Tag = match;
            p.Click += MatchBox_Click;

            Label lbl = new Label();
            lbl.AutoSize = false;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;

            var t1 = match.Team1.Name;
            var t2 = match.Team2.Name;

            lbl.Text =
                $"{t1} vs {t2}\n" +
                $"Score: {match.Team1Score} - {match.Team2Score}\n" +
                $"Status: {match.Status}";

            p.Controls.Add(lbl);

            return p;
        }

        private void MatchBox_Click(object? sender, EventArgs e)
        {
            Panel box = (Panel)sender;
            TeamMatch m = (TeamMatch)box.Tag;

            TeamMatchEditor editor = new TeamMatchEditor(m, teamMatchManager);
            editor.ShowDialog();

            // refresh after editing
            matches = teamMatchManager.GetTeamMatchesByTournament(tournament.Id);
            DrawBracket();
        }

        private List<List<TeamMatch>> GetRounds(List<TeamMatch> allMatches)
        {
            // Lottery:
            // 8 teams → 4,2,1
            // 12 teams → 4 play-in (round0), 4 quarters, 2 semis, 1 final
            // 16 teams → 8,4,2,1

            int teamCount = GetTeamCount();

            if (teamCount == 8)
                return new List<List<TeamMatch>>()
                {
                    allMatches.Take(4).ToList(),
                    allMatches.Skip(4).Take(2).ToList(),
                    allMatches.Skip(6).Take(1).ToList(),
                };

            if (teamCount == 12)
                return new List<List<TeamMatch>>()
                {
                    allMatches.Take(4).ToList(),      // play-in
                    allMatches.Skip(4).Take(4).ToList(), // quarter-finals
                    allMatches.Skip(8).Take(2).ToList(), // semi-finals
                    allMatches.Skip(10).Take(1).ToList() // final
                };

            if (teamCount == 16)
                return new List<List<TeamMatch>>()
                {
                    allMatches.Take(8).ToList(),
                    allMatches.Skip(8).Take(4).ToList(),
                    allMatches.Skip(12).Take(2).ToList(),
                    allMatches.Skip(14).Take(1).ToList(),
                };

            return new List<List<TeamMatch>>();
        }

        private int GetTeamCount()
        {
            return tournament.TeamSizeRequired * 0 == 0 // just satisfy compiler
                ? matches.Count * 2 // matches represent half of teams only in first round
                : matches.Count * 2;
        }

        private int GetRoundCount()
        {
            int c = matches.Count;

            if (c == 7) return 3;     // 8 teams
            if (c == 11) return 4;    // 12 teams
            if (c == 15) return 4;    // 16 teams

            return 3;
        }

        private Color GetStatusColor(Status s)
        {
            if (s == Status.Open) return Color.LightBlue;
            if (s == Status.InProgress) return Color.Khaki;
            return Color.LightGray;
        }
    }
}
