using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duszaverseny_2025
{
    internal class Teszt
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

        Dictionary<string, (string, string)> kazamataegyszeru = new Dictionary<string, (string, string)>(); //név, ellenfél, jutalom
        Dictionary<string, (string, string, string, string, string)> kazamatakicsi = new Dictionary<string, (string, string, string, string, string)>(); //név, ellenfél*3, vezér, jutalom
        Dictionary<string, (string, string, string, string, string, string)> kazamatanagy = new Dictionary<string, (string, string, string, string, string, string)>(); //név, ellenfél*5, vezér
        List<string> Pakli = new List<string>();
        string[] args = Environment.GetCommandLineArgs();

        public void Run()
        {
            StreamReader sr = new StreamReader(args[1]);
            Világsoronként(sr);

            ReadNextLine(sr);

            foreach (var kartya in kartyak)
            {
                Console.WriteLine(kartya.Value.Item1);
            }
            foreach (var vezerkartya in vezerkartyak)
            {
                Console.WriteLine(vezerkartya.Value.Item1);
            }

        }

        private void Világsoronként(StreamReader sr)
        {//kártya, vezér, kazamata, játékos, felvétel, pakli
            int kartyan = 0;
            int vezerkartyan = 0;
            int playercardsn = 0;
            bool ujjatakos = false;
            while (true)
            {
                string sor = sr.ReadLine();
                if (sor == null || sor == "")
                {
                    continue;
                }
                string[] sorreszek = sor.Split(';');
                if (!ujjatakos)
                {
                    if (sorreszek[0] == "uj kartya")
                    {
                        kartyak[kartyan] = (sorreszek[1], Convert.ToInt32(sorreszek[2]), Convert.ToInt32(sorreszek[3]), sorreszek[4]);
                        kartyan++;
                    }
                    if (sorreszek[0] == "uj vezer")
                    {
                        string alapnev = sorreszek[2];
                        foreach (int m in kartyak.Keys)
                        {
                            if (kartyak[m].Item1 == alapnev)
                            {
                                if (sorreszek[3] == "sebzes")
                                {
                                    vezerkartyak[vezerkartyan] = (sorreszek[1], kartyak[m].Item2 * 2, kartyak[m].Item3, kartyak[m].Item4);
                                }
                                else
                                {
                                    vezerkartyak[vezerkartyan] = (sorreszek[1], kartyak[m].Item2, kartyak[m].Item3 * 2, kartyak[m].Item4);
                                }
                                vezerkartyan++;
                            }
                        }
                    }
                    if (sorreszek[0] == "uj kazamata")
                    {
                        if (sorreszek[1] == "egyszeru")
                        {
                            kazamataegyszeru[sorreszek[2]] = (sorreszek[3], sorreszek[4]);
                        }
                        else if (sorreszek[1] == "kis")
                        {
                            string[] ellenfelek = sorreszek[3].Split(',');
                            kazamatakicsi[sorreszek[2]] = (ellenfelek[0], ellenfelek[1], ellenfelek[2], sorreszek[4], sorreszek[5]);
                        }
                        else if (sorreszek[1] == "nagy")
                        {
                            string[] ellenfelek = sorreszek[3].Split(',');
                            kazamatanagy[sorreszek[2]] = (ellenfelek[0], ellenfelek[1], ellenfelek[2], ellenfelek[3], ellenfelek[4], sorreszek[4]);
                        }
                    }
                }
                if (sorreszek[0] == "uj jatekos")
                {
                    ujjatakos = true;
                }
                if (sorreszek[0] == "felvetel gyujtemenybe")
                {
                    foreach (int i in kartyak.Keys)
                    {
                        if (kartyak[i].Item1 == sorreszek[1])
                        {
                            playercards[playercardsn] = (kartyak[i].Item1, kartyak[i].Item2, kartyak[i].Item3, kartyak[i].Item4);
                            playercardsn++;
                        }
                    }
                }
                if (sorreszek[0] == "uj pakli")
                {
                    Pakliba(sorreszek[1]);
                    break;
                }
            }
        }

        private void Pakliba(string c)
        {
            Pakli.Clear();
            string[] nevek = c.Split(',');
            int max;
            if (nevek.Length % 2 == 1)
            {
                max = (nevek.Length + 1) / 2;
            }
            else
            {
                max = nevek.Length / 2;
            }

            foreach (string j in nevek)
            {
                Pakli.Add(j);
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
                    Environment.Exit(0);
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
                        //Harc(sorreszek[1], sorreszek[2]);
                    }
                    else if (sorreszek[0] == "export vilag")
                    {
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
            int type = -1;
            string[] simaellefelek = new string[4];
            string vezer = "";
            //var actualstatsk = 
            foreach (string tnev in kazamataegyszeru.Keys)
            {
                if (tnev == nev)
                {
                    type = 0;
                    simaellefelek[0] = kazamataegyszeru[tnev].Item1;
                    simaellefelek[1] = kazamataegyszeru[tnev].Item2;
                }
            }
            if (type == -1)
            {
                foreach (string tnev in kazamatakicsi.Keys)
                {
                    if (tnev == nev)
                    {
                        type = 1;
                        simaellefelek[0] = kazamatakicsi[tnev].Item1;
                        simaellefelek[1] = kazamatakicsi[tnev].Item2;
                        simaellefelek[2] = kazamatakicsi[tnev].Item3;
                        vezer = kazamatakicsi[tnev].Item4;
                    }
                }
            }
            if (type == -1)
            {
                foreach (string tnev in kazamatanagy.Keys)
                {
                    if (tnev == nev)
                    {
                        type = 2;
                        simaellefelek[0] = kazamatanagy[tnev].Item1;
                        simaellefelek[1] = kazamatanagy[tnev].Item2;
                        simaellefelek[2] = kazamatanagy[tnev].Item3;
                        simaellefelek[3] = kazamatanagy[tnev].Item4;
                        simaellefelek[4] = kazamatanagy[tnev].Item5;
                        vezer = kazamatanagy[tnev].Item6;
                    }
                }
            }
            string[] harcosok = Pakli.ToArray();

        }

        private void ExportState(string típus, string output)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(Path.GetDirectoryName(args[1]), output));
            if (típus == "vilag")
            {
                foreach (int i in kartyak.Keys)
                {
                    sw.WriteLine("kartya;" + kartyak[i].Item1 + ";" + kartyak[i].Item2.ToString() + ";" + kartyak[i].Item3.ToString() + ";" + kartyak[i].Item4.ToString());
                }
                sw.WriteLine();
                foreach (int i in vezerkartyak.Keys)
                {
                    sw.WriteLine("vezer;" + kartyak[i].Item1 + ";" + kartyak[i].Item2.ToString() + ";" + kartyak[i].Item3.ToString() + ";" + kartyak[i].Item4.ToString());
                }
                sw.WriteLine();
                foreach (string nev in kazamataegyszeru.Keys)
                {
                    string w = string.Join(";", kazamataegyszeru[nev]);
                    w.Insert(0, nev + ";");
                    sw.WriteLine("kazamata;egyszeru;" + w);
                }
                foreach (string nev in kazamatakicsi.Keys)
                {
                    string w = string.Join(";", kazamatakicsi[nev]);
                    w.Insert(0, nev + ";");
                    sw.WriteLine("kazamata;kis;" + w);
                }
                foreach (string nev in kazamatanagy.Keys)
                {
                    string w = string.Join(";", kazamatanagy[nev]);
                    w.Insert(0, nev + ";");
                    sw.WriteLine("kazamata;nagy;" + w);
                }
            }
            else if (típus == "jatekos")
            {
                foreach (int i in playercards.Keys)
                {
                    sw.WriteLine("gyujtemeny;" + playercards[i].Item1.ToString() + ";" + playercards[i].Item2.ToString() + ";" + playercards[i].Item3.ToString() + ";" + playercards[i].Item4.ToString());
                }
                sw.WriteLine();
                foreach (string i in Pakli)
                {
                    sw.WriteLine("pakli;" + i.ToString());
                }
            }
            sw.Close();
        }
    }
}
