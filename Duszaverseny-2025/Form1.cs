using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Duszaverseny_2025
{ 
    //dmg: 2-100, hp: 1-100, kartya neve: max 14 karakter --> vezerkartya neve max 16 karakter
    /*TODO
    Alaphelyzetet tovabb vinni jatekmesterkent --> jatekmester is valaszthat alaphelyzetekbol (beolvasas kell hozza)
    Kartya kitorolhetove tehetese --> a belole szarmaztatott vezer, a kazamatak ahol ellenfelkent szerepelt szinten torlodnek*/

    public partial class Form1 : Form
    {
        Dictionary<string, string> tipusok = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "tuz", "Tűz" },
            { "viz", "Víz" },
            { "fold", "Föld" },
            { "levego", "Levegő" }
        };

        string egyszerukazamatanameinput;
        string konnyukazamatanameinput;
        string nehezkazamatanameinput;
        string megakazamatanameinput;
        string savefilenameinput;

        string selected = string.Empty;
        int ujkartyasorszam = 0;
        int gyujtemenysorszam = 0;
        int ujvezersorszam = 0;
        int ujkiskazamatasorszam = 0;
        int ujegyszerukazamatasorszam = 0;
        int ujnagykazamatasorszam = 0;
        int ujmegakazamatasorszam = 0;
        int vezerrefejlesztessorszam;

        Dictionary<int, (string, int, int, string)> kartyak = new Dictionary<int, (string, int, int, string)>(); //név, sebzés, életerő, típus
        Dictionary<int, (string, int, int, string)> vezerkartyak = new Dictionary<int, (string, int, int, string)>();
        Dictionary<int, (string, int, int, string)> playercards = new Dictionary<int, (string, int, int, string)>(); //properies of the player's cards
        List<string> Pakli = new List<string>();

        Dictionary<int, (string, string, string)> kazamataegyszeru = new Dictionary<int, (string, string, string)>(); //név, ellenfél, jutalom
        Dictionary<int, (string, string, string, string, string, string)> kazamatakicsi = new Dictionary<int, (string, string, string, string, string, string)>(); //név, ellenfél*3, vezér, jutalom
        Dictionary<int, (string, string, string, string, string, string, string)> kazamatanagy = new Dictionary<int, (string, string, string, string, string, string, string)>(); //név, ellenfél*5, vezér
        Dictionary<int, (string, string, string, string, string, string, string, string, string, string)> kazamatamega = new Dictionary<int, (string, string, string, string, string, string, string, string, string, string)>(); //név, ellenfél*5, vezér*3, jutalom

        Panel menu = new Panel();
        Panel playerscreen = new Panel();
        Panel savestart = new Panel();
        Panel mester = new Panel();
        Panel ujkartya = new Panel();
        Panel ujvezer = new Panel();
        Panel sebzeseletero = new Panel();
        Panel ujkazamata = new Panel();
        Panel gyujtemeny = new Panel();
        Panel egyszeruk = new Panel();
        Panel konnyuk = new Panel();
        Panel nehezk = new Panel();
        Panel megak = new Panel();

        public Form1()
        {
            InitializeComponent();
            menu.Hide();
            playerscreen.Hide();
            savestart.Hide();
            mester.Hide();
            ujkartya.Hide();
            ujvezer.Hide();
            sebzeseletero.Hide();
            ujkazamata.Hide();
            gyujtemeny.Hide();
            egyszeruk.Hide();
            konnyuk.Hide();
            nehezk.Hide();
            megak.Hide();
            menuu();
        }
        
        private void button(string text, string name, int sx, int sy, int locx, int locy, int fontsize, Panel panel, EventHandler clickevent)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Name = name;
            btn.Size = new Size(sx, sy);
            Point btnloc = new Point(locx, locy);
            btn.Location = btnloc;
            btn.Font = new Font("Microsoft Sans Seriff", fontsize);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Click += clickevent;
            panel.Controls.Add(btn);
        }

        private void textbox(string name, int sx, int sy, int locx, int locy, Panel panel)
        {
            TextBox textbox = new TextBox();
            textbox.Name = name;
            textbox.Size = new Size(sx, sy);
            Point textboxlox = new Point(locx, locy);
            textbox.Location = textboxlox;
            panel.Controls.Add(textbox);
        }

        private void label(string text, string name, int sx, int sy, int locx, int locy, int fontsize, Panel panel)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Text = text;
            label.Size = new Size(sx, sy);
            Point labelloc = new Point(locx, locy);
            label.Location = labelloc;
            label.Font = new Font("Microsoft Sans Seriff", fontsize);
            label.TextAlign = ContentAlignment.MiddleCenter;
            panel.Controls.Add(label);
        }

        private void menuu()
        {
            menu.Controls.Clear();
            menu.Show();
            menu.Dock = DockStyle.Fill;
            this.Controls.Add(menu);
            menu.BringToFront();

            label("Damareen", "title", 400, 200, 350, 100, 50, menu);
            button("Játékos", "jatekosgomb", 200, 200, 150, 300, 20, menu, MainScreenGombok);
            button("Játékmester", "jatekmestergomb", 200, 200, 700, 300, 20, menu, MainScreenGombok);
        }

        private void MainScreenGombok(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "jatekosgomb") //jatekos menube belepes
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }
                savestart.Controls.Clear();
                savestart.Show();
                savestart.Dock = DockStyle.Fill;
                this.Controls.Add(savestart);
                menu.Controls.Clear();
                menu.SendToBack();
                savestart.BringToFront();

                button("Mentések", "loadsavefile", 200, 200, 150, 300, 20, savestart, LoadFile);
                button("Alaphelyzetek", "loaddefault", 200, 200, 700, 300, 20, savestart, LoadFile);
            }
            else if (name == "jatekmestergomb") //jatekmester panelre belepes
            {
                kartyak.Clear();
                playercards.Clear();
                vezerkartyak.Clear();
                kazamataegyszeru.Clear();
                kazamatakicsi.Clear();
                kazamatanagy.Clear();
                kazamatamega.Clear();

                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }
                mester.Controls.Clear();
                mester.Show();
                mester.Dock = DockStyle.Fill;
                this.Controls.Add(mester);
                mester.BringToFront();

                button("Kész", "done", 100, 100, 200, 400, 20, mester, SaveNewfile);
                button("Kártya", "ujkartya", 200, 100, 150, 200, 20, mester, Újdolgok);
                button("Vezér", "ujvezer", 200, 100, 700, 200, 20, mester, Újdolgok);
                button("Kazamata", "ujkazamata", 200, 100, 150, 600, 20, mester, Újdolgok);
                button("Gyűjtemény", "gyujtemeny", 200, 100, 700, 600, 20, mester, Újdolgok);
                label("Név:", "name", 200, 50, 400, 300, 20, mester);
                textbox("ujsavename", 200, 50, 400, 400, mester);
            }
        }

        private void Újdolgok(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "ujkartya") //kartya hozzadasa
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }
                ujkartya.Controls.Clear();
                ujkartya.Show();
                ujkartya.Dock = DockStyle.Fill;
                this.Controls.Add(ujkartya);
                ujkartya.BringToFront();

                label("Kártya neve:", "ujkname", 300, 50, 150, 75, 20, ujkartya);
                textbox("ujknameinput", 300, 50, 150, 150, ujkartya);
                label("Kártya sebzése:", "ujkdmg", 300, 50, 700, 75, 20, ujkartya);
                textbox("ujkdmginput", 300, 50, 700, 150, ujkartya);
                label("Kártya életereje:", "ujkhp", 300, 50, 150, 575, 20, ujkartya);
                textbox("ujkhpinput", 300, 50, 150, 650, ujkartya);
                label("Kártya típusa:", "ujktype", 300, 50, 700, 375, 20, ujkartya);
                button("Tűz", "tuz", 100, 100, 600, 450, 20, ujkartya, ColortheTypes);
                button("Víz", "viz", 100, 100, 800, 450, 20, ujkartya, ColortheTypes);
                button("Levegő", "levego", 100, 100, 600, 650, 20, ujkartya, ColortheTypes);
                button("Föld", "fold", 100, 100, 800, 650, 20, ujkartya, ColortheTypes);
                button("Hozzáadás", "registercard", 100, 100, 350, 350, 20, ujkartya, Újdolgok);
                button("Vissza", "visszamester", 100, 100, 50, 400, 20, ujkartya, button_Click);
            }
            else if (name == "ujvezer") //kartya vezerre fejlesztese
            {
                if ((kartyak.Count - vezerkartyak.Count) < 1) { }
                else
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is Panel panel && panel.Visible)
                        {
                            panel.Hide();
                        }
                    }

                    ujvezer.Controls.Clear();
                    ujvezer.Show();
                    ujvezer.Dock = DockStyle.Fill;
                    this.Controls.Add(ujvezer);
                    ujvezer.BringToFront();

                    button("Vissza", "visszamester", 150, 100, 0, 0, 20, ujvezer, button_Click);

                    int x = 150;
                    int y = 0;
                    bool vanev = false;
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        if (vezerkartyak.Count > 0)
                        {
                            for (int j = 0; j < vezerkartyak.Count; j++)
                            {
                                if (vezerkartyak[j].Item1 == "v " + kartyak[i].Item1) { vanev = true; }
                            }
                            if (vanev == false)
                            {
                                button(kartyak[i].Item1, kartyak[i].Item1 + "ujvezer", 75, 100, x, y, 10, ujvezer, button_Click);
                                x += 75;
                                if (x > 975)
                                {
                                    y += 100;
                                    x = 0;
                                }
                            }
                            else { vanev = false; }
                        }
                        else
                        {
                            button(kartyak[i].Item1, kartyak[i].Item1 + "ujvezer", 75, 100, x, y, 10, ujvezer, button_Click);
                            x += 75;
                            if (x > 975)
                            {
                                y += 100;
                                x = 0;
                            }
                        }
                    }
                }
            }
            else if (name == "ujkazamata") //kazamata hozzaadas :skull:
            {
                if (kartyak.Count < 1) { }
                else
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is Panel panel && panel.Visible)
                        {
                            panel.Hide();
                        }
                    }

                    ujkazamata.Controls.Clear();
                    ujkazamata.Show();
                    ujkazamata.Dock = DockStyle.Fill;
                    this.Controls.Add(ujkazamata);
                    ujkazamata.BringToFront();

                    button("Vissza", "visszamester", 100, 100, 400, 400, 20, ujkazamata, button_Click);
                    button("Egyszerű", "egyszeruk", 200, 200, 0, 0, 20, ujkazamata, kazamaták);
                    button("Könnyű", "konnyuk", 200, 200, 200, 0, 20, ujkazamata, kazamaták);
                    button("Nehéz", "nehezk", 200, 200, 0, 200, 20, ujkazamata, kazamaták);
                    button("Mega", "megak", 200, 200, 200, 200, 20, ujkazamata, kazamaták);
                }
            }
            else if (name == "gyujtemeny") //gyujtemeny kivalasztasa
            {
                if (kartyak.Count < 1) { }
                else
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is Panel panel && panel.Visible)
                        {
                            panel.Hide();
                        }
                    }

                    gyujtemeny.Controls.Clear();
                    gyujtemeny.Show();
                    gyujtemeny.Dock = DockStyle.Fill;
                    this.Controls.Add(gyujtemeny);
                    gyujtemeny.BringToFront();

                    button("Vissza", "visszamester", 150, 100, 0, 0, 20, gyujtemeny, button_Click);
                    button("Kész", "gyujtemenydone", 150, 100, 150, 0, 20, gyujtemeny, Újdolgok);

                    int x = 300;
                    int y = 0;
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        button(kartyak[i].Item1, kartyak[i].Item1 + "gyujtemenybee", 75, 100, x, y, 10, gyujtemeny, button_Click);

                        for (int j = 0; j < kartyak.Count; j++)
                        {
                            Button button = gyujtemeny.Controls.OfType<Button>()
                              .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "gyujtemenybee");
                            if (button != null && playercards.Count > 0)
                            {
                                for (int k = 0; k < playercards.Count; k++)
                                {
                                    if (playercards[k].Item1 == kartyak[i].Item1)
                                    {
                                        button.BackColor = Color.Yellow;
                                    }
                                }
                            }
                        }
                        x += 75;
                        if (x > 975)
                        {
                            y += 100;
                            x = 0;
                        }
                    }
                }
            }
            else if (name == "gyujtemenydone") //gyujtemeny kivalasztva
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }
                mester.Show();
                playercards.Clear();
                gyujtemenysorszam = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    Button button = gyujtemeny.Controls.OfType<Button>()
                      .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "gyujtemenybee");
                    if (button != null)
                    {
                        if (button.BackColor == Color.Yellow)
                        {
                            playercards.Add(gyujtemenysorszam, (kartyak[i].Item1, kartyak[i].Item2, kartyak[i].Item3, kartyak[i].Item4));
                            gyujtemenysorszam++;
                        }
                    }
                }
            }
            else if (name == "registercard") //uj kartya hozzadasa
            {
                string ujkname = string.Empty;
                int ujkdmg = 0;
                int ujkhp = 0;
                TextBox txtb = ujkartya.Controls.OfType<TextBox>()
                      .FirstOrDefault(t => t.Name == "ujknameinput");
                if (txtb != null)
                {
                    ujkname = txtb.Text;
                }
                TextBox txtbb = ujkartya.Controls.OfType<TextBox>()
                      .FirstOrDefault(t => t.Name == "ujkdmginput");
                if (txtbb != null)
                {
                    if (int.TryParse(txtbb.Text, out ujkdmg))
                    {
                        ujkdmg = Convert.ToInt32(txtbb.Text);
                    }
                }
                TextBox txtbbb = ujkartya.Controls.OfType<TextBox>()
                      .FirstOrDefault(t => t.Name == "ujkhpinput");
                if (txtbbb != null)
                {
                    if (int.TryParse(txtbbb.Text, out ujkhp))
                    {
                        ujkhp = Convert.ToInt32(txtbbb.Text);
                    }
                }

                if (ujkname != string.Empty && ujkdmg != 0 && ujkhp != 0 && ujkname.Length <= 14 && ujkdmg >= 2 && ujkdmg <= 100 && ujkhp >= 1 && ujkhp <= 100) //csak akkor engedi letrehozni ha van megadva nev, ami max 14 karakter vezerre fejlesztes miatt, mert igy a vezer neve max 16 karakter lesz, tipus, dmg-és hp <= 100
                {
                    if (kartyak.Count > 0)
                    {
                        bool vane = false;
                        for (int i = 0; i < kartyak.Count; i++)
                        {
                            if (ujkname == kartyak[i].Item1) { vane = true; }  //van-e mar ilyen nevu kartya, ha van nem engedi letrehozni                             
                        }
                        if (vane == false)
                        {
                            kartyak.Add(ujkartyasorszam, (ujkname, ujkdmg, ujkhp, selected));
                            ujkartyasorszam++;

                            ujkartya.Hide();
                            mester.Show();
                        }
                    }
                    else
                    {
                        kartyak.Add(ujkartyasorszam, (ujkname, ujkdmg, ujkhp, selected));
                        ujkartyasorszam++;

                        ujkartya.Hide();
                        mester.Show();
                    }
                }
            }
        }

        private void kazamaták(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "egyszeruk" && kartyak.Count > 0)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }

                egyszeruk.Controls.Clear();
                egyszeruk.Show();
                egyszeruk.Dock = DockStyle.Fill;
                this.Controls.Add(egyszeruk);
                egyszeruk.BringToFront();

                button("Vissza", "visszakazamata", 150, 100, 0, 0, 20, egyszeruk, button_Click);
                button("Kész", "kazamatadone", 150, 100, 150, 0, 20, egyszeruk, kazamaták);
                textbox("egyszeruknameinput", 300, 100, 300, 0, egyszeruk);
                button("Sebzés", "jutalomse", 150, 100, 600, 0, 20, egyszeruk, button_Click);

                int x = 750;
                int y = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    button(kartyak[i].Item1, kartyak[i].Item1 + "egyszerukazamataba", 75, 100, x, y, 10, egyszeruk, button_Click);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
                for (int i = 0; i < vezerkartyak.Count; i++)
                {
                    button(vezerkartyak[i].Item1, vezerkartyak[i].Item1 + "egyszerukazamataba", 75, 100, x, y, 10, egyszeruk, button_Click);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
            }
            else if (name == "konnyuk" && kartyak.Count > 3 && vezerkartyak.Count > 0)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }

                konnyuk.Controls.Clear();
                konnyuk.Show();
                konnyuk.Dock = DockStyle.Fill;
                this.Controls.Add(konnyuk);
                konnyuk.BringToFront();

                button("Vissza", "visszakazamata", 150, 100, 0, 0, 20, konnyuk, button_Click);
                button("Kész", "kazamatadone", 150, 100, 150, 0, 20, konnyuk, kazamaták);
                textbox("konnyuknameinput", 300, 100, 300, 0, konnyuk);
                button("Sebzés", "jutalomse", 150, 100, 600, 0, 20, konnyuk, button_Click);

                int x = 750;
                int y = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    button(kartyak[i].Item1, kartyak[i].Item1 + "konnyukazamataba", 75, 100, x, y, 10, konnyuk, button_Click);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
                for (int i = 0; i < vezerkartyak.Count; i++)
                {
                    button(vezerkartyak[i].Item1, vezerkartyak[i].Item1 + "konnyukazamataba", 75, 100, x, y, 10, konnyuk, button_Click);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
            }
            else if (name == "nehezk" && kartyak.Count > 5 && vezerkartyak.Count > 0)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }

                nehezk.Controls.Clear();
                nehezk.Show();
                nehezk.Dock = DockStyle.Fill;
                this.Controls.Add(nehezk);
                nehezk.BringToFront();

                button("Vissza", "visszakazamata", 150, 100, 0, 0, 20, nehezk, button_Click);
                button("Kész", "kazamatadone", 150, 100, 150, 0, 20, nehezk, button_Click);
                textbox("nehezknameinput", 300, 100, 300, 0, nehezk);

                int x = 600;
                int y = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    button(kartyak[i].Item1, kartyak[i].Item1 + "nehezkazamataba", 75, 100, x, y, 10, nehezk, button_Click);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
                for (int i = 0; i < vezerkartyak.Count; i++)
                {
                    button(vezerkartyak[i].Item1, vezerkartyak[i].Item1 + "nehezkazamataba", 75, 100, x, y, 10, nehezk, button_Click);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
            }
            else if (name == "megak" && kartyak.Count > 7 && vezerkartyak.Count > 2)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }

                megak.Controls.Clear();
                megak.Show();
                megak.Dock = DockStyle.Fill;
                this.Controls.Add(megak);
                megak.BringToFront();

                button("Vissza", "visszakazamata", 150, 100, 0, 0, 20, megak, button_Click);
                button("Kész", "kazamatadone", 150, 100, 150, 0, 20, megak, kazamaták);
                textbox("megaknameinput", 300, 100, 300, 0, megak);

                int x = 600;
                int y = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    button(kartyak[i].Item1, kartyak[i].Item1 + "megakazamataba", 75, 100, x, y, 10, megak, button_Click);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
                for (int i = 0; i < vezerkartyak.Count; i++)
                {
                    button(vezerkartyak[i].Item1, vezerkartyak[i].Item1 + "megakazamataba", 75, 100, x, y, 10, megak, button_Click);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
            }
            else if (name == "kazamatadone")
            {
                if (egyszeruk.Visible) //egyszeru kazamatak
                {
                    List<string> selectedkartyak = new List<string>();
                    selectedkartyak.Clear();
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        Button button = egyszeruk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "egyszerukazamataba");
                        if (button != null)
                        {
                            if (button.BackColor == Color.Yellow)
                            {
                                selectedkartyak.Add(kartyak[i].Item1);
                            }
                        }
                    }
                    List<string> selectedvezer = new List<string>();
                    selectedvezer.Clear();
                    if (vezerkartyak.Count > 0)
                    {
                        for (int i = 0; i < vezerkartyak.Count; i++)
                        {
                            Button button = egyszeruk.Controls.OfType<Button>()
                                .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "egyszerukazamataba");
                            if (button != null)
                            {
                                if (button.BackColor == Color.Yellow)
                                {
                                    selectedvezer.Add(vezerkartyak[i].Item1);
                                }
                            }
                        }
                    }
                    TextBox txtb = egyszeruk.Controls.OfType<TextBox>()
                            .FirstOrDefault(b => b.Name == "egyszeruknameinput");
                    if (txtb != null)
                    {
                        egyszerukazamatanameinput = txtb.Text;
                    }
                    string jutalom = string.Empty;
                    Button button1 = egyszeruk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == "jutalomse");
                    if (button1 != null)
                    {
                        jutalom = button1.Text;
                    }
                    if (selectedvezer.Count == 0 && selectedkartyak.Count == 1 && egyszerukazamatanameinput != "" && egyszerukazamatanameinput != null) //0 vezer, 1 kartya, van megadva nev
                    {
                        if (kazamataegyszeru.Count > 0)
                        {
                            bool vane = false;
                            for (int i = 0; i < kazamataegyszeru.Count; i++)
                            {
                                if (kazamataegyszeru[i].Item1 == txtb.Name)
                                {
                                    vane = true;
                                }
                            }
                            if (vane == false)
                            {
                                kazamataegyszeru.Add(ujegyszerukazamatasorszam, (egyszerukazamatanameinput, selectedkartyak[0], jutalom));
                                ujegyszerukazamatasorszam++;
                                egyszeruk.Hide();
                                ujkazamata.Show();
                            }
                        }
                        else
                        {
                            kazamataegyszeru.Add(ujegyszerukazamatasorszam, (egyszerukazamatanameinput, selectedkartyak[0], jutalom));
                            ujegyszerukazamatasorszam++;
                            egyszeruk.Hide();
                            ujkazamata.Show();
                        }
                    }
                }
                else if (konnyuk.Visible) //konnyu kazamatak
                {
                    List<string> selectedkartyak = new List<string>();
                    selectedkartyak.Clear();
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        Button button = konnyuk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "konnyukazamataba");
                        if (button != null)
                        {
                            if (button.BackColor == Color.Yellow)
                            {
                                selectedkartyak.Add(kartyak[i].Item1);
                            }
                        }
                    }
                    List<string> selectedvezer = new List<string>();
                    selectedvezer.Clear();
                    if (vezerkartyak.Count > 0)
                    {
                        for (int i = 0; i < vezerkartyak.Count; i++)
                        {
                            Button button = konnyuk.Controls.OfType<Button>()
                                .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "konnyukazamataba");
                            if (button != null)
                            {
                                if (button.BackColor == Color.Yellow)
                                {
                                    selectedvezer.Add(vezerkartyak[i].Item1);
                                }
                            }
                        }
                    }
                    TextBox txtb = konnyuk.Controls.OfType<TextBox>()
                            .FirstOrDefault(b => b.Name == "konnyuknameinput");
                    if (txtb != null)
                    {
                        konnyukazamatanameinput = txtb.Text;
                    }
                    string jutalom = string.Empty;
                    Button button1 = konnyuk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == "jutalomse");
                    if (button1 != null)
                    {
                        jutalom = button1.Text;
                    }
                    if (selectedvezer.Count == 1 && selectedkartyak.Count == 3 && konnyukazamatanameinput != "" && konnyukazamatanameinput != null) //1 vezer, 3 kartya, van megadva nev
                    {
                        if (kazamatakicsi.Count > 0)
                        {
                            bool vane = false;
                            for (int i = 0; i < kazamatakicsi.Count; i++)
                            {
                                if (kazamatakicsi[i].Item1 == txtb.Name)
                                {
                                    vane = true;
                                }
                            }
                            if (vane == false)
                            {
                                kazamatakicsi.Add(ujkiskazamatasorszam, (egyszerukazamatanameinput, selectedkartyak[0], selectedkartyak[1], selectedkartyak[2], selectedvezer[0], jutalom));
                                ujkiskazamatasorszam++;
                                konnyuk.Hide();
                                ujkazamata.Show();
                            }
                        }
                        else
                        {
                            kazamatakicsi.Add(ujkiskazamatasorszam, (egyszerukazamatanameinput, selectedkartyak[0], selectedkartyak[1], selectedkartyak[2], selectedvezer[0], jutalom));
                            ujkiskazamatasorszam++;
                            konnyuk.Hide();
                            ujkazamata.Show();
                        }
                    }
                }
                else if (nehezk.Visible) //nehez kazamatak
                {
                    List<string> selectedkartyak = new List<string>();
                    selectedkartyak.Clear();
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        Button button = nehezk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "nehezkazamataba");
                        if (button != null)
                        {
                            if (button.BackColor == Color.Yellow)
                            {
                                selectedkartyak.Add(kartyak[i].Item1);
                            }
                        }
                    }
                    List<string> selectedvezer = new List<string>();
                    selectedvezer.Clear();
                    if (vezerkartyak.Count > 0)
                    {
                        for (int i = 0; i < vezerkartyak.Count; i++)
                        {
                            Button button = nehezk.Controls.OfType<Button>()
                                .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "nehezkazamataba");
                            if (button != null)
                            {
                                if (button.BackColor == Color.Yellow)
                                {
                                    selectedvezer.Add(vezerkartyak[i].Item1);
                                }
                            }
                        }
                    }
                    TextBox txtb = nehezk.Controls.OfType<TextBox>()
                            .FirstOrDefault(b => b.Name == "nehezknameinput");
                    if (txtb != null)
                    {
                        nehezkazamatanameinput = txtb.Text;
                    }
                    if (selectedvezer.Count == 1 && selectedkartyak.Count == 5 && nehezkazamatanameinput != "" && nehezkazamatanameinput != null) //1 vezer, 5 kartya, van megadva nev
                    {
                        if (kazamatanagy.Count > 0)
                        {
                            bool vane = false; //vane mar ilyen nevu
                            for (int i = 0; i < kazamatanagy.Count; i++)
                            {
                                if (kazamatanagy[i].Item1 == txtb.Name)
                                {
                                    vane = true;
                                }
                            }
                            if (vane == false)
                            {
                                kazamatanagy.Add(ujnagykazamatasorszam, (nehezkazamatanameinput, selectedkartyak[0], selectedkartyak[1], selectedkartyak[2], selectedkartyak[3], selectedkartyak[4], selectedvezer[0]));
                                ujnagykazamatasorszam++;
                                nehezk.Hide();
                                ujkazamata.Show();
                            }
                        }
                        else
                        {
                            kazamatanagy.Add(ujnagykazamatasorszam, (nehezkazamatanameinput, selectedkartyak[0], selectedkartyak[1], selectedkartyak[2], selectedkartyak[3], selectedkartyak[4], selectedvezer[0]));
                            ujnagykazamatasorszam++;
                            nehezk.Hide();
                            ujkazamata.Show();
                        }
                    }
                }
                else if (megak.Visible) //mega kazamatak
                {
                    List<string> selectedkartyak = new List<string>();
                    selectedkartyak.Clear();
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        Button button = megak.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "megakazamataba");
                        if (button != null)
                        {
                            if (button.BackColor == Color.Yellow)
                            {
                                selectedkartyak.Add(kartyak[i].Item1);
                            }
                        }
                    }
                    List<string> selectedvezer = new List<string>();
                    selectedvezer.Clear();
                    if (vezerkartyak.Count > 0)
                    {
                        for (int i = 0; i < vezerkartyak.Count; i++)
                        {
                            Button button = megak.Controls.OfType<Button>()
                                .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "megakazamataba");
                            if (button != null)
                            {
                                if (button.BackColor == Color.Yellow)
                                {
                                    selectedvezer.Add(vezerkartyak[i].Item1);
                                }
                            }
                        }
                    }
                    TextBox txtb = megak.Controls.OfType<TextBox>()
                            .FirstOrDefault(b => b.Name == "megaknameinput");
                    if (txtb != null)
                    {
                        megakazamatanameinput = txtb.Text;
                    }
                    if (selectedvezer.Count == 3 && selectedkartyak.Count == 5 && megakazamatanameinput != "" && megakazamatanameinput != null) //3 vezer, 5 kartya, van megadva nev
                    {
                        if (kazamatamega.Count > 0)
                        {
                            bool vane = false; //vane mar ilyen nevu
                            for (int i = 0; i < kazamatamega.Count; i++)
                            {
                                if (kazamatanagy[i].Item1 == txtb.Name)
                                {
                                    vane = true;
                                }
                            }
                            if (vane == false)
                            {
                                kazamatamega.Add(ujmegakazamatasorszam, (megakazamatanameinput, selectedkartyak[0], selectedkartyak[1], selectedkartyak[2], selectedkartyak[3], selectedkartyak[4], selectedvezer[0], selectedvezer[1], selectedvezer[2], "jutalom"));
                                ujmegakazamatasorszam++;
                                megak.Hide();
                                ujkazamata.Show();
                            }
                        }
                        else
                        {
                            kazamatamega.Add(ujmegakazamatasorszam, (megakazamatanameinput, selectedkartyak[0], selectedkartyak[1], selectedkartyak[2], selectedkartyak[3], selectedkartyak[4], selectedvezer[0], selectedvezer[1], selectedvezer[2], "jutalom"));
                            ujmegakazamatasorszam++;
                            megak.Hide();
                            ujkazamata.Show();
                        }
                    }
                }
            }
        }

        private void ColortheTypes(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "tuz" || name == "viz" || name == "levego" || name == "fold") //tipusgombok szinezese
            {
                List<string> tipus = new List<string>();
                tipus.Add("tuz");
                tipus.Add("viz");
                tipus.Add("levego");
                tipus.Add("fold");

                for (int i = 0; i < 4; i++)
                {
                    Button btnn = ujkartya.Controls.OfType<Button>()
                      .FirstOrDefault(b => b.Name == tipus[i]);
                    if (btnn != null)
                    {
                        btnn.BackColor = Color.White;
                    }
                    btn.BackColor = Color.LightBlue;
                    selected = btn.Name;
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                string name = btn.Name;

                if (name == "")
                {

                }
                else if (name == "visszamester") //vissza a jatekmester menure
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is Panel panel && panel.Visible)
                        {
                            panel.Hide();
                        }
                    }
                    mester.Show();
                }
                else if (name == "visszakazamata")
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is Panel panel && panel.Visible)
                        {
                            panel.Hide();
                        }
                    }
                    ujkazamata.Show();
                }
                else if (name == "jutalomse")
                {
                    Button button = egyszeruk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == "jutalomse");
                    if (button != null)
                    {
                        if (button.Text == "Sebzés")
                        {
                            button.Text = "Életerő";
                        }
                        else
                        {
                            button.Text = "Sebzés";
                        }
                    }
                    Button buttonn = konnyuk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == "jutalomse");
                    if (buttonn != null)
                    {
                        if (buttonn.Text == "Sebzés")
                        {
                            buttonn.Text = "Életerő";
                        }
                        else
                        {
                            buttonn.Text = "Sebzés";
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        if (name == kartyak[i].Item1 + "ujvezer") //vezernek valaszthato kartyak
                        {
                            foreach (Control ctrl in this.Controls)
                            {
                                if (ctrl is Panel panel && panel.Visible)
                                {
                                    panel.Hide();
                                }
                            }

                            vezerrefejlesztessorszam = i;

                            sebzeseletero.Controls.Clear();
                            sebzeseletero.Show();
                            sebzeseletero.Dock = DockStyle.Fill;
                            this.Controls.Add(sebzeseletero);
                            sebzeseletero.BringToFront();

                            button("Életerő", "hp", 200, 200, 350, 400, 20, sebzeseletero, VezerBuffok);
                            button("Sebzés", "dmg", 200, 200, 700, 400, 20, sebzeseletero, VezerBuffok);
                        }

                        if (name == kartyak[i].Item1 + "gyujtemenybee") //gyujtemenybe valasztott / kivett kartya
                        {
                            Button button = gyujtemeny.Controls.OfType<Button>()
                          .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "gyujtemenybee");
                            if (button != null)
                            {
                                if (button.BackColor == Color.Yellow)
                                {
                                    button.BackColor = Color.White;
                                }
                                else { button.BackColor = Color.Yellow; }
                            }
                        }

                        if (vezerkartyak.Count > i)
                        {
                            if (name == vezerkartyak[i].Item1 + "egyszerukazamataba")
                            {
                                Button button = egyszeruk.Controls.OfType<Button>()
                              .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "egyszerukazamataba");
                                if (button != null)
                                {
                                    string alap = string.Empty;
                                    for (int j = 0; j < kartyak.Count; j ++)
                                    {
                                        if ("v "+kartyak[j].Item1 == vezerkartyak[i].Item1)
                                        {
                                            alap = kartyak[j].Item1;
                                        }
                                    }

                                    Button butn = egyszeruk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == alap + "egyszerukazamataba");
                                    if (butn != null)
                                    {
                                        if (butn.BackColor == Color.Yellow)
                                        {

                                        }
                                        else
                                        {
                                            if (button.BackColor == Color.Yellow)
                                            {
                                                button.BackColor = Color.White;
                                            }
                                            else { button.BackColor = Color.Yellow; }
                                        }
                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.Yellow)
                                        {
                                            button.BackColor = Color.White;
                                        }
                                        else { button.BackColor = Color.Yellow; }
                                    }
                                }
                            }

                            if (name == vezerkartyak[i].Item1 + "konnyukazamataba")
                            {
                                Button button = konnyuk.Controls.OfType<Button>()
                              .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "konnyukazamataba");
                                if (button != null)
                                {
                                    string alap = string.Empty;
                                    for (int j = 0; j < kartyak.Count; j++)
                                    {
                                        if ("v " + kartyak[j].Item1 == vezerkartyak[i].Item1)
                                        {
                                            alap = kartyak[j].Item1;
                                        }
                                    }

                                    Button butn = konnyuk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == alap + "konnyukazamataba");
                                    if (butn != null)
                                    {
                                        if (butn.BackColor == Color.Yellow)
                                        {

                                        }
                                        else
                                        {
                                            if (button.BackColor == Color.Yellow)
                                            {
                                                button.BackColor = Color.White;
                                            }
                                            else { button.BackColor = Color.Yellow; }
                                        }
                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.Yellow)
                                        {
                                            button.BackColor = Color.White;
                                        }
                                        else { button.BackColor = Color.Yellow; }
                                    }
                                }
                            }

                            if (name == vezerkartyak[i].Item1 + "nehezkazamataba")
                            {
                                Button button = nehezk.Controls.OfType<Button>()
                              .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "nehezkazamataba");
                                if (button != null)
                                {
                                    string alap = string.Empty;
                                    for (int j = 0; j < kartyak.Count; j++)
                                    {
                                        if ("v " + kartyak[j].Item1 == vezerkartyak[i].Item1)
                                        {
                                            alap = kartyak[j].Item1;
                                        }
                                    }

                                    Button butn = nehezk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == alap + "nehezkazamataba");
                                    if (butn != null)
                                    {
                                        if (butn.BackColor == Color.Yellow)
                                        {

                                        }
                                        else
                                        {
                                            if (button.BackColor == Color.Yellow)
                                            {
                                                button.BackColor = Color.White;
                                            }
                                            else { button.BackColor = Color.Yellow; }
                                        }
                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.Yellow)
                                        {
                                            button.BackColor = Color.White;
                                        }
                                        else { button.BackColor = Color.Yellow; }
                                    }
                                }
                            }

                            if (name == vezerkartyak[i].Item1 + "megakazamataba")
                            {
                                Button button = megak.Controls.OfType<Button>()
                              .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "megakazamataba");
                                if (button != null)
                                {
                                    string alap = string.Empty;
                                    for (int j = 0; j < kartyak.Count; j++)
                                    {
                                        if ("v " + kartyak[j].Item1 == vezerkartyak[i].Item1)
                                        {
                                            alap = kartyak[j].Item1;
                                        }
                                    }

                                    Button butn = megak.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == alap + "megakazamataba");
                                    if (butn != null)
                                    {
                                        if (butn.BackColor == Color.Yellow)
                                        {

                                        }
                                        else
                                        {
                                            if (button.BackColor == Color.Yellow)
                                            {
                                                button.BackColor = Color.White;
                                            }
                                            else { button.BackColor = Color.Yellow; }
                                        }
                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.Yellow)
                                        {
                                            button.BackColor = Color.White;
                                        }
                                        else { button.BackColor = Color.Yellow; }
                                    }
                                }
                            }
                        }
                               
                        if (name == kartyak[i].Item1 + "egyszerukazamataba")
                        {
                            Button button = egyszeruk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "egyszerukazamataba");
                            if (button != null)
                            {
                                Button butn = egyszeruk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == "v "+kartyak[i].Item1 + "egyszerukazamataba");
                                if (butn != null)
                                {
                                    if (butn.BackColor == Color.Yellow)
                                    {

                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.Yellow)
                                        {
                                            button.BackColor = Color.White;
                                        }
                                        else { button.BackColor = Color.Yellow; }
                                    }
                                }
                                else 
                                {
                                    if (button.BackColor == Color.Yellow)
                                    {
                                        button.BackColor = Color.White;
                                    }
                                    else { button.BackColor = Color.Yellow; }
                                }                                
                            }
                        }

                        if (name == kartyak[i].Item1 + "konnyukazamataba")
                        {
                            Button button = konnyuk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "konnyukazamataba");
                            if (button != null)
                            {
                                Button butn = konnyuk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == "v " + kartyak[i].Item1 + "konnyukazamataba");
                                if (butn != null)
                                {
                                    if (butn.BackColor == Color.Yellow)
                                    {

                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.Yellow)
                                        {
                                            button.BackColor = Color.White;
                                        }
                                        else { button.BackColor = Color.Yellow; }
                                    }
                                }
                                else
                                {
                                    if (button.BackColor == Color.Yellow)
                                    {
                                        button.BackColor = Color.White;
                                    }
                                    else { button.BackColor = Color.Yellow; }
                                }
                            }
                        }

                        if (name == kartyak[i].Item1 + "nehezkazamataba")
                        {
                            Button button = nehezk.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "nehezkazamataba");
                            if (button != null)
                            {
                                Button butn = nehezk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == "v " + kartyak[i].Item1 + "nehezkazamataba");
                                if (butn != null)
                                {
                                    if (butn.BackColor == Color.Yellow)
                                    {

                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.Yellow)
                                        {
                                            button.BackColor = Color.White;
                                        }
                                        else { button.BackColor = Color.Yellow; }
                                    }
                                }
                                else
                                {
                                    if (button.BackColor == Color.Yellow)
                                    {
                                        button.BackColor = Color.White;
                                    }
                                    else { button.BackColor = Color.Yellow; }
                                }
                            }
                        }

                        if (name == kartyak[i].Item1 + "megakazamataba")
                        {
                            Button button = megak.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "megakazamataba");
                            if (button != null)
                            {
                                Button butn = megak.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == "v " + kartyak[i].Item1 + "megakazamataba");
                                if (butn != null)
                                {
                                    if (butn.BackColor == Color.Yellow)
                                    {

                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.Yellow)
                                        {
                                            button.BackColor = Color.White;
                                        }
                                        else { button.BackColor = Color.Yellow; }
                                    }
                                }
                                else
                                {
                                    if (button.BackColor == Color.Yellow)
                                    {
                                        button.BackColor = Color.White;
                                    }
                                    else { button.BackColor = Color.Yellow; }
                                }
                            }
                        }                        
                    }
                }
            }
        }

        private void LoadFile(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "loaddefault") //alaphelyzetek kivalasztasahoz belepes
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Select a file";
                dialog.Filter = "Game Save Files (*.gamesave)|*.gamesave";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    Console.WriteLine("You picked: " + selectedFile);
                    StreamReader sr = new StreamReader(dialog.FileName);
                    Világsoronként(sr, "con/con");
                    sr.Close();
                    savestart.Hide();
                    savestart.Controls.Clear();
                    savestart.SendToBack();
                    MainScreen();
                    playerscreen.Dock = DockStyle.Fill;
                    playerscreen.BringToFront();
                }
            }
            else if (name == "loadsavefile") //mentes kivalasztasahoz belepes
            {

            }
        }

        private void SaveNewfile(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "done")
            {
                TextBox button = mester.Controls.OfType<TextBox>()
                                    .FirstOrDefault(b => b.Name == "ujsavename");
                if (button != null)
                {
                    savefilenameinput = button.Text;
                }

                var txtFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\savefiles\starting\"));
                var fileName = savefilenameinput + ".gamesave";

                string fullPath = Path.Combine(txtFilePath, fileName);

                if (File.Exists(fullPath))
                {

                }
                else
                {
                    if (savefilenameinput != "" && savefilenameinput != null && kartyak.Count > 0 && playercards.Count > 0 && (kazamataegyszeru.Count + kazamatakicsi.Count + kazamatanagy.Count + kazamatamega.Count) > 0)
                    {
                        var txtFinalPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\savefiles\starting\" + savefilenameinput + ".gamesave"));
                        StreamWriter sw = new StreamWriter(txtFinalPath);
                        for (int i = 0; i < kartyak.Count; i++)
                        {
                            sw.WriteLine("uj kartya;" + kartyak[i].Item1 + ";" + kartyak[i].Item2 + ";" + kartyak[i].Item3 + ";" + kartyak[i].Item4);
                        }
                        sw.WriteLine();
                        if (vezerkartyak.Count > 0)
                        {
                            for (int i = 0; i < vezerkartyak.Count; i++)
                            {
                                for (int j = 0; j < kartyak.Count; j++)
                                {
                                    if ("v " + kartyak[j].Item1 == vezerkartyak[i].Item1)
                                    {
                                        string se = string.Empty;
                                        if (vezerkartyak[i].Item2 == kartyak[j].Item2)
                                        {
                                            se = "sebzes";
                                        }
                                        else
                                        {
                                            se = "eletero";
                                        }

                                        sw.WriteLine("uj vezer;" + vezerkartyak[i].Item1 + ";" + kartyak[j].Item1 + ";" + se);
                                    }
                                }
                            }
                            sw.WriteLine();
                        }

                        for (int i = 0; i < kazamataegyszeru.Count; i++)
                        {
                            sw.WriteLine("uj kazamata;egyszeru;" + kazamataegyszeru[i].Item1 + ";" + kazamataegyszeru[i].Item2 + ";" + kazamataegyszeru[i].Item3);
                        }
                        if (kazamatakicsi.Count > 0)
                        {
                            for (int i = 0; i < kazamatakicsi.Count; i++)
                            {
                                sw.WriteLine("uj kazamata;kicsi;" + kazamatakicsi[i].Item1 + ";" + kazamatakicsi[i].Item2 + "," + kazamatakicsi[i].Item3 + "," + kazamatakicsi[i].Item4 + ";" + kazamatakicsi[i].Item5 + ";" + kazamatakicsi[i].Item6);
                            }
                        }
                        if (kazamatanagy.Count > 0)
                        {
                            for (int i = 0; i < kazamatanagy.Count; i++)
                            {
                                sw.WriteLine("uj kazamata;nagy;" + kazamatanagy[i].Item1 + ";" + kazamatanagy[i].Item2 + "," + kazamatanagy[i].Item3 + "," + kazamatanagy[i].Item4 + "," + kazamatanagy[i].Item5 + "," + kazamatanagy[i].Item6 + ";" + kazamatanagy[i].Item7);
                            }
                        }
                        if (kazamatamega.Count > 0)
                        {
                            for (int i = 0; i < kazamatamega.Count; i++)
                            {
                                sw.WriteLine("uj kazamata;mega;" + kazamatamega[i].Item1 + ";" + kazamatamega[i].Item2 + "," + kazamatamega[i].Item3 + "," + kazamatamega[i].Item4 + "," + kazamatamega[i].Item5 + "," + kazamatamega[i].Item6 + ";" + kazamatamega[i].Item7 + "," + kazamatamega[i].Item8 + "," + kazamatamega[i].Item9 + ";" + kazamatamega[i].Item10);
                            }
                        }
                        sw.WriteLine();
                        sw.WriteLine("uj jatekos");
                        sw.WriteLine();
                        for (int i = 0; i < playercards.Count; i++)
                        {
                            sw.WriteLine("felvetel gyujtemenybe;" + playercards[i].Item1);
                        }
                        sw.Close();
                        foreach (Control ctrl in this.Controls)
                        {
                            if (ctrl is Panel panel && panel.Visible)
                            {
                                panel.Hide();
                            }
                        }
                        menu.Show();
                    }
                }
            }
        }

        private void VezerBuffok(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "dmg") //uj vezer sebzesduplazas
            {
                if (kartyak[vezerrefejlesztessorszam].Item2 > 50)
                {
                    vezerkartyak.Add(ujvezersorszam, ("v " + kartyak[vezerrefejlesztessorszam].Item1, 100, kartyak[vezerrefejlesztessorszam].Item3, kartyak[vezerrefejlesztessorszam].Item4));
                }
                else
                {
                    vezerkartyak.Add(ujvezersorszam, ("v " + kartyak[vezerrefejlesztessorszam].Item1, (kartyak[vezerrefejlesztessorszam].Item2) * 2, kartyak[vezerrefejlesztessorszam].Item3, kartyak[vezerrefejlesztessorszam].Item4));
                }
                ujvezersorszam++;
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }
                mester.Show();
            }
            else if (name == "hp") //uj vezer eletduplazas
            {
                if (kartyak[vezerrefejlesztessorszam].Item3 > 50)
                {
                    vezerkartyak.Add(ujvezersorszam, ("v " + kartyak[vezerrefejlesztessorszam].Item1, kartyak[vezerrefejlesztessorszam].Item2, 100, kartyak[vezerrefejlesztessorszam].Item4));
                }
                else
                {
                    vezerkartyak.Add(ujvezersorszam, ("v " + kartyak[vezerrefejlesztessorszam].Item1, kartyak[vezerrefejlesztessorszam].Item2, (kartyak[vezerrefejlesztessorszam].Item3) * 2, kartyak[vezerrefejlesztessorszam].Item4));
                }
                ujvezersorszam++;
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }
                mester.Show();
            }
        }

        public void Világsoronként(StreamReader sr, string bemenet)
        {//kártya, vezér, kazamata, játékos, felvétel, pakli
            int kartyan = 0;
            int vezerkartyan = 0;
            int playercardsn = 0;
            bool ujjatakos = false;
            while (!sr.EndOfStream)
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
                            if (bemenet == "con/con")
                            {
                                kazamataegyszeru[kazamataegyszeru.Count] = (sorreszek[2], sorreszek[3], sorreszek[4]);
                            }
                        }
                        else if (sorreszek[1] == "kis")
                        {
                            string[] ellenfelek = sorreszek[3].Split(',');
                            if (bemenet == "con/con")
                            {
                                kazamatakicsi[kazamatakicsi.Count] = (sorreszek[2], ellenfelek[0], ellenfelek[1], ellenfelek[2], sorreszek[4], sorreszek[5]);
                            }
                        }
                        else if (sorreszek[1] == "nagy")
                        {
                            string[] ellenfelek = sorreszek[3].Split(',');
                            if (bemenet == "con/con")
                            {
                                kazamatanagy[kazamatanagy.Count] = (sorreszek[2], ellenfelek[0], ellenfelek[1], ellenfelek[2], ellenfelek[3], ellenfelek[4], sorreszek[4]);
                            }
                        }
                        else if (sorreszek[1] == "mega")
                        {
                            string[] ellenfelek = sorreszek[3].Split(',');
                            string[] vezerellenfelek = sorreszek[4].Split(',');
                            kazamatamega[kazamatamega.Count] = (sorreszek[2], ellenfelek[0], ellenfelek[1], ellenfelek[2], ellenfelek[3], ellenfelek[4], vezerellenfelek[0], vezerellenfelek[1], vezerellenfelek[2], sorreszek[5]);
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
                    if (sorreszek.Length > 1)
                    {
                        Pakli.Clear();
                        string[] nevek = sorreszek[1].Split(',');
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
                }
            }
        }

        private void MainScreen()
        {
            int x = 5;
            System.Windows.Forms.Label info3lbl = new System.Windows.Forms.Label();
            info3lbl.Name = "info3";
            info3lbl.Text = "Paklid:";
            info3lbl.Font = new Font("Microsoft Sans Seriff", 14);
            info3lbl.Location = new Point(3, 450);
            info3lbl.Size = new System.Drawing.Size(110, 30);
            playerscreen.Controls.Add(info3lbl);

            Button ÚjPakli = new System.Windows.Forms.Button();
            ÚjPakli.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            ÚjPakli.Location = new System.Drawing.Point(120, 443);
            ÚjPakli.Name = "ÚjPakli";
            ÚjPakli.Size = new System.Drawing.Size(120, 40);
            ÚjPakli.TabIndex = 0;
            ÚjPakli.Text = "Új pakli";
            ÚjPakli.UseVisualStyleBackColor = true;
            ÚjPakli.Enabled = true;
            ÚjPakli.Click += new System.EventHandler(ÚjPakli_Click);
            playerscreen.Controls.Add(ÚjPakli);

            System.Windows.Forms.Label info6lbl = new System.Windows.Forms.Label();
            info6lbl.Name = "info6";
            info6lbl.Text = "Pakli módosításához nyomd meg az alábbi kártyákat."+ Environment.NewLine +"Pakli kiürétéséhez nyomd meg az új pakli gombot." + Environment.NewLine + "A pakliban gyűjteményednek legfeljebb fele szerepelhet.";
            info6lbl.Font = new Font("Microsoft Sans Seriff", 12);
            info6lbl.Location = new Point(302, 270);
            info6lbl.Size = new System.Drawing.Size(800, 70);
            playerscreen.Controls.Add(info6lbl);
        }


        List<int> paklint = new List<int>();
        private void ÚjPakli_Click(object sender, EventArgs e)
        {
            int paklix = 0;
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
            foreach (var btn in playerscreen.Controls.OfType<Button>().Where(b => b.Name.StartsWith("paklibtn")).ToList())
            {
                this.Controls.Remove(btn);
                btn.Dispose();
            }
            foreach (var btn in playerscreen.Controls.OfType<Button>())
            {
                if (btn.Name == "kazmgomb1" || btn.Name == "kazmgomb2" || btn.Name == "kazmgomb3")
                {
                    btn.Enabled = false;
                }
            }
        }










        /*elso fordulo
        public Form1()
        {
            InitializeComponent();
                        
            //elsofordulo
            /*kartyaklbl();
            Playerlabel();
            kartyaidbuttons();
            Paklilabel();
            vezerek();
            KazmataGombok();
        }
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
            System.Windows.Forms.Label info3lbl = new System.Windows.Forms.Label();
            info3lbl.Name = "info3";
            info3lbl.Text = "Paklid:";
            info3lbl.Font = new Font("Microsoft Sans Seriff", 14);
            info3lbl.Location = new Point(3, 450);
            info3lbl.Size = new System.Drawing.Size(110, 30);
            this.Controls.Add(info3lbl);

            /*Button KészPakli = new System.Windows.Forms.Button();
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
            ÚjPakli.Location = new System.Drawing.Point(120, 443);
            ÚjPakli.Name = "ÚjPakli";
            ÚjPakli.Size = new System.Drawing.Size(120, 40);
            ÚjPakli.TabIndex = 0;
            ÚjPakli.Text = "Új pakli";
            ÚjPakli.UseVisualStyleBackColor = true;
            ÚjPakli.Enabled = true;
            ÚjPakli.Click += new System.EventHandler(ÚjPakli_Click);
            this.Controls.Add(ÚjPakli);

            System.Windows.Forms.Label info6lbl = new System.Windows.Forms.Label();
            info6lbl.Name = "info6";
            info6lbl.Text = "Pakli módosíttásához nyomd meg az alábbi kártyákat."+ Environment.NewLine +"Pakli kiürétéséhez nyomd meg az új pakli gombot." + Environment.NewLine + "A pakliban gyűjteményednek legfeljebb fele szerepelhet.";
            info6lbl.Font = new Font("Microsoft Sans Seriff", 12);
            info6lbl.Location = new Point(302, 270);
            info6lbl.Size = new System.Drawing.Size(800, 70);
            this.Controls.Add(info6lbl);
        }

        private void kartyaidbuttons()
        {
            int x = 5;
            foreach (int i in playercards.Keys)
            {
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
                lbl.BorderStyle = BorderStyle.FixedSingle;
                lbl.Click += Button_Click;
                this.Controls.Add(lbl);
                x = x + 99;
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
                lbl.Click += new System.EventHandler(PakliClick);
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

        private void PakliClick(object sender, EventArgs e)
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

        private void ÚjPakli_Click(object sender, EventArgs e)
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
            kartyaklbl();
            Playerlabel();
            kartyaidbuttons();
            Paklilabel();
            vezerek();
            KazmataGombok();


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
                lbl.Name = "kazmgomb1";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 630);
                lbl.Enabled = false;
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
                lbl.Name = "kazmgomb2";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 680);
                lbl.Enabled = false;
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
                lbl.Name = "kazmgomb3";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = nev;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, 730);
                lbl.Enabled = false;
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
                    Console.WriteLine(playercards[10].Item1);

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

            if (ellélet < 0)
            {
                ellélet = 0;
            }
            return ellélet;
        }*/
    }
    }
