using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Duszaverseny_2025
{
    public partial class Form1 : Form
    {
        Dictionary<int, (string, int, int, string)> kartyak = new Dictionary<int, (string, int, int, string)>();
        Dictionary<int, (string, int, int, string)> playercards = new Dictionary<int, (string, int, int, string)>(); //properies of the player's cards
        List<string> Pakli = new List<string>();

        public Form1()
        {
            string[] args = Environment.GetCommandLineArgs();
            StreamReader sr = new StreamReader(args[1]);
            InitializeComponent();
            beolvasás(sr);
            PopulateWorld();
            PlayerCardsba("ObiWan");
            PlayerCardsba("Tul'Arak");
            Pakliba("Tul'Arak");
        }

        //in.txt beolvasás
        private void beolvasás(StreamReader sr)
        {
            string sor = sr.ReadLine();
            string[] sorreszek = sor.Split(';');
            int n = 0;
            if (sorreszek[0] == "uj kartya") //kartyak dictioanryba helyezese
            {
                kartyak[n] = (sorreszek[1], Convert.ToInt32(sorreszek[2]), Convert.ToInt32(sorreszek[3]), sorreszek[4]);
                n++;
            }
            else if (sorreszek[1] == "uj vezer") //vezer berakasa kartyak koze dictionarybe
            {
                if (sorreszek[3] == "sebzes")
                {
                    string vezer = sorreszek[2];
                    bool megvan = false;
                    int m = 0;
                    while (megvan = false)
                    {
                        if (kartyak[m].Item1 == vezer)
                        {
                            megvan = true;
                            kartyak[n] = (sorreszek[1], kartyak[m].Item2 * 2, kartyak[m].Item3, kartyak[m].Item4);
                            n++;
                        }
                        m++;
                    }
                }
                else
                {
                    string vezer = sorreszek[2];
                    bool megvan = false;
                    int m = 0;
                    while (megvan = false)
                    {
                        if (kartyak[m].Item1 == vezer)
                        {
                            megvan = true;
                            kartyak[n] = (sorreszek[1], kartyak[m].Item2, kartyak[m].Item3 * 2, kartyak[m].Item4);
                            n++;
                        }
                        m++;
                    }
                }
            }
        }

        private void PopulateWorld()
        {
            kartyak[1] = ("ObiWan", 2, 2, "fold");
            kartyak[2] = ("Tul'Arak", 2,4,"föld");
        }

        private void PlayerCardsba(string c)
        {
            int cycle = 1;
            for (int i = 1; i<=kartyak.Count; i++)
            {
                if (kartyak[i].Item1 == c)
                {
                    playercards[cycle] = kartyak[i];
                    cycle++;
                }
            }
        }

        private void Pakliba(string c)
        {
            int index = 0;
            for (int i = 1; i <= playercards.Count; i++)
            {
                if (playercards[i].Item1 == c)
                {
                    Pakli[index] = kartyak[i].Item1;
                    index++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            foreach(string c in Pakli)
            {
                string text = "";
                for (int i = 1; i <= playercards.Count; i++)
                {
                    if (playercards[i].Item1 == c)
                    {
                        text = (playercards[i].Item1.ToString() +"\n"+ playercards[i].Item2.ToString() +"/"+ playercards[i].Item3.ToString() + "\n"+ playercards[i].Item4.ToString());
                    }
                }
                
            }

        }
    }
}
