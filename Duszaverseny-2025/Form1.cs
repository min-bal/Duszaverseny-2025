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
    /*
     in.txt max. 200 sor
        formátuma mindig helyes
    sebzés/életerő: egész, 2-100/1-100
    vezéreket a sima kártyák után
    név max. 16 karakter
    kazamata neve max. 20
    gyűjteményben egy kártya csak egyszer
    uj pakli akárhányszor ismétlődhet
    kazamata elején életerő alaphelyzetbe
     */
    

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
        private void kartyaklbl()
        {
            int x = 5;
            System.Windows.Forms.Label info1lbl = new System.Windows.Forms.Label();
            info1lbl.Name = "info1";
            info1lbl.Text = "Kártyák:";
            info1lbl.Font = new Font("Microsoft Sans Seriff", 14);
            info1lbl.Location = new Point(3, 0);
            info1lbl.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(info1lbl);
            for (int i = 0; i < 11; i++)
            {
                System.Windows.Forms.Label lbl = new System.Windows.Forms.Label();
                lbl.Name = "kartyak" + i.ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.BorderStyle = BorderStyle.FixedSingle;
                lbl.Text = kartyak[i].Item1 + Environment.NewLine + kartyak[i].Item2 + "/" + kartyak[i].Item3 + Environment.NewLine + kartyak[i].Item4;
                lbl.Size = new Size(85, 100);
                lbl.Location = new Point(x, 30);
                this.Controls.Add(lbl);
                x = x + 99;
            }
        }

        private void Playerlabel()
        {
            int x = 5;
            System.Windows.Forms.Label info2lbl = new System.Windows.Forms.Label();
            info2lbl.Name = "info2";
            info2lbl.Text = "Te kártyáid:";
            info2lbl.Font = new Font("Microsoft Sans Seriff", 14);
            info2lbl.Location = new Point(2, 300);
            info2lbl.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(info2lbl);
            foreach (int i in playercards.Keys) { 
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = "gyujtemeny" + i.ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + playercards[i].Item4;
                lbl.Size = new Size(85, 100);
                lbl.Location = new Point(x, 340);
                lbl.Click += Button_Click;
                this.Controls.Add(lbl);
                x = x + 99;
            }
        }

        private void Paklilabel()
        {
            int x = 5;
            System.Windows.Forms.Label info3lbl = new System.Windows.Forms.Label();
            info3lbl.Name = "info3";
            info3lbl.Text = "Paklid:";
            info3lbl.Font = new Font("Microsoft Sans Seriff", 14);
            info3lbl.Location = new Point(3, 450);
            info3lbl.Size = new System.Drawing.Size(110, 30);
            this.Controls.Add(info3lbl);

            Button KészPakli = new System.Windows.Forms.Button();
            KészPakli.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            KészPakli.Location = new System.Drawing.Point(120, 443);
            KészPakli.Name = "KészPakli";
            KészPakli.Size = new System.Drawing.Size(150, 40);
            KészPakli.TabIndex = 0;
            KészPakli.Text = "Pakli használata";
            KészPakli.UseVisualStyleBackColor = true;
            KészPakli.Click += new System.EventHandler(KészPakli_Click);
            this.Controls.Add(KészPakli);
            Button ÚjPakli = new System.Windows.Forms.Button();
            ÚjPakli.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            ÚjPakli.Location = new System.Drawing.Point(300, 443);
            ÚjPakli.Name = "ÚjPakli";
            ÚjPakli.Size = new System.Drawing.Size(120, 40);
            ÚjPakli.TabIndex = 0;
            ÚjPakli.Text = "Új pakli";
            ÚjPakli.UseVisualStyleBackColor = true;
            ÚjPakli.Enabled = false;
            ÚjPakli.Click += new System.EventHandler(ÚjPakli_Click);
            this.Controls.Add(ÚjPakli);
        }

        private void vezerek()
        {
            int x = 5;
            System.Windows.Forms.Label info4lbl = new System.Windows.Forms.Label();
            info4lbl.Name = "info4";
            info4lbl.Text = "Vezérek:";
            info4lbl.Font = new Font("Microsoft Sans Seriff", 14);
            info4lbl.Location = new Point(2, 150);
            info4lbl.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(info4lbl);
            foreach (int i in vezerkartyak.Keys)
            {
                System.Windows.Forms.Label lbl = new System.Windows.Forms.Label();
                lbl.Name = "vezerek" + i.ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = vezerkartyak[i].Item1 + Environment.NewLine + vezerkartyak[i].Item2 + "/" + vezerkartyak[i].Item3 + Environment.NewLine + vezerkartyak[i].Item4;
                lbl.Size = new Size(85, 100);
                lbl.Location = new Point(x, 180);
                lbl.Click += Button_Click;
                this.Controls.Add(lbl);
                x = x + 99;
            }
        }

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

                lbl.Name = "paklibtn" + clicked.Name.Last();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.AutoSize = false;
                lbl.Text = playercards[Convert.ToInt32(clicked.Name.Last<char>().ToString())].Item1.ToString() + "\r\n" + playercards[Convert.ToInt32(clicked.Name.Last<char>().ToString())].Item2.ToString() + "/" + playercards[Convert.ToInt32(clicked.Name.Last<char>().ToString())].Item3.ToString() + "\r\n" + playercards[Convert.ToInt32(clicked.Name.Last<char>().ToString())].Item4.ToString();
                lbl.Size = new Size(85, 100);
                lbl.Location = new Point(paklix, 490);
                lbl.Click += new System.EventHandler(PakliClick);
                this.Controls.Add(lbl);
                paklix += 99;
                Pakli.Add(playercards[Convert.ToInt32(clicked.Name.Last().ToString())].Item1);
            }
            if (Pakli.Count == felekerint)
            {
                foreach (var btn in this.Controls.OfType<Button>().Where(b => b.Name.StartsWith("gyujtemeny")).ToList())
                {
                    btn.Enabled = false;
                }
                KészPakli_Click(this, EventArgs.Empty);
            }

        }

        public Form1()
        {
            InitializeComponent();
            kartyaklbl();
            Playerlabel();
            Paklilabel();
            vezerek();
            KazmataGombok();

        }

        private void KazmataGombok()
        {
            int x = 5;
            System.Windows.Forms.Label info5lbl = new System.Windows.Forms.Label();
            info5lbl.Name = "info5";
            info5lbl.Text = "Kazamaták:";
            info5lbl.Font = new Font("Microsoft Sans Seriff", 14);
            info5lbl.Location = new Point(2, 600);
            info5lbl.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(info5lbl);
            foreach (string nev in kazamataegyszeru.Keys)
            {
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = nev.ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 630);
                lbl.Click += (s, e) => Harc("egyszeru");
                this.Controls.Add(lbl);
                x = x + 99;

                System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                ell1.Name = "kazm0ell1";
                ell1.TextAlign = ContentAlignment.MiddleCenter;
                ell1.Text = kazamataegyszeru[nev].Item1;
                ell1.BorderStyle = BorderStyle.FixedSingle;
                ell1.Size = new Size(85, 44);
                ell1.Location = new Point(x, 633);
                this.Controls.Add(ell1);

                x += 99;
                System.Windows.Forms.Label jut1 = new System.Windows.Forms.Label();
                jut1.Name = "kazm0jut";
                jut1.TextAlign = ContentAlignment.MiddleCenter;
                jut1.Text = "Jutalom: " + kazamataegyszeru[nev].Item2;
                jut1.BorderStyle = BorderStyle.FixedSingle;
                jut1.Size = new Size(85, 30);
                jut1.Location = new Point(x, 640);
                this.Controls.Add(jut1);
                x += 99;
                System.Windows.Forms.Label info = new System.Windows.Forms.Label();
                info.Name = "info1";
                info.TextAlign = ContentAlignment.MiddleCenter;
                info.Text = "Típus: egyszerű";
                info.Size = new Size(85, 44);
                info.Location = new Point(x, 633);
                this.Controls.Add(info);
            }
            foreach (string nev in kazamatakicsi.Keys)
            {
                x = 5;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = nev.ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 680);
                lbl.Click += (s, e) => Harc("kicsi");
                this.Controls.Add(lbl);
                x = x + 99;

                System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                ell1.Name = "kazm1ell1";
                ell1.TextAlign = ContentAlignment.MiddleCenter;
                ell1.Text = kazamatakicsi[nev].Item1;
                ell1.BorderStyle = BorderStyle.FixedSingle;
                ell1.Size = new Size(85, 44);
                ell1.Location = new Point(x, 683);
                this.Controls.Add(ell1);
                x += 99;
                System.Windows.Forms.Label ell2 = new System.Windows.Forms.Label();
                ell2.Name = "kazm1ell1";
                ell2.TextAlign = ContentAlignment.MiddleCenter;
                ell2.Text = kazamatakicsi[nev].Item2;
                ell2.BorderStyle = BorderStyle.FixedSingle;
                ell2.Size = new Size(85, 44);
                ell2.Location = new Point(x, 683);
                this.Controls.Add(ell2);
                x += 99;
                System.Windows.Forms.Label ell3 = new System.Windows.Forms.Label();
                ell3.Name = "kazm1ell1";
                ell3.TextAlign = ContentAlignment.MiddleCenter;
                ell3.Text = kazamatakicsi[nev].Item3;
                ell3.BorderStyle = BorderStyle.FixedSingle;
                ell3.Size = new Size(85, 44);
                ell3.Location = new Point(x, 683);
                this.Controls.Add(ell3);
                x += 99;
                System.Windows.Forms.Label vez = new System.Windows.Forms.Label();
                vez.Name = "kazm1ell1";
                vez.TextAlign = ContentAlignment.MiddleCenter;
                vez.Text = "Vezér:" + Environment.NewLine +kazamatakicsi[nev].Item4;
                vez.BorderStyle = BorderStyle.FixedSingle;
                vez.Size = new Size(85, 44);
                vez.Location = new Point(x, 683);
                this.Controls.Add(vez);
                x += 99;
                System.Windows.Forms.Label jut1 = new System.Windows.Forms.Label();
                jut1.Name = "kazm0jut";
                jut1.TextAlign = ContentAlignment.MiddleCenter;
                jut1.Text = "Jutalom: " + kazamatakicsi[nev].Item5;
                jut1.BorderStyle = BorderStyle.FixedSingle;
                jut1.Size = new Size(85, 30);
                jut1.Location = new Point(x, 690);
                this.Controls.Add(jut1);
                x += 99;
                System.Windows.Forms.Label info = new System.Windows.Forms.Label();
                info.Name = "info2";
                info.TextAlign = ContentAlignment.MiddleCenter;
                info.Text = "Típus: kicsi";
                info.Size = new Size(85, 44);
                info.Location = new Point(x, 683);
                this.Controls.Add(info);
            }
            foreach (string nev in kazamatanagy.Keys)
            {
                x = 5;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = nev.ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 730);
                lbl.Click += (s, e) => Harc("nagy");
                this.Controls.Add(lbl);
                x = x + 99;

                System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                ell1.Name = "kazm2ell1";
                ell1.TextAlign = ContentAlignment.MiddleCenter;
                ell1.Text = kazamatanagy[nev].Item1;
                ell1.BorderStyle = BorderStyle.FixedSingle;
                ell1.Size = new Size(85, 44);
                ell1.Location = new Point(x, 733);
                this.Controls.Add(ell1);
                x += 99;
                System.Windows.Forms.Label ell2 = new System.Windows.Forms.Label();
                ell2.Name = "kazm2ell1";
                ell2.TextAlign = ContentAlignment.MiddleCenter;
                ell2.Text = kazamatanagy[nev].Item2;
                ell2.BorderStyle = BorderStyle.FixedSingle;
                ell2.Size = new Size(85, 44);
                ell2.Location = new Point(x, 733);
                this.Controls.Add(ell2);
                x += 99; 
                System.Windows.Forms.Label ell3 = new System.Windows.Forms.Label();
                ell3.Name = "kazm2ell3";
                ell3.TextAlign = ContentAlignment.MiddleCenter;
                ell3.Text = kazamatanagy[nev].Item3;
                ell3.BorderStyle = BorderStyle.FixedSingle;
                ell3.Size = new Size(85, 44);
                ell3.Location = new Point(x, 733);
                this.Controls.Add(ell3);
                x += 99;
                System.Windows.Forms.Label ell4 = new System.Windows.Forms.Label();
                ell4.Name = "kazm2ell4";
                ell4.TextAlign = ContentAlignment.MiddleCenter;
                ell4.Text = kazamatanagy[nev].Item4;
                ell4.BorderStyle = BorderStyle.FixedSingle;
                ell4.Size = new Size(85, 44);
                ell4.Location = new Point(x, 733);
                this.Controls.Add(ell4);
                x += 99;
                System.Windows.Forms.Label ell5 = new System.Windows.Forms.Label();
                ell5.Name = "kazm2ell5";
                ell5.TextAlign = ContentAlignment.MiddleCenter;
                ell5.Text = kazamatanagy[nev].Item5;
                ell5.BorderStyle = BorderStyle.FixedSingle;
                ell5.Size = new Size(85, 44);
                ell5.Location = new Point(x, 733);
                this.Controls.Add(ell5);
                x += 99;
                System.Windows.Forms.Label vez = new System.Windows.Forms.Label();
                vez.Name = "kazm2ell6";
                vez.TextAlign = ContentAlignment.MiddleCenter;
                vez.Text = "Vezér:" + Environment.NewLine+kazamatanagy[nev].Item6;
                vez.BorderStyle = BorderStyle.FixedSingle;
                vez.Size = new Size(85, 44);
                vez.Location = new Point(x, 733);
                this.Controls.Add(vez);
                x += 99;
                System.Windows.Forms.Label info = new System.Windows.Forms.Label();
                info.Name = "info3";
                info.TextAlign = ContentAlignment.MiddleCenter;
                info.Text = "Típus: nagy";
                info.Size = new Size(85, 44);
                info.Location = new Point(x, 733);
                this.Controls.Add(info);
                
            }
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

            int Xh = 230;
            int Xe = 300;
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

            while (true)
            {
                if (harcosok.Count == 0 && jharcos == "")
                {
                    veszitett = true;
                    break;
                }
                if (jellenfél == "") //kazamata kezd
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
                                ell1.Text = jellenfél + Environment.NewLine + jélete + "/" + jsebzése + Environment.NewLine + jtípuse;
                                ell1.BorderStyle = BorderStyle.FixedSingle;
                                ell1.Size = new System.Drawing.Size(70, 50);
                                ell1.Location = new Point(Xe, Y);
                                harctér.Controls.Add(ell1);
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
                                    ell1.Text = jellenfél + Environment.NewLine + jélete + "/" + jsebzése + Environment.NewLine + jtípuse;
                                    ell1.BorderStyle = BorderStyle.FixedSingle;
                                    ell1.Size = new System.Drawing.Size(70, 50);
                                    ell1.Location = new Point(Xe, Y);
                                    harctér.Controls.Add(ell1);
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
                        if (jéleth < 0)
                        {
                            jéleth = 0;
                        }
                        System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                        ell1.Name = "jellenfél";
                        ell1.TextAlign = ContentAlignment.MiddleCenter;
                        ell1.Text = jellenfél + Environment.NewLine + jélete + "/" + jsebzése + Environment.NewLine + jtípuse;
                        ell1.BorderStyle = BorderStyle.FixedSingle;
                        ell1.Size = new System.Drawing.Size(70, 50);
                        ell1.Location = new Point(Xe, Y);
                        harctér.Controls.Add(ell1);

                        System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                        harcos1.Name = "jharcos";
                        harcos1.TextAlign = ContentAlignment.MiddleCenter;
                        harcos1.Text = jharcos + Environment.NewLine + jéleth + "/" + jsebzésh + Environment.NewLine + jtípush;
                        harcos1.BorderStyle = BorderStyle.FixedSingle;
                        harcos1.Size = new System.Drawing.Size(70, 50);
                        harcos1.Location = new Point(Xh, Y);
                        harctér.Controls.Add(harcos1);
                    }
                    else if ((jtípuse == "Tűz" && jtípush == "Levegő") || (jtípuse == "Levegő" && jtípush == "Tűz") || (jtípuse == "Föld" && jtípush == "Víz") || (jtípuse == "Víz" && jtípush == "Föld"))
                    {
                        Console.WriteLine(jsebzése);
                        Console.WriteLine(jsebzése/2);
                        jéleth -= jsebzése / 2;
                        Console.WriteLine(jéleth);
                        if (jéleth < 0)
                        {
                            jéleth = 0;
                        }
                        System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                        ell1.Name = "jellenfél";
                        ell1.TextAlign = ContentAlignment.MiddleCenter;
                        ell1.Text = jellenfél + Environment.NewLine + jélete + "/" + jsebzése + Environment.NewLine + jtípuse;
                        ell1.BorderStyle = BorderStyle.FixedSingle;
                        ell1.Size = new System.Drawing.Size(70, 50);
                        ell1.Location = new Point(Xe, Y);
                        harctér.Controls.Add(ell1);

                        System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                        harcos1.Name = "jharcos";
                        harcos1.TextAlign = ContentAlignment.MiddleCenter;
                        harcos1.Text = jharcos + Environment.NewLine + jéleth + "/" + jsebzésh + Environment.NewLine + jtípush;
                        harcos1.BorderStyle = BorderStyle.FixedSingle;
                        harcos1.Size = new System.Drawing.Size(70, 50);
                        harcos1.Location = new Point(Xh, Y);
                        harctér.Controls.Add(harcos1);
                    }
                    else
                    {
                        jéleth -= jsebzése * 2;
                        if (jéleth < 0)
                        {
                            jéleth = 0;
                        }
                        System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                        ell1.Name = "jellenfél";
                        ell1.TextAlign = ContentAlignment.MiddleCenter;
                        ell1.Text = jellenfél + Environment.NewLine + jélete + "/" + jsebzése + Environment.NewLine + jtípuse;
                        ell1.BorderStyle = BorderStyle.FixedSingle;
                        ell1.Size = new System.Drawing.Size(70, 50);
                        ell1.Location = new Point(Xe, Y);
                        harctér.Controls.Add(ell1);

                        System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                        harcos1.Name = "jharcos";
                        harcos1.TextAlign = ContentAlignment.MiddleCenter;
                        harcos1.Text = jharcos + Environment.NewLine + jéleth + "/" + jsebzésh + Environment.NewLine + jtípush;
                        harcos1.BorderStyle = BorderStyle.FixedSingle;
                        harcos1.Size = new System.Drawing.Size(70, 50);
                        harcos1.Location = new Point(Xh, Y);
                        harctér.Controls.Add(harcos1);
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
                    if (harcosok.Count != 0)
                    {
                        jharcos = harcosok.Dequeue();
                        foreach (int i in kartyak.Keys)
                        {
                            if (kartyak[i].Item1 == jharcos)
                            {
                                jéleth = kartyak[i].Item3;
                                jsebzésh = kartyak[i].Item2;
                                jtípush = kartyak[i].Item4;
                                System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                                harcos1.Name = "jharcos";
                                harcos1.TextAlign = ContentAlignment.MiddleCenter;
                                harcos1.Text = jharcos + Environment.NewLine + jéleth + "/" + jsebzésh + Environment.NewLine + jtípush;
                                harcos1.BorderStyle = BorderStyle.FixedSingle;
                                harcos1.Size = new System.Drawing.Size(70, 50);
                                harcos1.Location = new Point(Xh, Y);
                                harctér.Controls.Add(harcos1);
                            }
                        }
                    }
                    else
                    {
                        veszitett = true;
                        break;
                    }
                }
                else //játékos támad, kivel, mennyi sebzés (típussal), kire, mennyi élet marad
                {
                    if (jtípuse == jtípush)
                    {
                        jélete -= jsebzésh;
                        if (jélete < 0)
                        {
                            jélete = 0;
                        }
                        System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                        ell1.Name = "jellenfél";
                        ell1.TextAlign = ContentAlignment.MiddleCenter;
                        ell1.Text = jellenfél + Environment.NewLine + jélete + "/" + jsebzése + Environment.NewLine + jtípuse;
                        ell1.BorderStyle = BorderStyle.FixedSingle;
                        ell1.Size = new System.Drawing.Size(70, 50);
                        ell1.Location = new Point(Xe, Y);
                        harctér.Controls.Add(ell1);

                        System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                        harcos1.Name = "jharcos";
                        harcos1.TextAlign = ContentAlignment.MiddleCenter;
                        harcos1.Text = jharcos + Environment.NewLine + jéleth + "/" + jsebzésh + Environment.NewLine + jtípush;
                        harcos1.BorderStyle = BorderStyle.FixedSingle;
                        harcos1.Size = new System.Drawing.Size(70, 50);
                        harcos1.Location = new Point(Xh, Y);
                        harctér.Controls.Add(harcos1);
                    }
                    else if ((jtípuse == "Tűz" && jtípush == "Levegő") || (jtípuse == "Levegő" && jtípush == "Tűz") || (jtípuse == "Föld" && jtípush == "Víz") || (jtípuse == "Víz" && jtípush == "Föld"))
                    {
                        jélete -= jsebzésh / 2;
                        if (jélete < 0)
                        {
                            jélete = 0;
                        }
                        System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                        ell1.Name = "jellenfél";
                        ell1.TextAlign = ContentAlignment.MiddleCenter;
                        ell1.Text = jellenfél + Environment.NewLine + jélete + "/" + jsebzése + Environment.NewLine + jtípuse;
                        ell1.BorderStyle = BorderStyle.FixedSingle;
                        ell1.Size = new System.Drawing.Size(70, 50);
                        ell1.Location = new Point(Xe, Y);
                        harctér.Controls.Add(ell1);

                        System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                        harcos1.Name = "jharcos";
                        harcos1.TextAlign = ContentAlignment.MiddleCenter;
                        harcos1.Text = jharcos + Environment.NewLine + jéleth + "/" + jsebzésh + Environment.NewLine + jtípush;
                        harcos1.BorderStyle = BorderStyle.FixedSingle;
                        harcos1.Size = new System.Drawing.Size(70, 50);
                        harcos1.Location = new Point(Xh, Y);
                        harctér.Controls.Add(harcos1);
                    }
                    else
                    {
                        jélete -= jsebzésh * 2;
                        if (jélete < 0)
                        {
                            jélete = 0;
                        }
                        System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                        ell1.Name = "jellenfél";
                        ell1.TextAlign = ContentAlignment.MiddleCenter;
                        ell1.Text = jellenfél + Environment.NewLine + jélete + "/" + jsebzése + Environment.NewLine + jtípuse;
                        ell1.BorderStyle = BorderStyle.FixedSingle;
                        ell1.Size = new System.Drawing.Size(70, 50);
                        ell1.Location = new Point(Xe, Y);
                        harctér.Controls.Add(ell1);

                        System.Windows.Forms.Label harcos1 = new System.Windows.Forms.Label();
                        harcos1.Name = "jharcos";
                        harcos1.TextAlign = ContentAlignment.MiddleCenter;
                        harcos1.Text = jharcos + Environment.NewLine + jéleth + "/" + jsebzésh + Environment.NewLine + jtípush;
                        harcos1.BorderStyle = BorderStyle.FixedSingle;
                        harcos1.Size = new System.Drawing.Size(70, 50);
                        harcos1.Location = new Point(Xh, Y);
                        harctér.Controls.Add(harcos1);
                    }
                    if (jélete <= 0)
                    {
                        jellenfél = "";
                        jélete = 0;
                        jsebzése = 0;
                        jtípuse = "";
                    }
                }
                Y += 70;
            }/*
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
            }*/




            //this.Controls.Remove(harctér);

        }

        private void PakliClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string last = button.Name.Last<char>().ToString();
            string asd = playercards[Convert.ToInt32(last)].Item1;
            Pakli.Remove(asd);
            foreach (var btn in this.Controls.OfType<Button>())
            {
                if (btn.Location.Y == button.Location.Y && btn.Location.X > button.Location.X)
                {
                    btn.Location = new Point(btn.Location.X-99, btn.Location.Y);
                }
                else if (btn.Name == "gyujtemeny" + last)
                {
                    btn.Enabled = true;
                }
            }
            paklix -= 99;
            Controls.Remove(button);
            button.Dispose();
        }

        private void ÚjPakli_Click(object sender, EventArgs e)
        {
            paklix = 0;
            Pakli.Clear();
            foreach (Control ctrl in Controls)
            {
                if (ctrl is Button btn)
                {
                    if (btn.Name.StartsWith("gyujtemeny"))
                    {
                        btn.Enabled = true;
                    }
                    else if (btn.Name == "ÚjPakli")
                    {
                        btn.Enabled = false;
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
        }

        private void KészPakli_Click(object sender, EventArgs e)
        {
            int felekerint;
            if (playercards.Count % 2 == 0)
            {
                felekerint = playercards.Count / 2;
            }
            else
            {
                felekerint = (playercards.Count + 1) / 2;
            }

            if (Pakli.Count <= felekerint)
            { 
                foreach (Control ctrl in Controls)
                {
                    if (ctrl is Button btn) 
                    {
                        if (btn.Name.StartsWith("gyujtemeny") || btn.Name.StartsWith("paklibtn"))
                        {
                            btn.Enabled = false;
                        }
                        else if (btn.Name == "ÚjPakli")
                        {
                            btn.Enabled = true;
                        }
                        else if (btn.Name == "KészPakli")
                        {
                            btn.Enabled = false;
                        }
                    }
                }
            }
        }
    }
}
