using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BattleShip
{
    public partial class GemileriDuzenle : Form
    {
        private List<Button> player1Buttons = new List<Button>();
        private List<Button> player2Buttons = new List<Button>();
        private string oyuncu1, oyuncu2;
        private int currentPlayer = 1;

        private GameData gameData = new GameData();
        private List<Point> selectedPoints = new List<Point>();
        private Dictionary<int, Ship> currentShips = new Dictionary<int, Ship>();
        private List<Point> yerlestirilmisHucreler = new List<Point>();

        private Dictionary<int, Color> gemiRenkleri = new Dictionary<int, Color>()
        {
            {2, Color.Blue },
            {3, Color.Green },
            {4, Color.Orange },
            {5, Color.Purple }
        };

        private ComboBox cmbGemiTur = new ComboBox();

        public GemileriDuzenle(string o1, string o2)
        {
            InitializeComponent();
            oyuncu1 = o1;
            oyuncu2 = o2;
        }

        public class Ship
        {
            public string Name { get; set; }
            public List<Point> Positions { get; set; }
            public int Size => Positions?.Count ?? 0;
        }

        public class GameData
        {
            public string Player1 { get; set; }
            public string Player2 { get; set; }
            public List<Ship> Player1Ship { get; set; } = new List<Ship>();
            public List<Ship> Player2Ship { get; set; } = new List<Ship>();
        }

        public void SaveGameData(GameData data)
        {
            string path = Path.Combine(Application.StartupPath, "gameData.json");
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private void GemileriDuzenle_Load(object sender, EventArgs e)
        {
            CreateGrid(player1Buttons);
            CreateGrid(player2Buttons);
            ShowGridForPlayer(1);

            gameData.Player1 = oyuncu1;
            gameData.Player2 = oyuncu2;

            cmbGemiTur.Left = 320;
            cmbGemiTur.Top = 20;
            cmbGemiTur.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGemiTur.Items.AddRange(new string[] { "2 blok", "3 blok", "4 blok", "5 blok" });
            cmbGemiTur.SelectedIndex = 0;
            this.Controls.Add(cmbGemiTur);

            Label lbl = new Label();
            lbl.Text = "Gemi Türü Seç:";
            lbl.Left = 220;
            lbl.Top = 23;
            lbl.AutoSize = true;
            this.Controls.Add(lbl);
        }

        private void CreateGrid(List<Button> buttonList)
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
                    btn.BackColor = Color.LightGray;
                    btn.Click += GridButton_Click;
                    buttonList.Add(btn);
                }
            }
        }

        private void ShowGridForPlayer(int player)
        {
            foreach (var btn in this.Controls.OfType<Button>().Where(b => b.Tag is Point).ToList())
                this.Controls.Remove(btn);

            var aktifButtons = player == 1 ? player1Buttons : player2Buttons;
            foreach (var btn in aktifButtons)
                this.Controls.Add(btn);
        }

        private int GetSelectedShipSize()
        {
            switch (cmbGemiTur.SelectedIndex)
            {
                case 0:
                    return 2;
                case 1:
                    return 3;
                case 2:
                    return 4;
                case 3:
                    return 5;
                default:
                    return 2; // Varsayılan değer
            }
        }

        private bool IsValidPlacement(List<Point> points)
        {
            if (points.Count < 2) return true;
            bool sameRow = points.All(p => p.X == points[0].X);
            bool sameCol = points.All(p => p.Y == points[0].Y);

            if (!sameRow && !sameCol) return false;

            var ordered = sameRow ? points.OrderBy(p => p.Y).ToList() : points.OrderBy(p => p.X).ToList();

            for (int i = 1; i < ordered.Count; i++)
            {
                if (sameRow && ordered[i].Y != ordered[i - 1].Y + 1) return false;
                if (sameCol && ordered[i].X != ordered[i - 1].X + 1) return false;
            }
            return true;
        }

        private void RemoveExistingShip(int size)
        {
            if (currentShips.ContainsKey(size))
            {
                var oldShip = currentShips[size];
                foreach (var point in oldShip.Positions)
                {
                    var targetBtn = this.Controls.OfType<Button>().FirstOrDefault(b => (Point)b.Tag == point);
                    if (targetBtn != null) targetBtn.BackColor = Color.LightGray;
                    yerlestirilmisHucreler.Remove(point);
                }

                if (currentPlayer == 1)
                    gameData.Player1Ship.RemoveAll(s => s.Size == size);
                else
                    gameData.Player2Ship.RemoveAll(s => s.Size == size);

                currentShips.Remove(size);
            }
        }

        private void GridButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Point pos = (Point)btn.Tag;
            int size = GetSelectedShipSize();

            if (yerlestirilmisHucreler.Contains(pos) && !selectedPoints.Contains(pos))
            {
                MessageBox.Show("Bu hücreye daha önce gemi yerleştirildi!");
                return;
            }

            if (selectedPoints.Contains(pos))
            {
                selectedPoints.Remove(pos);
                btn.BackColor = Color.LightGray;
                return;
            }

            selectedPoints.Add(pos);
            btn.BackColor = gemiRenkleri[size];

            if (selectedPoints.Count == size)
            {
                if (!IsValidPlacement(selectedPoints))
                {
                    MessageBox.Show("Gemiler sadece düz bir çizgide ve bitişik olmalıdır.", "Geçersiz Yerleştirme", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    foreach (var p in selectedPoints)
                    {
                        var b = this.Controls.OfType<Button>().FirstOrDefault(bt => (Point)bt.Tag == p);
                        if (b != null) b.BackColor = Color.LightGray;
                    }
                    selectedPoints.Clear();
                    return;
                }

                RemoveExistingShip(size);

                Ship ship = new Ship
                {
                    Name = $"Gemi_{size}B_{DateTime.Now.Ticks}",
                    Positions = new List<Point>(selectedPoints)
                };

                if (currentPlayer == 1)
                    gameData.Player1Ship.Add(ship);
                else
                    gameData.Player2Ship.Add(ship);

                currentShips[size] = ship;
                yerlestirilmisHucreler.AddRange(ship.Positions);
                selectedPoints.Clear();

                MessageBox.Show($"{size} blokluk gemi başarıyla yerleştirildi.");
            }
        }

        //private void btnTamam_Click(object sender, EventArgs e)
        //{
        //    if (currentShips.Count < 4)
        //    {
        //        MessageBox.Show("Her boyuttan yalnızca bir gemi yerleştirmelisiniz (2,3,4,5 blokluk).");
        //        return;
        //    }

        //    if (currentPlayer == 1)
        //    {
        //        currentPlayer = 2;
        //        MessageBox.Show("Sıra Oyuncu 2'de.");
        //        ClearGrid();
        //        ShowGridForPlayer(2);
        //        cmbGemiTur.SelectedIndex = 0;
        //        currentShips.Clear();
        //    }
        //    else
        //    {
        //        SaveGameData(gameData);
        //        MessageBox.Show("Tüm gemiler kaydedildi. Oyun başlatılıyor.");
        //        this.Hide();
        //        new OyunEkrani().Show();
        //    }
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentShips.Count < 4)
            {
                MessageBox.Show("Her boyuttan yalnızca bir gemi yerleştirmelisiniz (2,3,4,5 blokluk).");
                return;
            }

            if (currentPlayer == 1)
            {
                currentPlayer = 2;
                MessageBox.Show("Sıra Oyuncu 2'de.");
                ClearGrid();
                ShowGridForPlayer(2);
                cmbGemiTur.SelectedIndex = 0;
                currentShips.Clear();
            }
            else
            {
                SaveGameData(gameData);
                MessageBox.Show("Tüm gemiler kaydedildi. Oyun başlatılıyor.");
                this.Hide();
                new OyunEkrani().Show();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ClearGrid()
        {
            selectedPoints.Clear();
            yerlestirilmisHucreler.Clear();
            foreach (var btn in this.Controls.OfType<Button>().Where(b => b.Tag is Point))
                btn.BackColor = Color.LightGray;
        }
    }
}
