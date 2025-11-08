using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            string bemenet = "";
            if (args[1].EndsWith("/") || args[1].EndsWith("\\"))
            {
                bemenet = "in.txt";
            }
            else
            {
                bemenet = "/in.txt";
            }
            try
            {
                StreamReader tesztsr = new StreamReader(args[1] + bemenet);
                tesztsr.Close();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            StreamReader sr = new StreamReader (args[1] + bemenet);

            Világsoronként(sr);

            ReadNextLine(sr, bemenet);
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

        private void ReadNextLine(StreamReader sr, string bemenet)
        {
            while (true)
            {
                string sor = sr.ReadLine();
                if (string.IsNullOrEmpty(sor) && sor != "")
                {
                    sr.Close();
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
                        Harc(sorreszek[1], sorreszek[2], bemenet);
                    }
                    else if (sorreszek[0] == "export vilag")
                    {

                        ExportState("vilag", sorreszek[1], bemenet);
                    }
                    else if (sorreszek[0] == "export jatekos")
                    {
                        ExportState("jatekos", sorreszek[1], bemenet);
                    }
                }
            }
        }

        private void Harc(string nev, string output, string bemenet)
        {
            int type = 0;
            Queue<string> simaellenfelek = new Queue<string>();
            string jutalom = "";
            bool veszitett = false;

            string jharcos = "";
            int jéleth = 0;
            int jsebzésh = 0;
            string jtípush = "";

            string jellenfél = "";
            int jélete = 0;
            int jsebzése = 0;
            string jtípuse = "";
            string vezer = "üres";

            int kör = 0;
            foreach (string tnev in kazamataegyszeru.Keys)
            {
                if (tnev == nev)
                {
                    type = 1;
                    simaellenfelek.Enqueue(kazamataegyszeru[tnev].Item1);
                    jutalom = kazamataegyszeru[tnev].Item2;
                }
            }
            if (type == 0)
            {
                foreach (string tnev in kazamatakicsi.Keys)
                {
                    if (tnev == nev)
                    {
                        type = 3;
                        simaellenfelek.Enqueue(kazamatakicsi[tnev].Item1);
                        simaellenfelek.Enqueue(kazamatakicsi[tnev].Item2);
                        simaellenfelek.Enqueue(kazamatakicsi[tnev].Item3);
                        vezer = kazamatakicsi[tnev].Item4;
                        jutalom = kazamatakicsi[tnev].Item5;
                    }
                }
            }
            if (type == 0)
            {
                foreach (string tnev in kazamatanagy.Keys)
                {
                    if (tnev == nev)
                    {
                        type = 5;
                        simaellenfelek.Enqueue(kazamatanagy[tnev].Item1);
                        simaellenfelek.Enqueue(kazamatanagy[tnev].Item2);
                        simaellenfelek.Enqueue(kazamatanagy[tnev].Item3);
                        simaellenfelek.Enqueue(kazamatanagy[tnev].Item4);
                        simaellenfelek.Enqueue(kazamatanagy[tnev].Item5);
                        vezer = kazamatanagy[tnev].Item6;
                        jutalom = "kartya";
                    }
                }
            }

            Queue<string> harcosok = new Queue<string>();
            foreach (string k in Pakli)
            {
                harcosok.Enqueue(k);
            }
            string kimenet = "";
            if (bemenet.StartsWith("/"))
            {
                kimenet = "/" + output;
            }
            else
            {
                kimenet = output;
            }
            StreamWriter swharc = new StreamWriter(args[1] + kimenet);
            swharc.WriteLine("harc kezdodik;" + nev);
            swharc.WriteLine();
            kör++;


            while (true)
            {
                if (harcosok.Count == 0 && jharcos == "")
                {
                    swharc.WriteLine("jatekos vesztett");
                    swharc.Close();
                    veszitett = true;
                    break;
                }
                if (jellenfél == "") //kazamata kezd
                {
                    if (simaellenfelek.Count > 0) {
                        jellenfél = simaellenfelek.Dequeue();
                        foreach (int i in kartyak.Keys) {
                            if (kartyak[i].Item1 == jellenfél)
                            {
                                jélete = kartyak[i].Item3;
                                jsebzése = kartyak[i].Item2;
                                jtípuse = kartyak[i].Item4;
                                swharc.WriteLine(kör + ".kor;kazamata;kijatszik;" + jellenfél +";"+ jsebzése + ";" + jélete + ";" + jtípuse);
                            }
                        }
                    }
                    else
                    {
                        if (vezer != "üres") {
                            foreach (int i in vezerkartyak.Keys)
                            {
                                jellenfél = vezer;
                                if (vezerkartyak[i].Item1 == jellenfél)
                                {
                                    jélete = kartyak[i].Item3;
                                    jsebzése = kartyak[i].Item2;
                                    jtípuse = kartyak[i].Item4;
                                    swharc.WriteLine(kör + ".kor;kazamata;kijatszik;" + jellenfél + ";" + jsebzése + ";" + jélete + ";" + jtípuse);
                                }
                            } 
                        }
                        else break;
                    }
                }
                else if (jellenfél != "") //kazamata támad, kivel, mennyi sebzés (típussal), kire, mennyi élet marad
                {
                    if (jtípuse == jtípush)
                    {
                        jéleth -= jsebzése;
                        swharc.WriteLine(kör + ".kor;kazamata;tamad;" + jellenfél + ";" + jsebzése + ";" + jharcos + ";" + jéleth);
                    }
                    else if ((jtípuse == "tuz" && jtípush == "levego") || (jtípuse == "levego" && jtípush == "tuz") || (jtípuse == "fold" && jtípush == "viz") || (jtípuse == "viz" && jtípush == "fold"))
                    {
                        jéleth -= jsebzése/2;
                        swharc.WriteLine(kör + ".kor;kazamata;tamad;" + jellenfél + ";" + jsebzése / 2 + ";" + jharcos + ";" + jéleth);
                    }
                    else
                    {
                        jéleth -= jsebzése * 2;
                        swharc.WriteLine(kör + ".kor;kazamata;tamad;" + jellenfél + ";" + jsebzése * 2 + ";" + jharcos + ";" + jéleth);
                    }
                    if (jéleth <= 0)
                    {
                        jharcos = "";
                        jéleth = 0;
                        jsebzésh = 0;
                        jtípush = "";
                    }
                }

                if (jharcos == "") //játékos jön
                {
                    if (harcosok.Count != 0) {
                        jharcos = harcosok.Dequeue();
                        foreach (int i in kartyak.Keys)
                        {
                            if (kartyak[i].Item1 == jharcos)
                            {
                                jéleth = kartyak[i].Item3;
                                jsebzésh = kartyak[i].Item2;
                                jtípush = kartyak[i].Item4;
                                swharc.WriteLine(kör + ".kor;jatekos;kijatszik;" + jharcos + ";" + jsebzésh + ";" + jéleth + ";" + jtípush);
                            }
                        }
                    }
                    else
                    {
                        swharc.WriteLine("jatekos vesztett");
                        swharc.Close();
                        veszitett = true;
                        break;
                    }
                }
                else //játékos támad, kivel, mennyi sebzés (típussal), kire, mennyi élet marad
                {
                    if (jtípuse == jtípush)
                    {
                        jélete -= jsebzésh;
                        swharc.WriteLine(kör + ".kor;jatekos;tamad;" + jharcos + ";" + jsebzésh + ";" + jellenfél + ";" + jélete);
                    }
                    else if ((jtípuse == "tuz" && jtípush == "levego") || (jtípuse == "levego" && jtípush == "tuz") || (jtípuse == "fold" && jtípush == "viz") || (jtípuse == "viz" && jtípush == "fold"))
                    {
                        jélete -= jsebzésh / 2;
                        swharc.WriteLine(kör + ".kor;jatekos;tamad;" + jharcos + ";" + jsebzésh / 2 + ";" + jellenfél + ";" + jélete);
                    }
                    else
                    {
                        jélete -= jsebzésh * 2;
                        swharc.WriteLine(kör + ".kor;jatekos;tamad;" + jharcos + ";" + jsebzésh * 2 + ";" + jellenfél + ";" + jélete);
                    }
                    if (jélete <= 0)
                    {
                        jellenfél = "";
                        jélete = 0;
                        jsebzése = 0;
                        jtípuse = "";
                    }
                }
                swharc.WriteLine();
                kör++;
            }
            if (!veszitett)
            {
                if (jutalom == "")
                {
                    foreach (int i in kartyak.Keys)
                    {
                        bool van = false;
                        foreach (int j in playercards.Keys)
                        {
                            if (playercards[j].Item1 == kartyak[i].Item1)
                            {
                                van = true;
                            }
                        }
                        if (!van)
                        {
                            playercards.Add(playercards.Count - 1, kartyak[i]);
                            swharc.WriteLine("jatekos nyert;" + kartyak[i].Item1);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (int i in playercards.Keys)
                    {
                        if (playercards[i].Item1 == jharcos)
                        {
                            if (jutalom == "eletero")
                            {
                                var card = playercards[i];
                                card.Item3 += 2;
                                playercards[i] = card;
                                swharc.WriteLine("jatekos nyert;" + jutalom + ";" + jharcos);
                                break;
                            }
                            else if (jutalom == "sebzes")
                            {
                                var card = playercards[i];
                                card.Item2 += 1;
                                playercards[i] = card;
                                swharc.WriteLine("jatekos nyert;" + jutalom + ";" + jharcos);
                                break;
                            }
                        }
                    }
                }
                swharc.Close();
            }
        }

        private void ExportState(string típus, string output, string bemenet)
        {
            string kimenet = "";
            if (bemenet.StartsWith("/"))
            {
                kimenet = "/" + output;
            }
            else
            {
                kimenet = output;
            }
            StreamWriter sw = new StreamWriter(args[1] + kimenet);
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
