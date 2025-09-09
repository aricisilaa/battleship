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
    public partial class Sonuclar : Form
    {
        private List<Ship> player1Ships;
        private List<Ship> player2Ships;
        private List<Point> player1Attacks;
        private List<Point> player2Attacks;
        private string player1Name;
        private string player2Name;
        public Sonuclar(string p1, string p2, List<Ship> p1Ships, List<Ship> p2Ships, List<Point> p1Attacks, List<Point> p2Attacks)
        {
            InitializeComponent();
            player1Name = p1;
            player2Name = p2;
            player1Ships = p1Ships;
            player2Ships = p2Ships;
            player1Attacks = p1Attacks;
            player2Attacks = p2Attacks;
        }

        private void Sonuclar_Load(object sender, EventArgs e)
        {
            this.Text = "Oyun Sonucu";
            this.Width = 1300;
            this.Height = 700;

            // Başlıklar
            Label lbl1 = new Label { Text = $"{player1Name} - Gemi Yerleşimi", Left = 100, Top = 20, Width = 200 };
            Label lbl2 = new Label { Text = $"{player1Name} - Saldırılar", Left = 450, Top = 20, Width = 200 };
            Label lbl3 = new Label { Text = $"{player2Name} - Gemi Yerleşimi", Left = 800, Top = 20, Width = 200 };
            Label lbl4 = new Label { Text = $"{player2Name} - Saldırılar", Left = 1150, Top = 20, Width = 200 };

            this.Controls.AddRange(new Control[] { lbl1, lbl2, lbl3, lbl4 });

            DrawGrid(player1Ships, null, 100, 50);               // P1 yerleşimi
            DrawGrid(null, player1Attacks, 450, 50);             // P1 saldırı
            DrawGrid(player2Ships, null, 800, 50);               // P2 yerleşimi
            DrawGrid(null, player2Attacks, 1150, 50);            // P2 saldırı
        }
        private void DrawGrid(List<Ship> ships, List<Point> attacks, int startX, int startY)
        {
            int size = 20;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Button btn = new Button();
                    btn.Width = btn.Height = size;
                    btn.Left = startX + j * size;
                    btn.Top = startY + i * size;
                    btn.Enabled = false;
                    btn.BackColor = Color.White;

                    Point pos = new Point(i, j);

                    if (ships != null && ships.Any(s => s.Positions.Contains(pos)))
                        btn.BackColor = Color.LightBlue;

                    if (attacks != null)
                        btn.BackColor = attacks.Contains(pos) ?
                            (ships != null && ships.Any(s => s.Positions.Contains(pos)) ? Color.Red : Color.LightGray)
                            : btn.BackColor;

                    this.Controls.Add(btn);
                }
            }
        }
    
}
}
