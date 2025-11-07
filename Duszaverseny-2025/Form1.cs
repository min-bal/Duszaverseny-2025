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
        string[] nevek;
        string[] sebzes;
        string[] elet;
        string[] tipus;

        var kartyak = new Dictionary<string, int, int, string>
        {

        }
        public Form1()
        {
            string[] args = Environment.GetCommandLineArgs();
            StreamReader sr = new StreamReader(args[1]);
            InitializeComponent();
        }

        //in.txt beolvasás
        private void beolvasás(StreamReader sr)
        {
            int n = 0;
            string sor = sr.ReadLine();
            string[] sorreszek = sor.Split(';');
            if (sorreszek[0] == "uj kartya")
            {
                nevek[n] = sorreszek[1];
                sebzes[n] = sorreszek[2];
                elet[n] = sorreszek[3];
                tipus[n] = sorreszek[4];
                n++;
            }
            else if (sorreszek[1] == "uj vezer")
            {
                nevek[n] = sorreszek[1];
                if (sorreszek[3] == "sebzes")
                {
                    string vezer = sorreszek[2];
                    bool megvan = false;
                    int m = 0;
                    while (megvan = false)
                    {
                        if (nevek[m])
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
