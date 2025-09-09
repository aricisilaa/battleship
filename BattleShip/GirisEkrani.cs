using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShip
{
    public partial class GirisEkrani : Form
    {
        public GirisEkrani()
        {
            InitializeComponent();
        }

        private void GirisEkrani_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string oyuncu_1 = textBox1.Text.Trim();
            string oyuncu_2 = textBox2.Text.Trim();//isimleri yazar ve sonra trim boşluklara göre ayırır.

            if (string.IsNullOrEmpty(oyuncu_1) || string.IsNullOrEmpty(oyuncu_2)) 
            {
                MessageBox.Show("Lütfen iki oyuncunun da ismini giriniz.");
                return;
            }
            GemileriDuzenle GemiDüzenle= new GemileriDuzenle(oyuncu_1,oyuncu_2);
            GemiDüzenle.Show();
            this.Hide();


        }
    }
}
