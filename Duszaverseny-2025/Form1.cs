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
        public Form1()
        {
            string[] args = Environment.GetCommandLineArgs();
            StreamReader sr = new StreamReader(args[1]);
            InitializeComponent();
        }

        //in.txt beolvasás
        private void beolvasás(StreamReader sr)
        {
            string sor = sr.ReadLine();
            string[] sorreszek = sor.Split(';');
            int n = 0;
            if (sorreszek[0] == "uj kartya") //kartyak dictioanryba helyezese
            {
                kartyak[n] = (sorreszek[1], Convert.ToInt32(sorreszek[2]),Convert.ToInt32(sorreszek[3]), sorreszek[4]);
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
                            kartyak[n] = (sorreszek[1], kartyak[m].Item2*2, kartyak[m].Item3, kartyak[m].Item4);
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
                            kartyak[n] = (sorreszek[1], kartyak[m].Item2, kartyak[m].Item3*2, kartyak[m].Item4);
                            n++;
                        }
                        m++;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Button1 Clicked!";
        }
    }
}
