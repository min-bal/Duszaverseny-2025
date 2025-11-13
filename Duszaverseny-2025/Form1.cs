using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Duszaverseny_2025
{ 
    public partial class Form1 : Form
    {
        int paklix = 5; //pakliba levo kartyak elhelyezesehez kell
        Dictionary<int, (string, int, int, string)> kartyak = new Dictionary<int, (string, int, int, string)> //név, sebzés, életerő, típus
        {
            {0, ("Arin", 2, 5, "Föld") },
            {1, ("Liora", 2, 4, "Levegő") },
            {2, ("Nerun", 3, 3, "Tűz") },
            {3, ("Selia", 2, 6, "Víz") },
            {4, ("Torak", 3, 4, "Föld") },
            {5, ("Emera", 2, 5, "Levegő") },
            {6, ("Vorn", 2, 7, "Víz") },
            {7, ("Kael", 3, 5, "Tűz") },
            {8, ("Myra", 2, 6, "Föld") },
            {9, ("Thalen", 3, 5, "Levegő") },
            {10, ("Isara", 2, 6, "Víz") }
        }; 
        Dictionary<int, (string, int, int, string, string)> vezerkartyak = new Dictionary<int, (string, int, int, string, string)>
        {
            {0, ("Lord Torak", 6, 4, "Föld", "(Vezér)") },
            {1, ("Priestess Selia", 2, 12, "Víz", "(Vezér)") }
        };
        Dictionary<int, (string, int, int, string)> playercards = new Dictionary<int, (string, int, int, string)> //properies of the player's cards
        {
            {0, ("Arin", 2, 5, "Föld") },
            {1, ("Liora", 2, 4, "Levegő") },
            {2, ("Selia", 2, 6, "Víz") },
            {3, ("Nerun", 3, 3, "Tűz") },
            {4, ("Torak", 3, 4, "Föld") },
            {5, ("Emera", 2, 5, "Levegő") },
            {6, ("Kael", 3, 5, "Tűz") },
            {7, ("Myra", 2, 6, "Föld") },
            {8, ("Thalen", 3, 5, "Levegő") },
            {9, ("Isara", 2, 6, "Víz") }
        };

        Dictionary<string, (string, string)> kazamataegyszeru = new Dictionary<string, (string, string)> //név, ellenfél, jutalom
        {
            {"Barlangi Portya", ("Nerun", "sebzés") }
        };
        Dictionary<string, (string, string, string, string, string)> kazamatakicsi = new Dictionary<string, (string, string, string, string, string)> //név, ellenfél*3, vezér, jutalom
        {
            {"Ősi Szentély", ("Arin", "Emera", "Selia", "Lord Torak", "életerő") }
        }; 
        Dictionary<string, (string, string, string, string, string, string)> kazamatanagy = new Dictionary<string, (string, string, string, string, string, string)> //név, ellenfél*5, vezér
        {
            {"A mélység királynője", ("Liora", "Arin", "Selia", "Nerun", "Torak", "Priestess Selia") }
        };
        List<string> Pakli = new List<string>();

        private void Label(string name, string displaytext, int x, int y, int fontsize, int width, int height, bool border)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Name = name;
            label.Text = displaytext;
            label.Font = new Font("Microsoft Sans Seriff", fontsize);
            label.Location = new Point(x, y);
            label.Size = new System.Drawing.Size(width, height);
            if (border) {
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.BorderStyle = BorderStyle.FixedSingle;
            }
            this.Controls.Add(label);
        }

        private void Buttons(string name, string displaytext, int x, int y, int fontsize, int width, int height, EventHandler onClick)
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.Name = name;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Font = new System.Drawing.Font("Microsoft Sans Serif", fontsize);
            button.Text = displaytext;
            button.Size = new Size(width, height);
            button.Location = new Point(x, y);
            button.Click += onClick;
            this.Controls.Add(button);
        }

        private void MainScreen()
        {
            int x = 5;
            string EXTRAINFO = "Pakli módosításához nyomd meg az alábbi kártyákat." + Environment.NewLine + "Pakli kiürítéséhez nyomd meg az új pakli gombot." + Environment.NewLine + "A pakliban gyűjteményednek legfeljebb fele szerepelhet.";
            
            Label("info1", "Kártyák:",3,0,14,100,30, false);
            for (int i = 0; i < 11; i++)
            {
                string disptext = kartyak[i].Item1 + Environment.NewLine + kartyak[i].Item2 + "/" + kartyak[i].Item3 + Environment.NewLine + kartyak[i].Item4;
                Label("kartyak"+i.ToString(),disptext,x,30,8,85,100,true);
                x = x + 99;
            }
            x = 5;

            Label("info2", "Vezérek:", 3, 150, 14, 150, 30, false);
            foreach (int i in vezerkartyak.Keys)
            {
                string disptext = vezerkartyak[i].Item1 + Environment.NewLine + vezerkartyak[i].Item2 + "/" + vezerkartyak[i].Item3 + Environment.NewLine + vezerkartyak[i].Item4;
                Label("vezerek" + i.ToString(), disptext, x, 180, 8, 85, 100, true);
                x = x + 99;
            }
            x = 5;

            Label("info3", "Te kártyáid:",3,300,14,150,30, false);
            x = 5;
            foreach (int i in playercards.Keys)
            {
                string disptext = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + playercards[i].Item4;
                Buttons("gyujtemeny"+i.ToString(),disptext,x,340,8,85,100,Button_Click);
                x = x + 99;
            }
            x = 5;

            Label("info4","Paklid:",3,450,14,110,30,false);
            Buttons("ÚjPakli","Új pakli",120,443,12,120,40,ÚjPakliGomb_Click);
            Label("extrainfo",EXTRAINFO,302,270,12,800,70,false);

            Label("info5","Kazamaták:",2,600,14,150,30,false);

            foreach (string nev in kazamataegyszeru.Keys)
            {
                x = 5;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = "kazmgomb1";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 630);
                lbl.Enabled = false;
                lbl.Click += (s, e) => Harc("egyszeru");
                this.Controls.Add(lbl);
                x = x + 99;
                Label("kazm0ell1", kazamataegyszeru[nev].Item1, x, 633, 8, 85, 44, true);
                x += 99;
                Label("kazm0jut", "Jutalom: " + kazamataegyszeru[nev].Item2, x, 640, 8, 85, 30, true);
                x += 99;
                Label("kazm0info", "Típus: egyszerű", x, 648, 8, 85, 30, false);
            }
            foreach (string nev in kazamatakicsi.Keys)
            {
                x = 5;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = "kazmgomb2";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 680);
                lbl.Enabled = false;
                lbl.Click += (s, e) => Harc("kicsi");
                this.Controls.Add(lbl);
                x = x + 99;

                Label("kazm1ell1", kazamatakicsi[nev].Item1, x, 683, 8, 85, 44, true);
                x += 99;
                Label("kazm1ell2", kazamatakicsi[nev].Item2, x, 683, 8, 85, 44, true);
                x += 99;
                Label("kazm1ell3", kazamatakicsi[nev].Item3, x, 683, 8, 85, 44, true);
                x += 99;
                Label("kazm1ell4", "Vezér:" + Environment.NewLine + kazamatakicsi[nev].Item4, x, 683, 8, 85, 44, true);
                x += 99;
                Label("kazm1jut", "Jutalom: " + kazamatakicsi[nev].Item5, x, 690, 8, 85, 30, true);
                x += 109;
                Label("kazm1info", "Típus: kicsi", x, 698, 8, 85, 30, false);
            }
            foreach (string nev in kazamatanagy.Keys)
            {
                x = 5;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = "kazmgomb3";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 730);
                lbl.Enabled = false;
                lbl.Click += (s, e) => Harc("nagy");
                this.Controls.Add(lbl);
                x = x + 99;

                Label("kazm2ell1", kazamatanagy[nev].Item1, x, 733, 8, 85, 44, true);
                x += 99;
                Label("kazm2ell2", kazamatanagy[nev].Item2, x, 733, 8, 85, 44, true);
                x += 99;
                Label("kazm2ell3", kazamatanagy[nev].Item3, x, 733, 8, 85, 44, true);
                x += 99;
                Label("kazm2ell4", kazamatanagy[nev].Item4, x, 733, 8, 85, 44, true);
                x += 99;
                Label("kazm2ell5", kazamatanagy[nev].Item5, x, 733, 8, 85, 44, true);
                x += 99;
                Label("kazm2ell6", "Vezér:" + Environment.NewLine + kazamatanagy[nev].Item6, x, 733, 8, 85, 44, true);
                x += 104;
                Label("kazm2info", "Típus: nagy", x, 748, 8, 85, 44, false);
            }
        }


        List<int> paklint = new List<int>();
        private void Button_Click(object sender, EventArgs e)
        {
            Button clicked = sender as Button;

            clicked.Enabled = false;

            int felekerint;
            if (playercards.Count % 2 == 0)
            {
                felekerint = playercards.Count / 2;
            }
            else
            {
                felekerint = (playercards.Count + 1)/2;
            }

            if (Pakli.Count < felekerint)
            {
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.AutoSize = false;

                int nmb;
                if (clicked.Name.Length == 11)
                {
                    nmb = int.Parse(clicked.Name.Last().ToString());
                }
                else
                {
                    nmb = int.Parse(clicked.Name.Substring(clicked.Name.Length - 2));
                }
                paklint.Add(nmb);
                lbl.Name = "paklibtn" + nmb;
                lbl.Text = playercards[nmb].Item1.ToString() + "\r\n" + playercards[nmb].Item2.ToString() + "/" + playercards[nmb].Item3.ToString() + "\r\n" + playercards[nmb].Item4.ToString();
                lbl.Size = new Size(85, 100);
                lbl.Location = new Point(paklix, 490);
                lbl.Click += new System.EventHandler(PakliKartyakClick);
                this.Controls.Add(lbl);
                paklix += 99;
                Pakli.Add(playercards[nmb].Item1);
                foreach (var btn in this.Controls.OfType<Button>())
                {                                 
                    if (btn.Name == "kazmgomb1" || btn.Name == "kazmgomb2")
                    {
                        btn.Enabled = true;
                    }
                    else if (btn.Name == "kazmgomb3" && kartyak.Count != playercards.Count)
                    {
                        btn.Enabled = true;
                    }
                }
            }
            if (Pakli.Count == felekerint)
            {
                foreach (var btn in this.Controls.OfType<Button>().Where(b => b.Name.StartsWith("gyujtemeny")).ToList())
                {
                    btn.Enabled = false;
                }
                //KészPakli_Click(this, EventArgs.Empty);
            }

        }

        private void PakliKartyakClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int nmb;
            if (button.Name.Length == 9)
            {
                nmb = int.Parse(button.Name.Last().ToString());
            }
            else
            {
                nmb = int.Parse(button.Name.Substring(button.Name.Length - 2));
            }
            string last = nmb.ToString();
            string asd = playercards[Convert.ToInt32(last)].Item1;
            Pakli.Remove(asd);
            paklint.Remove(nmb);
            foreach (var btn in this.Controls.OfType<Button>())
            {
                if (btn.Location.Y == button.Location.Y && btn.Location.X > button.Location.X)
                {
                    btn.Location = new Point(btn.Location.X - 99, btn.Location.Y);
                }
                else if (btn.Name == "gyujtemeny" + last)
                {
                    btn.Enabled = true;
                }
                if ((btn.Name == "kazmgomb1" || btn.Name == "kazmgomb2" || btn.Name == "kazmgomb3") && Pakli.Count == 0)
                {
                    btn.Enabled = false;
                }
            }
            
            paklix -= 99;
            Controls.Remove(button);
            button.Dispose();

            foreach (int i in playercards.Keys)
            {
                if (paklint.Contains(i))
                {

                }
                else
                {
                    string buttonName = "gyujtemeny" + i;
                    Control[] found = this.Controls.Find(buttonName, true);
                    if (found.Length > 0 && found[0] is Button btn)
                    {
                        btn.Enabled = true;
                    }
                }
            }
        }

        private void ÚjPakliGomb_Click(object sender, EventArgs e)
        {
            paklix = 0;
            Pakli.Clear();
            paklint.Clear();
            foreach (Control ctrl in Controls)
            {
                if (ctrl is Button btn)
                {
                    if (btn.Name.StartsWith("gyujtemeny"))
                    {
                        btn.Enabled = true;
                    }
                    else if (btn.Name == "KészPakli")
                    {
                        btn.Enabled = true;
                    }
                }
            }
            foreach (var btn in this.Controls.OfType<Button>().Where(b => b.Name.StartsWith("paklibtn")).ToList())
            {
                this.Controls.Remove(btn);
                btn.Dispose();
            }
            foreach (var btn in this.Controls.OfType<Button>())
            {
                if (btn.Name == "kazmgomb1" || btn.Name == "kazmgomb2" || btn.Name == "kazmgomb3")
                {
                    btn.Enabled = false;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            MainScreen();

        }

        private void Harc(string típus)
        {
            Panel harctér = new Panel();
            harctér.BackColor = Color.LightGray;
            harctér.Dock = DockStyle.Fill;
            this.Controls.Add(harctér);
            harctér.BringToFront();

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

            int Xh = 0;
            int Xe = 70;
            int Y = 50;

            if (típus == "egyszeru")
            {
                simaellenfelek.Enqueue(kazamataegyszeru["Barlangi Portya"].Item1);
                jutalom = kazamataegyszeru["Barlangi Portya"].Item2;
            }
            if (típus == "kicsi")
            {
                simaellenfelek.Enqueue(kazamatakicsi["Ősi Szentély"].Item1);
                simaellenfelek.Enqueue(kazamatakicsi["Ősi Szentély"].Item2);
                simaellenfelek.Enqueue(kazamatakicsi["Ősi Szentély"].Item3);
                vezer = kazamatakicsi["Ősi Szentély"].Item4;
                jutalom = kazamatakicsi["Ősi Szentély"].Item5;
            }
            if (típus == "nagy")
            {
                simaellenfelek.Enqueue(kazamatanagy["A mélység királynője"].Item1);
                simaellenfelek.Enqueue(kazamatanagy["A mélység királynője"].Item2);
                simaellenfelek.Enqueue(kazamatanagy["A mélység királynője"].Item3);
                simaellenfelek.Enqueue(kazamatanagy["A mélység királynője"].Item4);
                simaellenfelek.Enqueue(kazamatanagy["A mélység királynője"].Item5);
                vezer = kazamatanagy["A mélység királynője"].Item6;
                jutalom = "kartya";
            }

            Queue<string> harcosok = new Queue<string>();
            foreach (string k in Pakli)
            {
                harcosok.Enqueue(k);
            }

            jellenfél = simaellenfelek.Dequeue();
            foreach (int i in kartyak.Keys)
            {
                if (kartyak[i].Item1 == jellenfél)
                {
                    jélete = kartyak[i].Item3;
                    jsebzése = kartyak[i].Item2;
                    jtípuse = kartyak[i].Item4;
                    System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                    ell1.Name = "jellenfél";
                    ell1.TextAlign = ContentAlignment.MiddleCenter;
                    ell1.Text = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                    ell1.BorderStyle = BorderStyle.FixedSingle;
                    ell1.Size = new System.Drawing.Size(70, 50);
                    ell1.Location = new Point(Xe, Y);
                    harctér.Controls.Add(ell1);
                }
            }

            jharcos = harcosok.Dequeue();
            foreach (int i in playercards.Keys)
            {
                if (playercards[i].Item1 == jharcos)
                {
                    jéleth = playercards[i].Item3;
                    jsebzésh = playercards[i].Item2;
                    jtípush = playercards[i].Item4;
                    System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                    harcos1.Name = "jharcos";
                    harcos1.TextAlign = ContentAlignment.MiddleCenter;
                    harcos1.Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                    harcos1.BorderStyle = BorderStyle.FixedSingle;
                    harcos1.Size = new System.Drawing.Size(70, 50);
                    harcos1.Location = new Point(Xh, Y);
                    harctér.Controls.Add(harcos1);
                }
            }
            Y += 70;
            
            while (true)
            {                                                
                if (harcosok.Count == 0 && jharcos == "")
                {
                    veszitett = true;
                    break;
                }
                if (jellenfél == "") //kazamata uj kartyat hoz be?
                {
                    if (simaellenfelek.Count > 0)
                    {
                        jellenfél = simaellenfelek.Dequeue();
                        foreach (int i in kartyak.Keys)
                        {
                            if (kartyak[i].Item1 == jellenfél)
                            {
                                jélete = kartyak[i].Item3;
                                jsebzése = kartyak[i].Item2;
                                jtípuse = kartyak[i].Item4;
                                System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                                ell1.Name = "jellenfél";
                                ell1.TextAlign = ContentAlignment.MiddleCenter;
                                ell1.Text = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                                ell1.BorderStyle = BorderStyle.FixedSingle;
                                ell1.Size = new System.Drawing.Size(70, 50);
                                ell1.Location = new Point(Xe, Y);
                                harctér.Controls.Add(ell1);

                                System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                                harcos1.Name = "jharcos";
                                harcos1.TextAlign = ContentAlignment.MiddleCenter;
                                harcos1.Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                                harcos1.BorderStyle = BorderStyle.FixedSingle;
                                harcos1.Size = new System.Drawing.Size(70, 50);
                                harcos1.Location = new Point(Xh, Y);
                                harctér.Controls.Add(harcos1);

                                if (Y > 680)
                                {
                                    Y = 50;
                                    Xe += 200;
                                    Xh += 200;
                                }
                                else
                                {
                                    Y += 70;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (vezer != "üres")
                        {
                            foreach (int i in vezerkartyak.Keys)
                            {
                                jellenfél = vezer;
                                if (vezerkartyak[i].Item1 == jellenfél)
                                {
                                    jélete = vezerkartyak[i].Item3;
                                    jsebzése = vezerkartyak[i].Item2;
                                    jtípuse = vezerkartyak[i].Item4;
                                    System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                                    ell1.Name = "jellenfél";
                                    ell1.TextAlign = ContentAlignment.MiddleCenter;
                                    ell1.Text = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                                    ell1.BorderStyle = BorderStyle.FixedSingle;
                                    ell1.Size = new System.Drawing.Size(70, 50);
                                    ell1.Location = new Point(Xe, Y);
                                    harctér.Controls.Add(ell1);

                                    System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                                    harcos1.Name = "jharcos";
                                    harcos1.TextAlign = ContentAlignment.MiddleCenter;
                                    harcos1.Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                                    harcos1.BorderStyle = BorderStyle.FixedSingle;
                                    harcos1.Size = new System.Drawing.Size(70, 50);
                                    harcos1.Location = new Point(Xh, Y);
                                    harctér.Controls.Add(harcos1);

                                    if (Y > 680)
                                    {
                                        Y = 50;
                                        Xe += 200;
                                        Xh += 200;
                                    }
                                    else
                                    {
                                        Y += 70;
                                    }
                                }
                            }
                            vezer = "üres";
                        }
                        else break;
                    }
                }
                else if (jellenfél != "" && jharcos != "") //kazamata támad, kivel, mennyi sebzés (típussal), kire, mennyi élet marad
                {
                    if (jtípuse == jtípush)
                    {
                        jéleth -= jsebzése;
                    }
                    else if ((jtípuse == "Tűz" && jtípush == "Levegő") || (jtípuse == "Levegő" && jtípush == "Tűz") || (jtípuse == "Föld" && jtípush == "Víz") || (jtípuse == "Víz" && jtípush == "Föld"))
                    {
                        jéleth -= jsebzése / 2;
                    }
                    else
                    {
                        jéleth -= jsebzése * 2;
                    }

                    if (jéleth < 0)
                    {
                        jéleth = 0;
                    }

                    System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                    ell1.Name = "jellenfél";
                    ell1.TextAlign = ContentAlignment.MiddleCenter;
                    ell1.Text = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                    ell1.BorderStyle = BorderStyle.FixedSingle;
                    ell1.Size = new System.Drawing.Size(70, 50);
                    ell1.Location = new Point(Xe, Y);
                    harctér.Controls.Add(ell1);

                    System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                    harcos1.Name = "jharcos";
                    harcos1.TextAlign = ContentAlignment.MiddleCenter;
                    harcos1.Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                    harcos1.BorderStyle = BorderStyle.FixedSingle;
                    harcos1.Size = new System.Drawing.Size(70, 50);
                    harcos1.Location = new Point(Xh, Y);
                    harctér.Controls.Add(harcos1);

                    if (Y > 680)
                    {
                        Y = 50;
                        Xe += 200;
                        Xh += 200;
                    }
                    else
                    {
                        Y += 70;
                    }

                    if (jéleth <= 0)
                    {
                        jharcos = "";
                        jéleth = 0;
                        jsebzésh = 0;
                        jtípush = "";
                    }
                }
                if (jharcos == "") //játékos hoz elo kartyat?
                {
                    if (harcosok.Count != 0)
                    {
                        jharcos = harcosok.Dequeue();
                        foreach (int i in playercards.Keys)
                        {
                            if (playercards[i].Item1 == jharcos)
                            {
                                jéleth = playercards[i].Item3;
                                jsebzésh = playercards[i].Item2;
                                jtípush = playercards[i].Item4;
                                System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                                harcos1.Name = "jharcos";
                                harcos1.TextAlign = ContentAlignment.MiddleCenter;
                                harcos1.Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                                harcos1.BorderStyle = BorderStyle.FixedSingle;
                                harcos1.Size = new System.Drawing.Size(70, 50);
                                harcos1.Location = new Point(Xh, Y);
                                harctér.Controls.Add(harcos1);

                                System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                                ell1.Name = "jellenfél";
                                ell1.TextAlign = ContentAlignment.MiddleCenter;
                                ell1.Text = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                                ell1.BorderStyle = BorderStyle.FixedSingle;
                                ell1.Size = new System.Drawing.Size(70, 50);
                                ell1.Location = new Point(Xe, Y);
                                harctér.Controls.Add(ell1);

                                if (Y > 680)
                                {
                                    Y = 50;
                                    Xe += 200;
                                    Xh += 200;
                                }
                                else
                                {
                                    Y += 70;
                                }
                            }
                        }
                    }
                    else
                    {
                        veszitett = true;
                        break;
                    }
                }
                else if (jharcos != "" && jellenfél != "") //játékos támad, kivel, mennyi sebzés (típussal), kire, mennyi élet marad
                {
                    if (jtípuse == jtípush)
                    {
                        jélete -= jsebzésh;
                    }
                    else if ((jtípuse == "Tűz" && jtípush == "Levegő") || (jtípuse == "Levegő" && jtípush == "Tűz") || (jtípuse == "Föld" && jtípush == "Víz") || (jtípuse == "Víz" && jtípush == "Föld"))
                    {
                        jélete -= jsebzésh / 2;
                    }
                    else
                    {
                        jélete -= jsebzésh * 2;
                    }

                    if (jélete < 0)
                    {
                        jélete = 0;
                    }

                    System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                    ell1.Name = "jellenfél";
                    ell1.TextAlign = ContentAlignment.MiddleCenter;
                    ell1.Text = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                    ell1.BorderStyle = BorderStyle.FixedSingle;
                    ell1.Size = new System.Drawing.Size(70, 50);
                    ell1.Location = new Point(Xe, Y);
                    harctér.Controls.Add(ell1);

                    System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                    harcos1.Name = "jharcos";
                    harcos1.TextAlign = ContentAlignment.MiddleCenter;
                    harcos1.Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                    harcos1.BorderStyle = BorderStyle.FixedSingle;
                    harcos1.Size = new System.Drawing.Size(70, 50);
                    harcos1.Location = new Point(Xh, Y);
                    harctér.Controls.Add(harcos1);

                    if (Y > 680)
                    {
                        Y = 50;
                        Xe += 200;
                        Xh += 200;
                    }
                    else
                    {
                        Y += 70;
                    }

                    if (jélete <= 0)
                    {
                        jellenfél = "";
                        jélete = 0;
                        jsebzése = 0;
                        jtípuse = "";
                    }
                }
            }

            Button back = new Button();
            back.Size = new System.Drawing.Size(100, 100);
            back.Location = new Point(900, 695);
            back.Text = "Vissza a főmenüre";
            back.Click += (s, e) => this.Controls.Remove(harctér);
            harctér.Controls.Add (back);
            System.Windows.Forms.Label nyeremeny = new System.Windows.Forms.Label();
            nyeremeny.Size = new System.Drawing.Size(100, 100);
            nyeremeny.Location = new Point(900, 495);
            nyeremeny.Text = "";
            nyeremeny.BorderStyle = BorderStyle.FixedSingle;
            harctér.Controls.Add(nyeremeny);
            if (veszitett)
            {
                nyeremeny.Text = "Játékos Veszített";
            }
            else
            {
                if (jutalom == "kartya")
                {
                    Button kikapcs = this.Controls["kazmgomb3"] as Button;
                    if (kikapcs != null)
                    {
                        kikapcs.Enabled = false;
                    }

                    Random rnd = new Random();
                    int ujkartya;
                    while (true)
                    {
                        ujkartya = rnd.Next(0, 11);
                        int j = 0;
                        foreach (int i in playercards.Keys)
                        {
                            if (kartyak[ujkartya].Item1 == playercards[i].Item1)
                            {
                                j++;
                            }
                        }
                        if (j == 0)
                        {
                            break;
                        }
                    }
                    playercards[playercards.Count] = (kartyak[ujkartya].Item1, kartyak[ujkartya].Item2, kartyak[ujkartya].Item3, kartyak[ujkartya].Item4);
                    nyeremeny.Text = "Játékos nyert!\nNyeremény: " + kartyak[ujkartya].Item1;

                    System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                    lbl.Name = "gyujtemeny" + (playercards.Count - 1).ToString();

                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Text = playercards[playercards.Count - 1].Item1 + Environment.NewLine + playercards[playercards.Count - 1].Item2 + "/" + playercards[playercards.Count - 1].Item3 + Environment.NewLine + playercards[playercards.Count - 1].Item4;
                    lbl.Size = new Size(85, 100);
                    lbl.Location = new Point(5 + (playercards.Count - 1) * 99, 340);
                    lbl.Click += Button_Click;
                    this.Controls.Add(lbl);

                    foreach (int i in playercards.Keys)
                    {
                        if (paklint.Contains(i))
                        {

                        }
                        else
                        {
                            string buttonName = "gyujtemeny" + i;
                            Control[] found = this.Controls.Find(buttonName, true);
                            if (found.Length > 0 && found[0] is Button btn)
                            {
                                btn.Enabled = true;
                            }
                        }
                    }


                }
                else
                {
                    foreach (int i in playercards.Keys)
                    {
                        if (playercards[i].Item1 == jharcos)
                        {
                            if (jutalom == "sebzés")
                            {
                                playercards[i] = (playercards[i].Item1, playercards[i].Item2 + 1, playercards[i].Item3, playercards[i].Item4);
                                nyeremeny.Text = "Játékos nyert" + Environment.NewLine +  "+1 sebzés: " + playercards[i].Item1;
                            }
                            else
                            {
                                playercards[i] = (playercards[i].Item1, playercards[i].Item2, playercards[i].Item3 + 2, playercards[i].Item4);
                                nyeremeny.Text = "Játékos nyert" + Environment.NewLine+ "+2 élet: " + playercards[i].Item1;
                            }
                            string buttonName = "gyujtemeny" + i;
                            Control[] found = this.Controls.Find(buttonName, true);
                            if (found.Length > 0 && found[0] is Button btn)
                            {
                                btn.Text = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + playercards[i].Item4;
                            }
                            string buttonName1 = "paklibtn" + i;
                            Control[] found1 = this.Controls.Find(buttonName1, true);
                            if (found1.Length > 0 && found1[0] is Button btn1)
                            {
                                btn1.Text = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + playercards[i].Item4;
                            }
                            break;
                        }
                    }
                }
            }            
        }
    }
}
