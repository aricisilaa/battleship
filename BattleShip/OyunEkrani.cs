using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BattleShip.GemileriDuzenle;

namespace BattleShip
{
    public partial class OyunEkrani : Form
    {
        private List<Button> attackButtons = new List<Button>();
        private GameData gameData;
        private int currentPlayer = 1;
        private List<Point> player1Attacks = new List<Point>();
        private List<Point> player2Attacks = new List<Point>();



        public OyunEkrani()
        {
            InitializeComponent();
        }

        private void OyunEkrani_Load(object sender, EventArgs e)
        {
            LoadGameData();
            CreateGrid();
            ShowCurrentPlayerBoard();

        }

        private void LoadGameData()
        {
            string path = Path.Combine(Application.StartupPath, "gameData.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                gameData = JsonConvert.DeserializeObject<GameData>(json);
            }
            else
            {
                MessageBox.Show("Oyun verisi bulunamadı.");
                Close();
            }
        }
        private void CreateGrid()
        {
            int size = 30;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Button btn = new Button();
                    btn.Width = btn.Height = size;
                    btn.Left = j * size;
                    btn.Top = i * size;
                    btn.Tag = new Point(i, j);
                    btn.BackColor = Color.LightBlue;
                    btn.Click += Attack_Click;
                    attackButtons.Add(btn);
                    this.Controls.Add(btn);
                }
            }
        }


        private void ShowCurrentPlayerBoard()
        {
            Text = currentPlayer == 1 ? $"{gameData.Player1}, saldırı zamanı!" : $"{gameData.Player2}, saldırı zamanı!";
            UpdateBoardView();
        }

        private void UpdateBoardView()
        {
            var attacks = currentPlayer == 1 ? player1Attacks : player2Attacks;
            var opponentShips = currentPlayer == 1 ? gameData.Player2Ship : gameData.Player1Ship;

            foreach (var btn in attackButtons)
            {
                Point pos = (Point)btn.Tag;
                if (attacks.Contains(pos))
                {
                    bool isHit = IsHit(pos, opponentShips);
                    btn.Text = "";
                    btn.BackColor = isHit ? Color.Red : Color.LightGray;
                    btn.Enabled = false;
                }
                else
                {
                    btn.Text = "";
                    btn.BackColor = Color.LightBlue;
                    btn.Enabled = true;
                }
            }
        }

        private void Attack_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Point pos = (Point)btn.Tag;

            List<Ship> opponentShips = currentPlayer == 1 ? gameData.Player2Ship : gameData.Player1Ship;
            var currentAttacks = currentPlayer == 1 ? player1Attacks : player2Attacks;

            if (currentAttacks.Contains(pos)) return;

            bool isHit = IsHit(pos, opponentShips);
            currentAttacks.Add(pos);

            btn.Text = "";
            btn.BackColor = isHit ? Color.Red : Color.LightGray;
            btn.Enabled = false;

            if (AllShipsSunk(opponentShips, currentAttacks))
            {
                string winner = currentPlayer == 1 ? gameData.Player1 : gameData.Player2;
                MessageBox.Show($"Tebrikler, {winner} kazandı!", "Oyun Bitti");

                var resultForm = new Sonuclar(
                    gameData.Player1,
                    gameData.Player2,
                    gameData.Player1Ship,
                    gameData.Player2Ship,
                    player1Attacks,
                    player2Attacks
                );

                resultForm.Show();
                this.Hide();
                return;
            }

            if (!isHit)
            {
                currentPlayer = currentPlayer == 1 ? 2 : 1;
                MessageBox.Show("Iska! Sıra rakibe geçti.");
            }
            else
            {
                MessageBox.Show("İsabet!");
            }

            ShowCurrentPlayerBoard();
        }
        private bool AllShipsSunk(List<Ship> ships, List<Point> attacks)
        {
            foreach (var ship in ships)
            {
                foreach (var pos in ship.Positions)
                {
                    if (!attacks.Contains(pos))
                        return false; // Hâlâ vurulmamış parça var
                }
            }
            return true; // Tüm parçalar vurulmuş
        }

        private bool IsHit(Point p, List<Ship> ships)
        {
            return ships.Any(ship => ship.Positions.Any(pos => pos == p));
        }
    }
}
