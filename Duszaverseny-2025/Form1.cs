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
    /*
     in.txt max. 200 sor
        formátuma mindig helyes
    sebzés/életerő: egész, 2-100/1-100
    vezéreket a sima kártyák után
    név max. 16 karakter
    kazamata neve max. 20
    gyűjteményben egy kártya csak egyszer
    uj pakli akárhányszor ismétlődhet
    kazamata elején életerő alaphelyzetbe kerül

     
     
     */

    public partial class Form1 : Form
    {
        Dictionary<string, string> tipusok = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "tuz", "Tűz" },
            { "viz", "Víz" },
            { "fold", "Föld" },
            { "levego", "Levegő" }
        };
        Dictionary<int, (string, int, int, string)> kartyak = new Dictionary<int, (string, int, int, string)>(); //név, sebzés, életerő, típus
        Dictionary<int, (string, int, int, string)> vezerkartyak = new Dictionary<int, (string, int, int, string)>(); 
        Dictionary<int, (string, int, int, string)> playercards = new Dictionary<int, (string, int, int, string)>(); //properies of the player's cards
        List<string> Pakli = new List<string>();

        public Form1()
        {
            string[] args = Environment.GetCommandLineArgs();
            StreamReader sr = new StreamReader(args[1]);
            InitializeComponent();
            VilágBeolvasás(sr);
            Gyujteménykészítés(sr);

            ReadNextLine(sr);



            Pakliba("Sadan");
            Pakliba("Aragorn");


            foreach (var kartya in kartyak)
            {
                Console.WriteLine(kartya.Value.Item1);
            }
            foreach (var vezerkartya in vezerkartyak)
            {
                Console.WriteLine(vezerkartya.Value.Item1);
            }

        }

        //in.txt beolvasás
        private void VilágBeolvasás(StreamReader sr)
        {
            int n = 0;
            bool vanvezer = false;
            while (true)
            {
                string sor = sr.ReadLine();
                if (sor == "uj jatekos") break;
                if (sor != "" && !vanvezer)
                {
                    string[] sorreszek = sor.Split(';');
                    if (sorreszek[0] == "uj kartya")
                    {
                        kartyak[n] = (sorreszek[1], Convert.ToInt32(sorreszek[2]), Convert.ToInt32(sorreszek[3]), sorreszek[4]);
                        n++;
                    }

                    else if (sorreszek[0] == "uj vezer") //vezer berakasa kartyak koze dictionarybe
                    {
                        vanvezer = true;

                        string alapnev = sorreszek[2];
                        int m = 0;
                        while (true)
                        {
                            if (kartyak[m].Item1 == alapnev)
                            {
                                if (sorreszek[3] == "sebzes")
                                {
                                    vezerkartyak[0] = (sorreszek[1], kartyak[m].Item2 * 2, kartyak[m].Item3, kartyak[m].Item4);
                                }
                                else
                                {
                                    vezerkartyak[0] = (sorreszek[1], kartyak[m].Item2, kartyak[m].Item3 * 2, kartyak[m].Item4);
                                }
                                n++;
                                break;
                            }
                            m++;
                        }
                    }
                }
            }
        }

        private void Gyujteménykészítés(StreamReader sr)
        {
            int n = 0;
            while (true)
            {
                string sor = sr.ReadLine();
                if (sor != "") 
                { 
                    string[] sorreszek = sor.Split(';');
                    if (sorreszek[0] != "felvetel gyujtemenybe") break;
                    else
                    {
                        foreach (int i in kartyak.Keys)
                        {
                            if (kartyak[i].Item1 == sorreszek[1])
                            {
                                playercards[n] = kartyak[i];
                            }
                        }
                        
                    }
                }
            }
        }

        private void Pakliba(string c)
        {
            for (int i = 0; i < playercards.Count; i++)
            {
                if (playercards[i].Item1 == c)
                {
                    Pakli.Add(playercards[i].Item1);
                }
            }
        }

        private void ReadNextLine(StreamReader sr)
        {
            while (true)
            {
                string sor = sr.ReadLine();
                if (string.IsNullOrEmpty(sor) && sor != "")
                {
                    sr.Close();
                    Console.WriteLine("Vége a bemenetelnek");
                    Application.Exit();
                }
                else if (sor != "")
                {
                    string[] sorreszek = sor.Split(';');
                    if (sorreszek[0] == "uj pakli")
                    {
                        Pakliba(sorreszek[1]);
                    }
                    else if (sorreszek[0] == "harc")
                    {
                        Harc(sorreszek[1], sorreszek[2]);
                    }
                    else if (sorreszek[0] == "export vilag") {
                        ExportState("vilag", sorreszek[1]);
                    }
                    else if (sorreszek[0] == "export jatekos")
                    {
                        ExportState("jatekos", sorreszek[1]);
                    }
                }
            }
        }

        private void Harc(string nev, string output)
        {

        }

        private void ExportState(string típus, string output)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Control ctrl = new Control();
            
            Console.WriteLine(Pakli.Count);
            for (int i = 0; i < Pakli.Count; i++) { 
                string text = "";
                for (int j = 0; j < playercards.Count; j++)
                {
                    if (playercards[j].Item1 == Pakli[i])
                    {
                        tipusok.TryGetValue(playercards[j].Item4, out string olvashatotipus);
                        text = (playercards[j].Item1.ToString() +"\n"+ playercards[j].Item2.ToString() +"/"+ playercards[j].Item3.ToString() + "\n"+ olvashatotipus);
                        
                    }
                }
                Console.WriteLine(text);
            }
            Console.WriteLine("end");
        }
    }
}
