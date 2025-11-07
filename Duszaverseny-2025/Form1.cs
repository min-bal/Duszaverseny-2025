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
        Dictionary<int, (string, int, int, string)> vezerkartyak = new Dictionary<int, (string, int, int, string)>();
        Dictionary<int, (string, int, int, string)> playercards = new Dictionary<int, (string, int, int, string)>(); //properies of the player's cards
        List<string> Pakli = new List<string>();

        public Form1()
        {
            string[] args = Environment.GetCommandLineArgs();
            StreamReader sr = new StreamReader(args[1]);
            InitializeComponent();
            beolvasás(sr);

            foreach (var kartya in kartyak)
            {
                Console.WriteLine(kartya.Value.Item1);
            }
            Console.WriteLine();
            foreach(var vezerkartya in vezerkartyak)
            {
                Console.WriteLine(vezerkartya.Value.Item1);
            }
            /*
            PlayerCardsba("ObiWan");
            PlayerCardsba("Tul'Arak");
            Pakliba("Tul'Arak");*/
        }

        //in.txt beolvasás
        private void beolvasás(StreamReader sr)
        {
            int n = 0;
            while (true)
            {
                string sor = sr.ReadLine();
                if (sor == "uj jatekos") break;
                if (sor != "")
                {
                    string[] sorreszek = sor.Split(';');
                    if (sorreszek[0] == "uj kartya") //kartyak dictioanryba helyezese
                    {
                        kartyak[n] = (sorreszek[1], Convert.ToInt32(sorreszek[2]), Convert.ToInt32(sorreszek[3]), sorreszek[4]);
                        n++;
                    }

                    else if (sorreszek[0] == "uj vezer") //vezer berakasa kartyak koze dictionarybe
                    {
                        if (sorreszek[3] == "sebzes")
                        {
                            string vezer = sorreszek[2];
                            bool megvan = false;
                            int m = 0;
                            while (!megvan)
                            {
                                if (kartyak[m].Item1 == vezer)
                                {
                                    megvan = true;
                                    vezerkartyak[0] = (sorreszek[1], kartyak[m].Item2 * 2, kartyak[m].Item3, kartyak[m].Item4);
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
                            while (!megvan)
                            {
                                if (kartyak[m].Item1 == vezer)
                                {
                                    megvan = true;
                                    vezerkartyak[0] = (sorreszek[1], kartyak[m].Item2, kartyak[m].Item3 * 2, kartyak[m].Item4);
                                    n++;
                                }
                                m++;
                            }
                        }
                    }
                }
            }
            sr.Close();
        }

        private void PlayerCardsba(string c)
        {
            int index = 0;
            for (int i = 0; i< kartyak.Count; i++)
            {
                if (kartyak[i].Item1 == c)
                {
                    playercards[index] = kartyak[i];
                    index++;
                }
            }
        }

        private void Pakliba(string c)
        {
            int index = 0;
            for (int i = 0; i < playercards.Count; i++)
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
            Console.WriteLine("asd");
            for (int i = 0; i < Pakli.Count; i++) { 
                string text = "asd";
                for (int j = 1; j <= playercards.Count; j++)
                {
                    if (playercards[j].Item1 == Pakli[i])
                    {
                        text = (playercards[j].Item1.ToString() +"\n"+ playercards[j].Item2.ToString() +"/"+ playercards[j].Item3.ToString() + "\n"+ playercards[j].Item4.ToString());
                    }
                }
                Console.WriteLine(text);
            }
            Console.WriteLine("end");
        }
    }
}
