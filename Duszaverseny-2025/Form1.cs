using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Duszaverseny_2025
{
    public partial class Form1 : Form
    {
        Dictionary<int, (string, int, int, string)> data = new Dictionary<int, (string, int, int, string)>(); //base properties of each normal card
        Dictionary<int, (string, int, int, string)> playercards = new Dictionary<int, (string, int, int, string)>(); //properies of the player's cards
        List<string> Pakli = new List<string>();

        public Form1()
        {
            InitializeComponent();
            label1.Text = "Hello World!";
            PopulateWorld();
            PlayerCardsba("ObiWan");
            PlayerCardsba("Tul'Arak");
            Pakliba("Tul'Arak");
        }

        private void PopulateWorld()
        {
            data[1] = ("ObiWan", 2, 2, "fold");
            data[2] = ("Tul'Arak", 2,4,"föld");
        }

        private void PlayerCardsba(string c)
        {
            int cycle = 1;
            for (int i = 1; i<=data.Count; i++)
            {
                if (data[i].Item1 == c)
                {
                    playercards[cycle] = data[i];
                    cycle++;
                }
            }
        }

        private void Pakliba(string c)
        {
            int cycle = 1;
            for (int i = 1; i <= playercards.Count; i++)
            {
                if (playercards[i].Item1 == c)
                {
                    Pakli[cycle] = data[i].Item1;
                    cycle++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            foreach(string c in Pakli)
            {

            }

        }
    }
}
