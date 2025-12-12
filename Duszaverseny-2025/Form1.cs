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
    //jatekmester kazamata cooked af
    //kazamata jutalmak es azok kiirasa kartyak kezelese screenen
    //kazamataban a pakliban levo kartyakat nem pakliba valasztas sorrendje alapjan pakolja

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
        string loadedPath;
        string savename;
        int currentdifficulty;
        int powerupcount;

        Dictionary<int, (string, int, int, string)> kartyak = new Dictionary<int, (string, int, int, string)>(); //név, sebzés, életerő, típus
        Dictionary<int, (string, int, int, string, string, string)> vezerkartyak = new Dictionary<int, (string, int, int, string, string, string)>();
        Dictionary<int, (string, int, int, string)> playercards = new Dictionary<int, (string, int, int, string)>(); //properies of the player's cards
        List<string> Pakli = new List<string>();
        int paklix = 5;

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
        Panel buffselect = new Panel();
        Panel kartyakscreen = new Panel();
        Panel kazmatascreen = new Panel();


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
            buffselect.Hide();
            menuu();
        }

        private void button(string text, string name, int sx, int sy, int locx, int locy, int fontsize, Panel panel, EventHandler clickevent, System.Drawing.Color backcolor, System.Drawing.Color forecolor, System.Drawing.Color bordercolor)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Name = name;
            btn.Size = new Size(sx, sy);
            Point btnloc = new Point(locx, locy);
            btn.Location = btnloc;
            btn.Font = new Font("Microsoft Sans Seriff", fontsize);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.BackColor = backcolor;
            btn. ForeColor = forecolor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 3;
            btn.FlatAppearance.BorderColor = bordercolor;
            btn.Click += clickevent;
            panel.Controls.Add(btn);
        }

        private void textbox(string name, int sx, int sy, int locx, int locy, Panel panel, System.Drawing.Color backcolor, System.Drawing.Color forecolor, int fontsize, string text)
        {
            TextBox textbox = new TextBox();
            textbox.Name = name;
            textbox.Size = new Size(sx, sy);
            Point textboxlox = new Point(locx, locy);
            textbox.BackColor = backcolor;
            textbox.ForeColor = forecolor;
            textbox.Multiline = true;
            textbox.Font = new Font("Microsoft Sans Seriff", fontsize);
            textbox.TextAlign = HorizontalAlignment.Center;
            textbox.Location = textboxlox;
            textbox.Text = text;
            panel.Controls.Add(textbox);
        }

        private void label(string text, string name, int sx, int sy, int locx, int locy, int fontsize, Panel panel, System.Drawing.Color backcolor, System.Drawing.Color forecolor)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Text = text;
            label.Size = new Size(sx, sy);
            Point labelloc = new Point(locx, locy);
            label.Location = labelloc;
            label.Font = new Font("Microsoft Sans Seriff", fontsize);
            label.BackColor = backcolor;
            label.ForeColor = forecolor;
            label.TextAlign = ContentAlignment.MiddleCenter;
            panel.Controls.Add(label);
        }

        private void menuu()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel panel && panel.Visible)
                {
                    panel.Hide();
                }
            }
            kartyak.Clear();
            playercards.Clear();
            vezerkartyak.Clear();
            kazamataegyszeru.Clear();
            kazamatakicsi.Clear();
            kazamatanagy.Clear();
            kazamatamega.Clear();
            ujkartyasorszam = 0;
            gyujtemenysorszam = 0;
            ujvezersorszam = 0;
            ujkiskazamatasorszam = 0;
            ujegyszerukazamatasorszam = 0;
            ujnagykazamatasorszam = 0;
            ujmegakazamatasorszam = 0;
            menu.Controls.Clear();
            menu.Show();
            menu.BackColor = Color.DimGray;
            menu.Dock = DockStyle.Fill;
            this.Controls.Add(menu);
            menu.BringToFront();

            label("Damareen", "title", 400, 200, 350, 100, 50, menu, Color.Transparent, Color.White);
            button("Játékos", "jatekosgomb", 200, 200, 150, 300, 20, menu, MainScreenGombok, Color.DarkGray, Color.Black, Color.DarkRed);
            button("Játékmester", "jatekmestergomb", 200, 200, 750, 300, 20, menu, MainScreenGombok, Color.DarkGray, Color.Black, Color.DarkRed);
            label("Újdonságok:\nMega kazamaták: 5 ellenség és 3 vezér, ebből lehet új kártyát kapni\n\nErősítések: nagy kazamatából kapható,\negy körre megduplázza egy választott kártya egyik adatát", "info", 400, 200, 350, 500, 14, menu, Color.Transparent, Color.White);
        }

        private void MainScreenGombok(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "jatekosgomb") //jatekos menube belepes
            {
                kartyak.Clear();
                playercards.Clear();
                vezerkartyak.Clear();
                kazamataegyszeru.Clear();
                kazamatakicsi.Clear();
                kazamatanagy.Clear();
                kazamatamega.Clear();
                ujkartyasorszam = 0;
                gyujtemenysorszam = 0;
                ujvezersorszam = 0;
                ujkiskazamatasorszam = 0;
                ujegyszerukazamatasorszam = 0;
                ujnagykazamatasorszam = 0;
                ujmegakazamatasorszam = 0;
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
                savestart.BackColor = Color.DimGray;

                button("Mentések", "loadsavefile", 200, 200, 150, 300, 20, savestart, LoadFile, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Alaphelyzetek", "loaddefault", 200, 200, 750, 300, 20, savestart, LoadFile, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Vissza", "visszamenu", 150, 75, 35, 745, 20, savestart, menuuu, Color.DarkGray, Color.Black, Color.DarkRed);
                ComboBox comboBox = new ComboBox();
                comboBox.Name = "difficulty";
                for (int i = 0; i <= 10; i++)
                {
                    comboBox.Items.Add(i);
                }
                comboBox.SelectedIndex = 0;
                comboBox.Location = new Point(775, 570);
                comboBox.Size = new Size(150, 20);
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                savestart.Controls.Add(comboBox);
                label("Nehézségi szint:", "info", 150, 30, 775, 520, 14, savestart, Color.Transparent, Color.Black);
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
                ujkartyasorszam = 0;
                gyujtemenysorszam = 0;
                ujvezersorszam = 0;
                ujkiskazamatasorszam = 0;
                ujegyszerukazamatasorszam = 0;
                ujnagykazamatasorszam = 0;
                ujmegakazamatasorszam = 0;
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel panel && panel.Visible)
                    {
                        panel.Hide();
                    }
                }
                mester.Controls.Clear();
                mester.Show();
                mester.BackColor = Color.DimGray;
                mester.Dock = DockStyle.Fill;
                this.Controls.Add(mester);
                mester.BringToFront();

                label("Új világ létrehozása:", "ujvilaginfo", 500, 75, 0, 25, 35, mester, Color.Transparent, Color.Black);
                label("Új világ létrehozásához egy nem üres gyűjtemény, legalább egy kazamata definiálása, illetve egy név megadása szükséges.", "ujvilagfeltetel", 500, 35, 0, 100, 10, mester, Color.Transparent, Color.Black);
                textbox("ujsavename", 400, 35, 500, 50, mester, Color.White, Color.Black, 20, "Új világ");
                button("Kész", "done", 150, 75, 925, 25, 20, mester, SaveNewfile, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Új kártya\nhozzáadása", "ujkartya", 400, 100, 50, 200, 20, mester, Újdolgok, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Új vezér\nhozzáadása", "ujvezer", 400, 100, 650, 200, 20, mester, Újdolgok, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Új kazamata\nhozzáadása", "ujkazamata", 400, 100, 50, 500, 20, mester, Újdolgok, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Gyűjtemény\nkiválasztása", "gyujtemeny", 400, 100, 650, 500, 20, mester, Újdolgok, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Vissza", "visszamenu", 150, 75, 35, 745, 20, mester, menuuu, Color.DarkGray, Color.Black, Color.DarkRed);
            }
        }

        private void menuuu(object sender, EventArgs e)
        {
            menuu();
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
                ujkartya.BackColor = Color.DimGray;
                ujkartya.Dock = DockStyle.Fill;
                this.Controls.Add(ujkartya);
                ujkartya.BringToFront();

                label("Új kártya neve:", "ujkname", 300, 50, 150, 75, 20, ujkartya, Color.Transparent, Color.Black);
                textbox("ujknameinput", 350, 35, 125, 150, ujkartya, Color.White, Color.Black, 20, "Max 16 karakter");
                label("Új kártya sebzése:", "ujkdmg", 300, 50, 150, 325, 20, ujkartya, Color.Transparent, Color.Black);
                textbox("ujkdmginput", 350, 35, 125, 400, ujkartya, Color.White, Color.Black, 20, "2 és 100 közötti szám");
                label("Új kártya életereje:", "ujkhp", 300, 50, 150, 575, 20, ujkartya, Color.Transparent, Color.Black);
                textbox("ujkhpinput", 350, 35, 125, 650, ujkartya, Color.White, Color.Black, 20, "1 és 100 közötti szám");
                label("Új kártya típusa:", "ujktype", 300, 50, 650, 75, 20, ujkartya, Color.Transparent, Color.Black);
                button("Tűz", "tuz", 150, 150, 625, 200, 20, ujkartya, ColortheTypes, Color.DarkGray, Color.Black, Color.Orange);
                button("Víz", "viz", 150, 150, 825, 200, 20, ujkartya, ColortheTypes, Color.DarkGray, Color.Black, Color.Blue);
                button("Levegő", "levego", 150, 150, 625, 400, 20, ujkartya, ColortheTypes, Color.DarkGray, Color.Black, Color.LightBlue);
                button("Föld", "fold", 150, 150, 825, 400, 20, ujkartya, ColortheTypes, Color.DarkGray, Color.Black, Color.SaddleBrown);
                button("Hozzáadás", "registercard", 200, 75, 825, 745, 20, ujkartya, Újdolgok, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Vissza", "visszamester", 150, 75, 35, 745, 20, ujkartya, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
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
                    ujvezer.BackColor = Color.DimGray;
                    ujvezer.Show();
                    ujvezer.Dock = DockStyle.Fill;
                    this.Controls.Add(ujvezer);
                    ujvezer.BringToFront();

                    label("Kártya vezérré fejlesztése", "vezerinfo", 600, 75, 0, 25, 35, ujvezer, Color.Transparent, Color.Black);
                    button("Vissza", "visszamester", 150, 75, 35, 745, 20, ujvezer, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);

                    Color border = new Color();
                    string tipisekezettel = string.Empty;

                    int x = 10;
                    int y = 125;
                    bool vanev = false;
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        if (kartyak[i].Item4 == "tuz") { border = Color.Orange; tipisekezettel = "Tűz"; }
                        else if (kartyak[i].Item4 == "viz") { border = Color.Blue; tipisekezettel = "VÍz"; }
                        else if (kartyak[i].Item4 == "levego") { border = Color.LightBlue; tipisekezettel = "Levegő"; }
                        else if (kartyak[i].Item4 == "fold") { border = Color.SaddleBrown; tipisekezettel = "Föld"; }
                        if (vezerkartyak.Count > 0)
                        {
                            for (int j = 0; j < vezerkartyak.Count; j++)
                            {
                                if (vezerkartyak[j].Item5 == kartyak[i].Item1) { vanev = true; }
                            }
                            if (vanev == false)
                            {
                                button(kartyak[i].Item1 + Environment.NewLine + "⚔️" + kartyak[i].Item2 + "/" + kartyak[i].Item3 + "❤️" + Environment.NewLine + tipisekezettel, kartyak[i].Item1 + "ujvezer", 100, 130, x, y, 10, ujvezer, button_Click, Color.DarkGray, Color.Black, border);
                                x += 110;
                                if (x > 1000)
                                {
                                    y += 140;
                                    x = 10;
                                }
                            }
                            else { vanev = false; }
                        }
                        else
                        {
                            button(kartyak[i].Item1 + Environment.NewLine + "⚔️" + kartyak[i].Item2 + "/" + kartyak[i].Item3 + "❤️" + Environment.NewLine + tipisekezettel, kartyak[i].Item1 + "ujvezer", 100, 130, x, y, 10, ujvezer, button_Click, Color.DarkGray, Color.Black, border);
                            x += 110;
                            if (x > 1000)
                            {
                                y += 140;
                                x = 10;
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
                    ujkazamata.BackColor = Color.DimGray;
                    this.Controls.Add(ujkazamata);
                    ujkazamata.BringToFront();

                    button("Vissza", "visszamester", 100, 100, 400, 400, 20, ujkazamata, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                    button("Egyszerű", "egyszeruk", 200, 200, 0, 0, 20, ujkazamata, kazamaták, Color.DarkGray, Color.Black, Color.DarkRed);
                    button("Könnyű", "konnyuk", 200, 200, 200, 0, 20, ujkazamata, kazamaták, Color.DarkGray, Color.Black, Color.DarkRed);
                    button("Nehéz", "nehezk", 200, 200, 0, 200, 20, ujkazamata, kazamaták, Color.DarkGray, Color.Black, Color.DarkRed);
                    button("Mega", "megak", 200, 200, 200, 200, 20, ujkazamata, kazamaták, Color.DarkGray, Color.Black, Color.DarkRed);
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
                    gyujtemeny.BackColor = Color.DimGray;
                    gyujtemeny.Show();
                    gyujtemeny.Dock = DockStyle.Fill;
                    this.Controls.Add(gyujtemeny);
                    gyujtemeny.BringToFront();

                    label("Gyűjtemény kiválasztása", "gyujtemenyinfo", 600, 75, 0, 25, 35, gyujtemeny, Color.Transparent, Color.Black);
                    button("Kész", "gyujtemenydone", 150, 75, 35, 745, 20, gyujtemeny, Újdolgok, Color.DarkGray, Color.Black, Color.DarkRed);

                    Color border = new Color();
                    string tipisekezettel = string.Empty;
                    int x = 10;
                    int y = 125;
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        if (kartyak[i].Item4 == "tuz") { border = Color.Orange; tipisekezettel = "Tűz"; }
                        else if (kartyak[i].Item4 == "viz") { border = Color.Blue; tipisekezettel = "VÍz"; }
                        else if (kartyak[i].Item4 == "levego") { border = Color.LightBlue; tipisekezettel = "Levegő"; }
                        else if (kartyak[i].Item4 == "fold") { border = Color.SaddleBrown; tipisekezettel = "Föld"; }
                        button(kartyak[i].Item1 + Environment.NewLine + "⚔️" + kartyak[i].Item2 + "/" + kartyak[i].Item3 + "❤️" + Environment.NewLine + tipisekezettel, kartyak[i].Item1 + "gyujtemenybee", 100, 130, x, y, 10, gyujtemeny, button_Click, Color.DarkGray, Color.Black, border);

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
                                        button.BackColor = Color.White;
                                    }
                                }
                            }
                        }
                        x += 110;
                        if (x > 1000)
                        {
                            y += 140;
                            x = 10;
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
                        if (button.BackColor == Color.White)
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

                if (ujkname != string.Empty && ujkdmg != 0 && ujkhp != 0 && selected != string.Empty && ujkname.Length <= 16 && ujkdmg >= 2 && ujkdmg <= 100 && ujkhp >= 1 && ujkhp <= 100) //csak akkor engedi letrehozni ha van megadva nev, ami max 14 karakter vezerre fejlesztes miatt, mert igy a vezer neve max 16 karakter lesz, tipus, dmg-és hp <= 100
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
                        else
                        {
                            TextBox doboz = ujkartya.Controls.OfType<TextBox>()
                            .FirstOrDefault(t => t.Name == "ujknameinput");
                            if (doboz != null)
                            {
                                doboz.Text = "Már van ilyen nevű kártya";
                            }
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
                egyszeruk.BackColor = Color.DimGray;
                egyszeruk.BringToFront();

                button("Vissza", "visszakazamata", 150, 100, 0, 0, 20, egyszeruk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Kész", "kazamatadone", 150, 100, 150, 0, 20, egyszeruk, kazamaták, Color.DarkGray, Color.Black, Color.DarkRed);
                textbox("egyszeruknameinput", 300, 100, 300, 0, egyszeruk, Color.White, Color.Black, 20, "Kazamata neve");
                button("Sebzés", "jutalomse", 150, 100, 600, 0, 20, egyszeruk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                label("nevezd el a kazamatát, válaszd ki a jutalmat, válaszd ki a kártyákat","info",300,50,2,700,12,egyszeruk,Color.DarkGray,Color.Black);

                int x = 750;
                int y = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    button(kartyak[i].Item1, kartyak[i].Item1 + "egyszerukazamataba", 75, 100, x, y, 10, egyszeruk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
                if (vezerkartyak.Count>0)
                {
                    for (int i = 0; i < vezerkartyak.Count; i++)
                    {
                        button(vezerkartyak[i].Item1, vezerkartyak[i].Item1 + "egyszerukazamataba", 75, 100, x, y, 10, egyszeruk, button_Click, Color.Black, Color.White, Color.Black);
                        x += 75;
                        if (x > 975)
                        {
                            y += 100;
                            x = 0;
                        }
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
                konnyuk.BackColor = Color.DimGray;

                button("Vissza", "visszakazamata", 150, 100, 0, 0, 20, konnyuk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Kész", "kazamatadone", 150, 100, 150, 0, 20, konnyuk, kazamaták, Color.DarkGray, Color.Black, Color.DarkRed);
                textbox("konnyuknameinput", 300, 100, 300, 0, konnyuk, Color.White, Color.Black, 20, "Kazamata neve");
                button("Sebzés", "jutalomse", 150, 100, 600, 0, 20, konnyuk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                label("nevezd el a kazamatát, válaszd ki a jutalmat, válaszd ki a kártyákat","info",300,50,2,700,12,konnyuk,Color.DarkGray,Color.Black);

                int x = 750;
                int y = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    button(kartyak[i].Item1, kartyak[i].Item1 + "konnyukazamataba", 75, 100, x, y, 10, konnyuk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
                if (vezerkartyak.Count>0)
                {
                    for (int i = 0; i < vezerkartyak.Count; i++)
                    {
                        button(vezerkartyak[i].Item1, vezerkartyak[i].Item1 + "konnyukazamataba", 75, 100, x, y, 10, konnyuk, button_Click, Color.Black, Color.White, Color.Black);
                        x += 75;
                        if (x > 975)
                        {
                            y += 100;
                            x = 0;
                        }
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
                nehezk.BackColor = Color.DimGray;
                label("nevezd el a kazamatát, válaszd ki a jutalmat, válaszd ki a kártyákat","info",300,50,2,700,12,nehezk,Color.DarkGray,Color.Black);

                button("Vissza", "visszakazamata", 150, 100, 0, 0, 20, nehezk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Kész", "kazamatadone", 150, 100, 150, 0, 20, nehezk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                textbox("nehezknameinput", 300, 100, 300, 0, nehezk, Color.White, Color.Black, 20, "Kazamata neve");

                int x = 600;
                int y = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    button(kartyak[i].Item1, kartyak[i].Item1 + "nehezkazamataba", 75, 100, x, y, 10, nehezk, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
                if (vezerkartyak.Count>0)
                {
                    for (int i = 0; i < vezerkartyak.Count; i++)
                    {
                        button(vezerkartyak[i].Item1, vezerkartyak[i].Item1 + "nehezkazamataba", 75, 100, x, y, 10, nehezk, button_Click, Color.Black, Color.White, Color.Black);
                        x += 75;
                        if (x > 975)
                        {
                            y += 100;
                            x = 0;
                        }
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
                megak.BackColor = Color.DimGray;
                label("nevezd el a kazamatát, válaszd ki a jutalmat, válaszd ki a kártyákat","info",300,50,2,700,12,megak,Color.DarkGray,Color.Black);

                button("Vissza", "visszakazamata", 150, 100, 0, 0, 20, megak, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Kész", "kazamatadone", 150, 100, 150, 0, 20, megak, kazamaták, Color.DarkGray, Color.Black, Color.DarkRed);
                textbox("megaknameinput", 300, 100, 300, 0, megak, Color.White, Color.Black, 20, "Kazamata neve");

                int x = 600;
                int y = 0;
                for (int i = 0; i < kartyak.Count; i++)
                {
                    button(kartyak[i].Item1, kartyak[i].Item1 + "megakazamataba", 75, 100, x, y, 10, megak, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                    x += 75;
                    if (x > 975)
                    {
                        y += 100;
                        x = 0;
                    }
                }
                if (vezerkartyak.Count > 0)
                {
                    for (int i = 0; i < vezerkartyak.Count; i++)
                    {
                        button(vezerkartyak[i].Item1, vezerkartyak[i].Item1 + "megakazamataba", 75, 100, x, y, 10, megak, button_Click, Color.Black, Color.White, Color.Black);
                        x += 75;
                        if (x > 975)
                        {
                            y += 100;
                            x = 0;
                        }
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
                            if (button.BackColor == Color.White)
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
                                if (button.BackColor == Color.White)   
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
                            if (button.BackColor == Color.White)
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
                                if (button.BackColor == Color.White)
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
                            if (button.BackColor == Color.White)
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
                                if (button.BackColor == Color.White)
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
                else if (megak.Visible)
                {
                    List<string> selectedkartyak = new List<string>();
                    selectedkartyak.Clear();
                    for (int i = 0; i < kartyak.Count; i++)
                    {
                        Button button = megak.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "megakazamataba");
                        if (button != null)
                        {
                            if (button.BackColor == Color.White)
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
                                if (button.BackColor == Color.White)
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
                                kazamatamega.Add(ujmegakazamatasorszam, (megakazamatanameinput, selectedkartyak[0], selectedkartyak[1], selectedkartyak[2], selectedkartyak[3], selectedkartyak[4], selectedvezer[0], selectedvezer[1], selectedvezer[2], "kartya"));
                                ujmegakazamatasorszam++;
                                megak.Hide();
                                ujkazamata.Show();
                            }
                        }
                        else
                        {
                            kazamatamega.Add(ujmegakazamatasorszam, (megakazamatanameinput, selectedkartyak[0], selectedkartyak[1], selectedkartyak[2], selectedkartyak[3], selectedkartyak[4], selectedvezer[0], selectedvezer[1], selectedvezer[2], "kartya"));
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
                        btnn.BackColor = Color.DarkGray;
                    }
                    btn.BackColor = Color.White;
                    selected = btn.Name;
                }
            }
        }

        private void VezerBuffok(object sender, EventArgs e)
        {
            string ujjvezernev = string.Empty;
            System.Windows.Forms.Label label = sebzeseletero.Controls.OfType<System.Windows.Forms.Label>()
                            .FirstOrDefault(t => t.Text == kartyak[vezerrefejlesztessorszam].Item1);
            if (label != null)
            {
                TextBox doboz = sebzeseletero.Controls.OfType<TextBox>()
                            .FirstOrDefault(t => t.Name == "ujvezernevepre");
                if (doboz != null)
                {
                    TextBox doboz1 = sebzeseletero.Controls.OfType<TextBox>()
                                .FirstOrDefault(t => t.Name == "ujvezernevesuf");
                    if (doboz1 != null)
                    {
                        if (doboz.Text == "" && doboz1.Text == "")
                        {
                            doboz.Text = "Legalább az egyik";
                            doboz1.Text = "mezőbe írj valamit!";
                        }
                        else
                        {
                            ujjvezernev = doboz.Text + label.Text + doboz1.Text;
                            Button btn = sender as Button;
                            string name = btn.Name;

                            if (vezerkartyak.Count > 0)
                            {
                                bool vane = false;
                                for (int i = 0; i < vezerkartyak.Count; i++)
                                {
                                    if (vezerkartyak[i].Item1 == ujjvezernev) { vane = true; }
                                }
                                if (vane == true)
                                {
                                    doboz.Text = "Van már ilyen";
                                    doboz1.Text = "nevű vezér!";
                                }
                                else
                                {
                                    if (name == "dmg") //uj vezer sebzesduplazas
                                    {
                                        if (kartyak[vezerrefejlesztessorszam].Item2 > 50)
                                        {
                                            vezerkartyak.Add(ujvezersorszam, (ujjvezernev, 100, kartyak[vezerrefejlesztessorszam].Item3, kartyak[vezerrefejlesztessorszam].Item4, kartyak[vezerrefejlesztessorszam].Item1, "sebzes"));
                                        }
                                        else
                                        {
                                            vezerkartyak.Add(ujvezersorszam, (ujjvezernev, (kartyak[vezerrefejlesztessorszam].Item2) * 2, kartyak[vezerrefejlesztessorszam].Item3, kartyak[vezerrefejlesztessorszam].Item4, kartyak[vezerrefejlesztessorszam].Item1, "sebzes"));
                                        }
                                        ujvezersorszam++;
                                    }
                                    else if (name == "hp") //uj vezer eletduplazas
                                    {
                                        if (kartyak[vezerrefejlesztessorszam].Item3 > 50)
                                        {
                                            vezerkartyak.Add(ujvezersorszam, (ujjvezernev, kartyak[vezerrefejlesztessorszam].Item2, 100, kartyak[vezerrefejlesztessorszam].Item4, kartyak[vezerrefejlesztessorszam].Item1, "eletero"));
                                        }
                                        else
                                        {
                                            vezerkartyak.Add(ujvezersorszam, (ujjvezernev, kartyak[vezerrefejlesztessorszam].Item2, (kartyak[vezerrefejlesztessorszam].Item3) * 2, kartyak[vezerrefejlesztessorszam].Item4, kartyak[vezerrefejlesztessorszam].Item1, "eletero"));
                                        }
                                        ujvezersorszam++;
                                    }
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
                            else
                            {
                                if (name == "dmg") //uj vezer sebzesduplazas
                                {
                                    if (kartyak[vezerrefejlesztessorszam].Item2 > 50)
                                    {
                                        vezerkartyak.Add(ujvezersorszam, (ujjvezernev, 100, kartyak[vezerrefejlesztessorszam].Item3, kartyak[vezerrefejlesztessorszam].Item4, kartyak[vezerrefejlesztessorszam].Item1, "sebzes"));
                                    }
                                    else
                                    {
                                        vezerkartyak.Add(ujvezersorszam, (ujjvezernev, (kartyak[vezerrefejlesztessorszam].Item2) * 2, kartyak[vezerrefejlesztessorszam].Item3, kartyak[vezerrefejlesztessorszam].Item4, kartyak[vezerrefejlesztessorszam].Item1, "sebzes"));
                                    }
                                    ujvezersorszam++;
                                }
                                else if (name == "hp") //uj vezer eletduplazas
                                {
                                    if (kartyak[vezerrefejlesztessorszam].Item3 > 50)
                                    {
                                        vezerkartyak.Add(ujvezersorszam, (ujjvezernev, kartyak[vezerrefejlesztessorszam].Item2, 100, kartyak[vezerrefejlesztessorszam].Item4, kartyak[vezerrefejlesztessorszam].Item1, "eletero"));
                                    }
                                    else
                                    {
                                        vezerkartyak.Add(ujvezersorszam, (ujjvezernev, kartyak[vezerrefejlesztessorszam].Item2, (kartyak[vezerrefejlesztessorszam].Item3) * 2, kartyak[vezerrefejlesztessorszam].Item4, kartyak[vezerrefejlesztessorszam].Item1, "eletero"));
                                    }
                                    ujvezersorszam++;
                                }
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
                    }
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
                else if (name == "visszavezer")
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is Panel panel && panel.Visible)
                        {
                            panel.Hide();
                        }
                    }
                    ujvezer.Show();
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
                            sebzeseletero.BackColor = Color.DimGray;
                            sebzeseletero.Show();
                            sebzeseletero.Dock = DockStyle.Fill;
                            this.Controls.Add(sebzeseletero);
                            sebzeseletero.BringToFront();

                            label("Vezér neve:", "ujvezerneveinfo", 200, 50, 450, 50, 20, sebzeseletero, Color.Transparent, Color.Black);
                            textbox("ujvezernevepre", 250, 35, 150, 150, sebzeseletero, Color.White, Color.Black, 20, "");
                            label(kartyak[i].Item1, "ujvezerneve", 300, 50, 400, 140, 20, sebzeseletero, Color.Transparent, Color.Black);
                            textbox("ujvezernevesuf", 250, 35, 700, 150, sebzeseletero, Color.White, Color.Black, 20, "");
                            button("Életerő duplázása", "hp", 200, 200, 325, 400, 20, sebzeseletero, VezerBuffok, Color.DarkGray, Color.Black, Color.DarkRed);
                            button("Sebzés duplázása", "dmg", 200, 200, 575, 400, 20, sebzeseletero, VezerBuffok, Color.DarkGray, Color.Black, Color.DarkRed);
                            button("Vissza", "visszavezer", 150, 75, 35, 745, 20, sebzeseletero, button_Click, Color.DarkGray, Color.Black, Color.DarkRed);
                        }

                        if (name == kartyak[i].Item1 + "gyujtemenybee") //gyujtemenybe valasztott / kivett kartya
                        {
                            Button button = gyujtemeny.Controls.OfType<Button>()
                          .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "gyujtemenybee");
                            if (button != null)
                            {
                                if (button.BackColor == Color.White)
                                {
                                    button.BackColor = Color.DarkGray;
                                }
                                else { button.BackColor = Color.White; }
                            }
                        }

                        if (name == kartyak[i].Item1 + "kartyakpanelen") //gyujtemenybe valasztott / kivett kartya
                        {
                            int n = 0;
                                if (btn.BackColor == Color.White)
                                {
                                    btn.BackColor = Color.DarkGray;
                                }
                                else
                            {
                                for (int j = 0; j < playercards.Count; j++)
                                {
                                    Button buttonn = kartyakscreen.Controls.OfType<Button>()
                                    .FirstOrDefault(b => b.Name == kartyak[j].Item1 + "kartyakpanelen");
                                    if (buttonn.BackColor == Color.White && buttonn != null)
                                    {
                                        n++;
                                    }


                                }

                                if (n < playercards.Count / 2)
                                {

                                    if (btn.BackColor == Color.DarkGray)
                                    {
                                        btn.BackColor = Color.White;
                                    }


                                }
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
                                    for (int j = 0; j < kartyak.Count; j++)
                                    {
                                        if (kartyak[j].Item1 == vezerkartyak[i].Item5)
                                        {
                                            alap = kartyak[j].Item1;
                                        }
                                    }

                                    Button butn = egyszeruk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == alap + "egyszerukazamataba");
                                    if (butn != null)
                                    {
                                        if (butn.BackColor == Color.White)
                                        {

                                        }
                                        else
                                        {
                                            if (button.BackColor == Color.White)
                                            {
                                                button.BackColor = Color.DarkGray;
                                            }
                                            else { button.BackColor = Color.White; }
                                        }
                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.White)
                                        {
                                            button.BackColor = Color.DarkGray;
                                        }
                                        else { button.BackColor = Color.White; }
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
                                        if (kartyak[j].Item1 == vezerkartyak[i].Item5)
                                        {
                                            alap = kartyak[j].Item1;
                                        }
                                    }

                                    Button butn = konnyuk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == alap + "konnyukazamataba");
                                    if (butn != null)
                                    {
                                        if (butn.BackColor == Color.White)
                                        {

                                        }
                                        else
                                        {
                                            if (button.BackColor == Color.White)
                                            {
                                                button.BackColor = Color.DarkGray;
                                            }
                                            else { button.BackColor = Color.White; }
                                        }
                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.White)
                                        {
                                            button.BackColor = Color.DarkGray;
                                        }
                                        else { button.BackColor = Color.White; }
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
                                        if (kartyak[j].Item1 == vezerkartyak[i].Item5)
                                        {
                                            alap = kartyak[j].Item1;
                                        }
                                    }

                                    Button butn = nehezk.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == alap + "nehezkazamataba");
                                    if (butn != null)
                                    {
                                        if (butn.BackColor == Color.White)
                                        {

                                        }
                                        else
                                        {
                                            if (button.BackColor == Color.White)
                                            {
                                                button.BackColor = Color.DarkGray;
                                            }
                                            else { button.BackColor = Color.White; }
                                        }
                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.White)
                                        {
                                            button.BackColor = Color.DarkGray;
                                        }
                                        else { button.BackColor = Color.White; }
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
                                        if (kartyak[j].Item1 == vezerkartyak[i].Item5)   
                                        {
                                            alap = kartyak[j].Item1;
                                        }
                                    }

                                    Button butn = megak.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == alap + "megakazamataba");
                                    if (butn != null)
                                    {
                                        if (butn.BackColor == Color.White)
                                        {

                                        }
                                        else
                                        {
                                            if (button.BackColor == Color.White)
                                            {
                                                button.BackColor = Color.DarkGray;
                                            }
                                            else { button.BackColor = Color.White; }
                                        }
                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.White)
                                        {
                                            button.BackColor = Color.DarkGray;
                                        }
                                        else { button.BackColor = Color.White; }
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
                                        .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "egyszerukazamataba");
                                if (butn != null)
                                {
                                    if (butn.BackColor == Color.White) 
                                    {

                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.White)
                                        {
                                            button.BackColor = Color.DarkGray;
                                        }
                                        else { button.BackColor = Color.White; }
                                    }
                                }
                                else
                                {
                                    if (button.BackColor == Color.White)
                                    {
                                        button.BackColor = Color.DarkGray;
                                    }
                                    else { button.BackColor = Color.White; }
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
                                        .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "konnyukazamataba");
                                if (butn != null)
                                {
                                    if (butn.BackColor == Color.White)
                                    {

                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.White)
                                        {
                                            button.BackColor = Color.DarkGray;
                                        }
                                        else { button.BackColor = Color.White; }
                                    }
                                }
                                else
                                {
                                    if (button.BackColor == Color.White)
                                    {
                                        button.BackColor = Color.DarkGray;
                                    }
                                    else { button.BackColor = Color.White; }
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
                                        .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "nehezkazamataba");
                                if (butn != null)
                                {
                                    if (butn.BackColor == Color.White)
                                    {

                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.White)
                                        {
                                            button.BackColor = Color.DarkGray;
                                        }
                                        else { button.BackColor = Color.White; }
                                    }
                                }
                                else
                                {
                                    if (button.BackColor == Color.White)
                                    {
                                        button.BackColor = Color.DarkGray;
                                    }
                                    else { button.BackColor = Color.White; }
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
                                        .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "megakazamataba");
                                if (butn != null)
                                {
                                    if (butn.BackColor == Color.White)
                                    {

                                    }
                                    else
                                    {
                                        if (button.BackColor == Color.White)
                                        {
                                            button.BackColor = Color.DarkGray;
                                        }
                                        else { button.BackColor = Color.White; }
                                    }
                                }
                                else
                                {
                                    if (button.BackColor == Color.White)
                                    {
                                        button.BackColor = Color.DarkGray;
                                    }
                                    else { button.BackColor = Color.White; }
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
                dialog.Filter = "Játékkörnyezet Fájlok (*.gamedefaultsave)|*.gamedefaultsave";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    StreamReader sr = new StreamReader(dialog.FileName);
                    ComboBox combo = savestart.Controls["difficulty"] as ComboBox;
                    currentdifficulty = combo.SelectedIndex;
                    savename = Path.GetFileNameWithoutExtension(dialog.FileName);
                    loadedPath = selectedFile;
                    
                    Világsoronként(sr, "con/con");
                    sr.Close();
                    MainScreen(null,null);

                    savestart.Hide();
                    savestart.Controls.Clear();
                    savestart.SendToBack();
                    playerscreen.Show();

                    playerscreen.Visible = true;
                    playerscreen.BringToFront();
                    playerscreen.Dock = DockStyle.Fill;
                    this.Controls.Add(playerscreen);

                }
            }
            else if (name == "loadsavefile") //mentes kivalasztasahoz belepes
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Select a file";
                dialog.Filter = "Játék Mentés Fájlok (*.gamesave)|*.gamesave";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    StreamReader sr = new StreamReader(dialog.FileName);
                    ComboBox combo = savestart.Controls["difficulty"] as ComboBox;
                    currentdifficulty = combo.SelectedIndex;
                    savename = Path.GetFileNameWithoutExtension(dialog.FileName);
                    loadedPath = selectedFile;

                    Világsoronként(sr, "con/aux");
                    sr.Close();
                    MainScreen(null, null);

                    savestart.Hide();
                    savestart.Controls.Clear();
                    savestart.SendToBack();
                    playerscreen.Show();

                    playerscreen.Visible = true;
                    playerscreen.BringToFront();
                    playerscreen.Dock = DockStyle.Fill;
                    this.Controls.Add(playerscreen);

                }
            }
        }

        private void SaveNewfile(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            if (name == "done")
            {
                bool ujra = true;
                while (ujra == true)
                {
                    TextBox button = mester.Controls.OfType<TextBox>()
                                    .FirstOrDefault(b => b.Name == "ujsavename");
                    if (button != null)
                    {
                        savefilenameinput = button.Text;
                    }

                    //var txtFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\savefiles\starting\"));
                    var fileName = savefilenameinput + ".gamedefaultsave";
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    dialog.Description = "Válaszd ki a mentés helyét!\nFájl neve: " + fileName;
                    dialog.ShowNewFolderButton = true;
                    string txtFilePath = "";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        txtFilePath = dialog.SelectedPath;
                    }
                    if (txtFilePath != "")
                    {
                        string fullPath = Path.Combine(txtFilePath, fileName);

                        if (savefilenameinput != "" && savefilenameinput != null && kartyak.Count > 0 && playercards.Count > 0 && (kazamataegyszeru.Count + kazamatakicsi.Count + kazamatanagy.Count + kazamatamega.Count) > 0)
                        {
                            //var txtFinalPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\savefiles\starting\" + savefilenameinput + ".gamedefaultsave"));
                            //StreamWriter sw = new StreamWriter(txtFinalPath);
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < kartyak.Count; i++)
                            {
                                sb.AppendLine("uj kartya;" + kartyak[i].Item1 + ";" + kartyak[i].Item2 + ";" + kartyak[i].Item3 + ";" + kartyak[i].Item4);
                            }
                            sb.AppendLine();
                            if (vezerkartyak.Count > 0)
                            {
                                for (int i = 0; i < vezerkartyak.Count; i++)
                                {
                                    for (int j = 0; j < kartyak.Count; j++)
                                    {
                                        if (kartyak[j].Item1 == vezerkartyak[i].Item5)
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

                                            sb.AppendLine("uj vezer;" + vezerkartyak[i].Item1 + ";" + kartyak[j].Item1 + ";" + se);
                                        }
                                    }
                                }
                                sb.AppendLine();
                            }

                            for (int i = 0; i < kazamataegyszeru.Count; i++)
                            {
                                sb.AppendLine("uj kazamata;egyszeru;" + kazamataegyszeru[i].Item1 + ";" + kazamataegyszeru[i].Item2 + ";" + kazamataegyszeru[i].Item3);
                            }
                            if (kazamatakicsi.Count > 0)
                            {
                                for (int i = 0; i < kazamatakicsi.Count; i++)
                                {
                                    sb.AppendLine("uj kazamata;kicsi;" + kazamatakicsi[i].Item1 + ";" + kazamatakicsi[i].Item2 + "," + kazamatakicsi[i].Item3 + "," + kazamatakicsi[i].Item4 + ";" + kazamatakicsi[i].Item5 + ";" + kazamatakicsi[i].Item6);
                                }
                            }
                            if (kazamatanagy.Count > 0)
                            {
                                for (int i = 0; i < kazamatanagy.Count; i++)
                                {
                                    sb.AppendLine("uj kazamata;nagy;" + kazamatanagy[i].Item1 + ";" + kazamatanagy[i].Item2 + "," + kazamatanagy[i].Item3 + "," + kazamatanagy[i].Item4 + "," + kazamatanagy[i].Item5 + "," + kazamatanagy[i].Item6 + ";" + kazamatanagy[i].Item7);
                                }
                            }
                            if (kazamatamega.Count > 0)
                            {
                                for (int i = 0; i < kazamatamega.Count; i++)
                                {
                                    sb.AppendLine("uj kazamata;mega;" + kazamatamega[i].Item1 + ";" + kazamatamega[i].Item2 + "," + kazamatamega[i].Item3 + "," + kazamatamega[i].Item4 + "," + kazamatamega[i].Item5 + "," + kazamatamega[i].Item6 + ";" + kazamatamega[i].Item7 + "," + kazamatamega[i].Item8 + "," + kazamatamega[i].Item9 + ";" + kazamatamega[i].Item10);
                                }
                            }
                            sb.AppendLine();
                            sb.AppendLine("uj jatekos");
                            sb.AppendLine();
                            for (int i = 0; i < playercards.Count; i++)
                            {
                                sb.AppendLine("felvetel gyujtemenybe;" + playercards[i].Item1);
                            }
                            string SAVE = sb.ToString();
                            File.WriteAllText(fullPath, SAVE);

                            foreach (Control ctrl in this.Controls)
                            {
                                if (ctrl is Panel panel && panel.Visible)
                                {
                                    panel.Hide();
                                }
                            }
                            menu.Show();
                        }
                        ujra = false;
                    }
                }                
            }
        }

        private int AppDiff(int damage, int diff, int type)
        {
            Random rnd = new Random();
            double calculated;
            calculated = rnd.NextDouble();
            if (type == 0) {//katakomba
                calculated = calculated * diff / 10;
                calculated = 1 + calculated;
            }
            else //harcos
            {
                calculated = calculated * diff / 20;
                calculated = 1 - calculated;
            }
            calculated = calculated * damage;
            return Convert.ToInt32(Math.Round(calculated, MidpointRounding.AwayFromZero));
        }

        public void Világsoronként(StreamReader sr, string bemenet)
        {//kártya, vezér, kazamata, játékos, felvétel, pakli
            int kartyan = 0;
            int vezerkartyan = 0;
            int playercardsn = 0;
            bool ujjatakos = false;
            kartyak.Clear();
            playercards.Clear();
            vezerkartyak.Clear();
            kazamataegyszeru.Clear();
            kazamatakicsi.Clear();
            kazamatanagy.Clear();
            kazamatamega.Clear();
            Pakli.Clear();
            while (!sr.EndOfStream)
            {
                string sor = sr.ReadLine();
                if (sor == null || sor == "")
                {
                    continue;
                }
                string[] sorreszek = sor.Split(';');
                try
                {
                    if (!ujjatakos)
                    {
                        if (sorreszek[0] == "difficulty")
                        {
                            currentdifficulty = Convert.ToInt32(sorreszek[1]);
                        }
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
                                        vezerkartyak[vezerkartyan] = (sorreszek[1], kartyak[m].Item2 * 2, kartyak[m].Item3, kartyak[m].Item4, alapnev, "sebzes");
                                    }
                                    else if (sorreszek[3] == "eletero")
                                    {
                                        vezerkartyak[vezerkartyan] = (sorreszek[1], kartyak[m].Item2, kartyak[m].Item3 * 2, kartyak[m].Item4, alapnev, "eletero");
                                    }
                                    vezerkartyan++;
                                }
                            }
                        }
                        if (sorreszek[0] == "uj kazamata")
                        {
                            if (sorreszek[1] == "egyszeru")
                            {
                                kazamataegyszeru[kazamataegyszeru.Count] = (sorreszek[2], sorreszek[3], sorreszek[4]);
                            }
                            else if (sorreszek[1] == "kicsi")
                            {
                                string[] ellenfelek = sorreszek[3].Split(',');
                                kazamatakicsi[kazamatakicsi.Count] = (sorreszek[2], ellenfelek[0], ellenfelek[1], ellenfelek[2], sorreszek[4], sorreszek[5]);
                            }
                            else if (sorreszek[1] == "nagy")
                            {
                                string[] ellenfelek = sorreszek[3].Split(',');
                                kazamatanagy[kazamatanagy.Count] = (sorreszek[2], ellenfelek[0], ellenfelek[1], ellenfelek[2], ellenfelek[3], ellenfelek[4], sorreszek[4]);
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
                        if (bemenet == "con/con")
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
                        else
                        {
                            playercards[playercardsn] = (sorreszek[1], Convert.ToInt32(sorreszek[2]), Convert.ToInt32(sorreszek[3]), sorreszek[4]);
                            playercardsn++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hiba történt a fájl olvasása során!");
                    Console.WriteLine(ex);
                }
            }
        }

        private void MainScreen(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                string name = btn.Name;
                if (name == "backtomainscreenpakli")
                {
                    for (int i = 0; i < playercards.Count; i++)
                    {

                        Button button = kartyakscreen.Controls.OfType<Button>()
                            .FirstOrDefault(b => b.Name == playercards[i].Item1 + "kartyakpanelen");
                        if (button != null)
                        {
                            if (button.BackColor == Color.White)
                            {
                                Pakli.Add(playercards[i].Item1);


                            }
                        }



                    }
                    int a = Pakli.Count;
                }
                
            }

                kazmatascreen.Hide();
                kartyakscreen.Hide();
                playerscreen.Show();
                playerscreen.BringToFront();
                playerscreen.BackColor = Color.DimGray;

                button("Kártyák kezelése", "kartyakmenugomb", 200, 200, 150, 450, 20, playerscreen, Kartyagombok, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Harc⚔️", "harcmenugomb", 200, 200, 750, 450, 20, playerscreen, KazmataGombok, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Játék mentése", "SaveGame", 500, 75, 50, 50, 20, playerscreen, Save, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Kilépés a főmenüre", "quitnosave", 500, 75, 50, 150, 20, playerscreen, Save, Color.DarkGray, Color.Black, Color.DarkRed);
                button("Játék mentése és kilépés a főmenüre", "savenquit", 500, 75, 50, 250, 20, playerscreen, Save, Color.DarkGray, Color.Black, Color.DarkRed);
            
            
            
            
        }

        private void Kartyagombok(object sender, EventArgs e)
        {
            Pakli.Clear();
            playerscreen.Hide();
            if (!this.Controls.Contains(kartyakscreen)) this.Controls.Add(kartyakscreen);
            kartyakscreen.Show();
            kartyakscreen.BringToFront();
            kartyakscreen.BackColor = Color.DimGray;


            button("Vissza", "backtomainscreenpakli", 150, 75, 35, 745, 20, kartyakscreen, MainScreen, Color.DarkGray, Color.Black, Color.DarkRed);

            Color border = new Color();
            string tipisekezettel = string.Empty;


            int x = 10;
            int y = 40;
            label("Vezérek:", "info", 100, 30, 2, 7, 14, kartyakscreen, Color.Transparent, Color.Black);
            if (vezerkartyak.Count > 0)
            {

                for (int i = 0; i < vezerkartyak.Count; i++)
                {
                    if (vezerkartyak[i].Item4 == "tuz") { border = Color.Orange; tipisekezettel = "Tűz"; }
                    else if (vezerkartyak[i].Item4 == "viz") { border = Color.Blue; tipisekezettel = "VÍz"; }
                    else if (vezerkartyak[i].Item4 == "levego") { border = Color.LightBlue; tipisekezettel = "Levegő"; }
                    else if (vezerkartyak[i].Item4 == "fold") { border = Color.SaddleBrown; tipisekezettel = "Föld"; }
                    button(vezerkartyak[i].Item1 + Environment.NewLine + "⚔️" + vezerkartyak[i].Item2 + "/" + vezerkartyak[i].Item3 + "❤️" + Environment.NewLine + tipisekezettel, vezerkartyak[i].Item1 + "kartyakpanelen", 100, 130, x, y, 10, kartyakscreen, button_Click, Color.DarkGray, Color.Black, border);
                    x += 110;
                    if (x > 1000) { x = 10; y += 140; }

                    Button butn = kartyakscreen.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == vezerkartyak[i].Item1 + "kartyakpanelen");
                    if (butn != null)
                    {
                        butn.Enabled = false;
                    }
                }
            }
            if (x <= 1000)
            {
                x = 10;
                y += 137;
            }

            label("Kártyák:", "info", 94, 30, 3, y, 14, kartyakscreen, Color.Transparent, Color.Black);
            y += 37;
            for (int i = 0; i < kartyak.Count; i++)
            {
                if (kartyak[i].Item4 == "tuz") { border = Color.Orange; tipisekezettel = "Tűz"; }
                else if (kartyak[i].Item4 == "viz") { border = Color.Blue; tipisekezettel = "VÍz"; }
                else if (kartyak[i].Item4 == "levego") { border = Color.LightBlue; tipisekezettel = "Levegő"; }
                else if (kartyak[i].Item4 == "fold") { border = Color.SaddleBrown; tipisekezettel = "Föld"; }
                button(kartyak[i].Item1 + Environment.NewLine + "⚔️" + kartyak[i].Item2 + "/" + kartyak[i].Item3 + "❤️" + Environment.NewLine + tipisekezettel, kartyak[i].Item1 + "kartyakpanelen", 100, 130, x, y, 10, kartyakscreen, button_Click, Color.DarkGray, Color.Black, border);
                x += 110;
                if (x > 1000) { x = 10; y += 140; }

                Button butnn = kartyakscreen.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == kartyak[i].Item1 + "kartyakpanelen");
                if (butnn != null)
                {
                    butnn.Enabled = false;
                }
                for (int j=0; j < playercards.Count; j++)
                {
                    Button butn = kartyakscreen.Controls.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == playercards[j].Item1 + "kartyakpanelen");
                    if (butn != null && playercards[j].Item1 == kartyak[i].Item1)
                    {
                        butn.Enabled = true;
                    }
                    
                }
                
            }







            
            /*label("Paklid:", "info", 100, 30, 3, 450, 14, kartyakscreen, Color.DimGray, Color.DarkRed);
            label("Kártyáid:", "info", 94, 30, 5, 300, 14, kartyakscreen, Color.DimGray, Color.DarkRed);


            string infoText = "Kártyák adatai: név, sebzés/életerő, típus" + Environment.NewLine + "Pakli módosításához nyomd meg az alábbi kártyákat." + Environment.NewLine + "Pakli kiürétéséhez nyomd meg az új pakli gombot." + Environment.NewLine + "A pakliban gyűjteményednek legfeljebb fele szerepelhet.";
            label(infoText, "info", 400, 70, 302, 270, 10, kartyakscreen, Color.DimGray, Color.DarkRed);
           
            button("Vissza", "backtomainscreen", 100, 85, 900, 260, 14, kartyakscreen, MainScreen, Color.DimGray, Color.DarkRed, Color.Black);

            

            int x = 10;
            foreach (int i in kartyak.Keys)
            {
                string kartyaText = kartyak[i].Item1 + Environment.NewLine + kartyak[i].Item2 + "/" + kartyak[i].Item3 + Environment.NewLine + tipusok[kartyak[i].Item4];
                label(kartyaText, "kartya" + i.ToString(), 100, 100, x, 40, 10, kartyakscreen, Color.DimGray, Color.DarkRed);
                x = x + 100;
            }
            x = 10;
            label("Vezérek:", "info", 100, 30, 10, 140, 14, kartyakscreen, Color.DimGray, Color.DarkRed);
            foreach (int i in vezerkartyak.Keys)
            {
                string vezérText = vezerkartyak[i].Item1 + Environment.NewLine + vezerkartyak[i].Item2 + "/" + vezerkartyak[i].Item3 + Environment.NewLine + tipusok[vezerkartyak[i].Item4];
                label(vezérText, "vezerek" + i.ToString(), 100, 100, x, 170, 10, kartyakscreen, Color.DimGray, Color.DarkRed);
                x = x + 100;
            }

            x = 10;
            label("Kártyáid:", "info", 94, 30, 10, 270, 14, kartyakscreen, Color.DimGray, Color.DarkRed);
            foreach (int i in playercards.Keys)
            {
                string gyujtemenyText = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + tipusok[playercards[i].Item4];
                button(gyujtemenyText, "gyujtemeny" + i.ToString(), 85, 100, x, 340, 8, kartyakscreen, Button_Click, Color.DimGray, Color.DarkRed, Color.Black);
                x = x + 99;
            }*/
            kartyakscreen.Show();
            kartyakscreen.Dock = DockStyle.Fill;
            kartyakscreen.BringToFront();
            kartyakscreen.BackColor = Color.DimGray;
        }

        List<int> paklint = new List<int>();
        /*private void ÚjPakli_Click(object sender, EventArgs e)
        {
            Pakli.Clear();
            paklint.Clear();
            paklix = 5;
            foreach (Button btn in kartyakscreen.Controls.OfType<Button>())
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
            foreach (var btn in kartyakscreen.Controls.OfType<Button>().Where(b => b.Name.StartsWith("paklibtn")).ToList())
            {
                kartyakscreen.Controls.Remove(btn);
                btn.Dispose();
            }
            foreach (var btn in kartyakscreen.Controls.OfType<Button>())
            {
                if (btn.Name.StartsWith("kazmgomb"))
                {
                    btn.Enabled = false;
                }
            }
        }*/

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
                felekerint = (playercards.Count + 1) / 2;
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
                else if (clicked.Name.Length == 12)
                {
                    nmb = int.Parse(clicked.Name.Substring(clicked.Name.Length - 2));
                }
                else
                {
                    nmb = int.Parse(clicked.Name.Substring(clicked.Name.Length - 3));
                }
                paklint.Add(nmb);
                string buttonText = playercards[nmb].Item1.ToString() + "\r\n" + playercards[nmb].Item2.ToString() + "/" + playercards[nmb].Item3.ToString() + "\r\n" + tipusok[playercards[nmb].Item4.ToString()];
                button(buttonText, "paklibtn" + nmb, 85, 100, paklix, 490, 8, kartyakscreen, PakliClick, Color.Black, Color.White, Color.Black);
                paklix += 99;
                Pakli.Add(playercards[nmb].Item1);
                foreach (var btn in kartyakscreen.Controls.OfType<Button>())
                {
                    if (btn.Name.StartsWith("kazmgomb"))
                    {
                        btn.Enabled = true;
                    }
                }
            }
            if (Pakli.Count == felekerint)
            {
                foreach (var btn in kartyakscreen.Controls.OfType<Button>().Where(b => b.Name.StartsWith("gyujtemeny")).ToList())
                {
                    btn.Enabled = false;
                }
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
            foreach (var btn in kartyakscreen.Controls.OfType<Button>())
            {
                if (btn.Location.Y == button.Location.Y && btn.Location.X > button.Location.X)
                {
                    btn.Location = new Point(btn.Location.X - 99, btn.Location.Y);
                }
                else if (btn.Name == "gyujtemeny" + last)
                {
                    btn.Enabled = true;
                }
                if ((btn.Name.StartsWith("kazmgomb") && Pakli.Count == 0))
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
                    Control[] found = kartyakscreen.Controls.Find(buttonName, true);
                    if (found.Length > 0 && found[0] is Button btn)
                    {
                        btn.Enabled = true;
                    }
                }
            }
        }

        private void KazmataGombok(object sender, EventArgs E)
        {
            if (!this.Controls.Contains(kazmatascreen)) this.Controls.Add(kazmatascreen);
            kazmatascreen.Show();
            kazmatascreen.Dock = DockStyle.Fill;
            kazmatascreen.BringToFront();
            kazmatascreen.BackColor = Color.DimGray;
            playerscreen.Hide();
            int x = 10;
            int y = 30;
            int counter = 0;
            label("Kazamaták:", "info", 110, 30, 10, 2, 14, kazmatascreen, Color.DimGray, Color.Black);
            button("Vissza","back",100,30,150,2,11,kazmatascreen,MainScreen,Color.DarkGray,Color.Black,Color.DarkRed);

            foreach (int i in kazamataegyszeru.Keys)
            {
                counter++;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = "kazmgomb1";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = kazamataegyszeru[i].Item1;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, y);
                if (Pakli.Count == 0) lbl.Enabled = false;
                lbl.BackColor = Color.DarkGray;
                lbl.ForeColor = Color.Black;
                lbl.FlatAppearance.BorderSize = 1;
                lbl.FlatAppearance.BorderColor = Color.DarkRed;
                lbl.Click += (s, e) => SelectBuffsScreen(i,"egyszeru");
                kazmatascreen.Controls.Add(lbl);
                x = x + 100;

                label(kazamataegyszeru[i].Item2, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);

                x += 100;
                label("Jutalom: " + kazamataegyszeru[i].Item3, "kazmjut", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label("Típus: egyszerű", "info", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
            }
            y += 80;
            foreach (int i in kazamatakicsi.Keys)
            {
                counter++;
                x = 10;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = "kazmgomb2";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = kazamatakicsi[i].Item1;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, y);
                if (Pakli.Count == 0) lbl.Enabled = false;
                lbl.BackColor = Color.DarkGray;
                lbl.ForeColor = Color.Black;
                lbl.FlatAppearance.BorderSize = 1;
                lbl.FlatAppearance.BorderColor = Color.DarkRed;
                lbl.Click += (s, e) => SelectBuffsScreen(i,"kicsi");
                kazmatascreen.Controls.Add(lbl);
                x = x + 99;

                System.Windows.Forms.Label ell1 = new System.Windows.Forms.Label();
                label(kazamatakicsi[i].Item2, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 99;
                label(kazamatakicsi[i].Item3, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 99;
                label(kazamatakicsi[i].Item4, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 99;
                label("Vezér:\n" + kazamatakicsi[i].Item5, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 99;
                label("Jutalom: " + kazamatakicsi[i].Item6, "kazmjut", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 99;
                label("Típus: kicsi", "info", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
            }
            y += 80;
            foreach (int i in kazamatanagy.Keys)
            {
                counter++;
                x = 10;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = "kazmgomb3";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = kazamatanagy[i].Item1;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, y);
                if (Pakli.Count == 0) lbl.Enabled = false;
                lbl.BackColor = Color.DarkGray;
                lbl.ForeColor = Color.Black;
                lbl.FlatAppearance.BorderSize = 1;
                lbl.FlatAppearance.BorderColor = Color.DarkRed;
                lbl.Click += (s, e) => SelectBuffsScreen(i,"nagy");
                kazmatascreen.Controls.Add(lbl);
                x = x + 100;

                label(kazamatanagy[i].Item2, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label(kazamatanagy[i].Item3, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.  Black);
                x += 100;
                label(kazamatanagy[i].Item4, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label(kazamatanagy[i].Item5, "kazmell", 85, 50, x, y,10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label(kazamatanagy[i].Item6, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label("Vezér:\n" + kazamatanagy[i].Item7, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label("Típus: nagy", "kazminfo", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
            }
            y += 80;
            foreach (int i in kazamatamega.Keys)
            {
                counter++;
                x = 10;
                System.Windows.Forms.Button lbl = new System.Windows.Forms.Button();
                lbl.Name = "kazmgomb4";
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = kazamatamega[i].Item1;
                lbl.Size = new Size(85, 50);
                lbl.Location = new Point(x, y);
                if (Pakli.Count == 0) lbl.Enabled = false;
                lbl.BackColor = Color.DarkGray;
                lbl.ForeColor = Color.Black;
                lbl.FlatAppearance.BorderSize = 3;
                lbl.FlatAppearance.BorderColor = Color.DarkRed;
                lbl.Click += (s, e) => SelectBuffsScreen(i,"mega");
                kazmatascreen.Controls.Add(lbl);
                x = x + 100;
                label(kazamatamega[i].Item2, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label(kazamatamega[i].Item3, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label(kazamatamega[i].Item4, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label(kazamatamega[i].Item5, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label(kazamatamega[i].Item6, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label("Vezér:\n" + kazamatamega[i].Item7, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label("Vezér:\n" + kazamatamega[i].Item8, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label("Vezér:\n" + kazamatamega[i].Item9, "kazmell", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label("Jutalom: " + kazamatamega[i].Item10, "kazmjut", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
                x += 100;
                label("Típus: mega", "kazminfo", 85, 50, x, y, 10, kazmatascreen, Color.DimGray, Color.Black);
            }
        }

        private void mentes()
        {            
            //default env + stats for player cards+difficulty number
            StringBuilder sb = new StringBuilder();
            foreach (int i in kartyak.Keys)
            {
                sb.Append("uj kartya;").Append(kartyak[i].Item1 + ";" + kartyak[i].Item2.ToString() + ";" + kartyak[i].Item3.ToString() + ";" + kartyak[i].Item4).Append(Environment.NewLine);
            }
            foreach (int i in vezerkartyak.Keys)
            {
                sb.Append("uj vezer;").Append(vezerkartyak[i].Item1 + ";" + vezerkartyak[i].Item5 + ";" + vezerkartyak[i].Item6).Append(Environment.NewLine);
            }
            foreach (int i in kazamataegyszeru.Keys)
            {
                sb.Append("uj kazamata;egyszeru;").Append(kazamataegyszeru[i].Item1 + ";" + kazamataegyszeru[i].Item2 + ";" + kazamataegyszeru[i].Item3).Append(Environment.NewLine);
            }
            foreach (int i in kazamatakicsi.Keys)
            {
                sb.Append("uj kazamata;kicsi;").Append(kazamatakicsi[i].Item1 + ";" + kazamatakicsi[i].Item2 + "," + kazamatakicsi[i].Item3 + "," + kazamatakicsi[i].Item4 + ";" + kazamatakicsi[i].Item5 + ";" + kazamatakicsi[i].Item6).Append(Environment.NewLine);
            }
            foreach (int i in kazamatanagy.Keys)
            {
                sb.Append("uj kazamata;nagy;").Append(kazamatanagy[i].Item1 + ";" + kazamatanagy[i].Item2 + "," + kazamatanagy[i].Item3 + "," + kazamatanagy[i].Item4 + "," + kazamatanagy[i].Item5 + "," + kazamatanagy[i].Item6 + ";" + kazamatanagy[i].Item7).Append(Environment.NewLine);
            }
            foreach (int i in kazamatamega.Keys)
            {
                sb.Append("uj kazamata;mega;").Append(kazamatamega[i].Item1 + ";" + kazamatamega[i].Item2 + "," + kazamatamega[i].Item3 + "," + kazamatamega[i].Item4 + "," + kazamatamega[i].Item5 + "," + kazamatamega[i].Item6 + ";" + kazamatamega[i].Item7 + "," + kazamatamega[i].Item8 + "," + kazamatamega[i].Item9 + ";" + kazamatamega[i].Item10).Append(Environment.NewLine);
            }
            sb.Append("uj jatekos").Append(Environment.NewLine);
            foreach (int i in playercards.Keys)
            {
                sb.Append("felvetel gyujtemenybe;").Append(playercards[i].Item1 + ";" + playercards[i].Item2.ToString() + ";" + playercards[i].Item3.ToString() + ";" + playercards[i].Item4).Append(Environment.NewLine);
            }
            string SAVE = "difficulty;" + currentdifficulty.ToString() + Environment.NewLine + sb.ToString();

            if (loadedPath.EndsWith(".gamedefaultsave"))
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "Válszad ki a mentés helyét! \nFájl neve: " + savename + ".gamesave";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = Path.Combine(dialog.SelectedPath, savename + ".gamesave");
                    File.WriteAllText(filePath, SAVE);
                }
            }
            else
            {
                File.WriteAllText(loadedPath, SAVE);
            }            
        }

        private void Save(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Name == "SaveGame")
            {
                mentes();
            }
            else if (btn != null && btn.Name == "savenquit")
            {
                mentes();
                menuu();
            }
            else if (btn != null && btn.Name == "quitnosave")
            {
                menuu();
            }
        }

        private void SelectBuffsScreen(int key, string típus)
        {
            int x = 5;
            if (powerupcount == 0)
            {
                Dictionary<string, string> Pairs = new Dictionary<string, string>();
                Harc(key, típus, Pairs);
            }
            else
            {
                buffselect.Show();
                buffselect.BackColor = Color.LightGray;
                buffselect.Dock = DockStyle.Fill;
                this.Controls.Add(buffselect);
                kazmatascreen.Hide();
                buffselect.BringToFront();

                label("Válaszd ki a kártyákat, amiket erősíteni szeretnél a körre\nPiros: sebzés duplázása, Kék: életerő duplázása\nErősítések száma: " + powerupcount, "info", 500,60,5,5,12,buffselect,Color.White, Color.Black);
                foreach (string s in Pakli)
                {
                    foreach (int i in playercards.Keys)
                    {
                        if (s == playercards[i].Item1) {
                            string gyujtemenyText = playercards[i].Item1 + '\n' + playercards[i].Item2 + "/" + playercards[i].Item3 + '\n' + tipusok[playercards[i].Item4];
                            button(gyujtemenyText, s, 85, 100, x, 100, 8, buffselect, SelectBuff, Color.LightGray,Color.Black,Color.Black);
                            x = x + 99;
                            break;
                        }
                    }
                }

                Button start = new Button();
                start.Size = new System.Drawing.Size(100, 80);
                start.Location = new Point(5, 200);
                start.Text = "Harc indítása";
                start.Name = "indít";
                start.Font = new Font("Microsoft Sans Seriff", 14);
                start.Click += (s,e) => HarcIndítás(key,típus);
                buffselect.Controls.Add(start);
            }
        }

        private void SelectBuff(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            string[] lines = btn.Text.Split('\n');
            string[] numbs = lines[1].Split('/');
            int damage = Convert.ToInt32(numbs[0]);
            int health = Convert.ToInt32(numbs[1]);
            if (powerupcount > 0 && btn.BackColor == Color.LightGray)
            {
                btn.BackColor = Color.Red;
                btn.Text = lines[0] + '\n' + (damage * 2).ToString() + '/' + health.ToString() + '\n' + lines[2];
                powerupcount--;
            }
            else if (btn.BackColor == Color.Red)
            {
                btn.BackColor = Color.Blue;
                btn.Text = lines[0] + '\n' + (damage / 2).ToString() + '/' + (health * 2).ToString() + '\n' + lines[2];
            }
            else if (btn.BackColor == Color.Blue)
            {
                btn.BackColor = Color.LightGray;
                btn.Text = lines[0] + '\n' + damage.ToString() + '/' + (health / 2).ToString() + '\n' + lines[2];
                powerupcount++;
            }
        }

        private void HarcIndítás(int key, string típus)
        {
            Dictionary<string, string> Pairs = new Dictionary<string, string>();

            foreach (Control c in buffselect.Controls)
            {
                if (c is Button btn)
                {
                    if (btn.BackColor == Color.Red)
                    {
                        Pairs.Add(btn.Name,"sebzes");
                        Console.WriteLine(btn.Name);
                    }
                    else if (btn.BackColor == Color.Blue)
                    {
                        Pairs.Add(btn.Name,"eletero");
                        Console.WriteLine(btn.Name);
                    }
                }
                c.Dispose();
            }
            this.Controls.Remove(buffselect);
            Harc(key, típus, Pairs);
        }

        private void Harc(int key, string típus, Dictionary<string,string> buffs)
        {
            foreach (string s in buffs.Keys) {
                Console.WriteLine(s+ " " + buffs[s]);
            }
            Panel harctér = new Panel();
            harctér.BackColor = Color.LightGray;
            harctér.Dock = DockStyle.Fill;
            this.Controls.Add(harctér);
            playerscreen.Hide();
            buffselect.Hide();
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
            Queue<string> vezer = new Queue<string>();

            int Xh = 0;
            int Xe = 70;
            int Y = 50;

            if (típus == "egyszeru")
            {
                simaellenfelek.Enqueue(kazamataegyszeru[key].Item2);
                jutalom = kazamataegyszeru[key].Item3;
            }
            else if (típus == "kicsi")
            {
                simaellenfelek.Enqueue(kazamatakicsi[key].Item2);
                simaellenfelek.Enqueue(kazamatakicsi[key].Item3);
                simaellenfelek.Enqueue(kazamatakicsi[key].Item4);
                vezer.Enqueue(kazamatakicsi[key].Item5);
                jutalom = kazamatakicsi[key].Item6;
            }
            else if (típus == "nagy")
            {
                simaellenfelek.Enqueue(kazamatanagy[key].Item2);
                simaellenfelek.Enqueue(kazamatanagy[key].Item3);
                simaellenfelek.Enqueue(kazamatanagy[key].Item4);
                simaellenfelek.Enqueue(kazamatanagy[key].Item5);
                simaellenfelek.Enqueue(kazamatanagy[key].Item6);
                vezer.Enqueue(kazamatanagy[key].Item7);
                jutalom = "powerup";
            }
            else if (típus == "mega")
            {
                simaellenfelek.Enqueue(kazamatamega[key].Item2);
                simaellenfelek.Enqueue(kazamatamega[key].Item3);
                simaellenfelek.Enqueue(kazamatamega[key].Item4);
                simaellenfelek.Enqueue(kazamatamega[key].Item5);
                simaellenfelek.Enqueue(kazamatamega[key].Item6);
                vezer.Enqueue(kazamatamega[key].Item7);
                vezer.Enqueue(kazamatamega[key].Item8);
                vezer.Enqueue(kazamatamega[key].Item9);
                jutalom = kazamatamega[key].Item10;

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
                    string labelText = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                    label(labelText,"jellenfél",70,50,Xe,Y,8,harctér,Color.LightGray,Color.Black);
                }
            }

            jharcos = harcosok.Dequeue();
            foreach (int i in playercards.Keys)
            {
                if (playercards[i].Item1 == jharcos)
                {
                    if (buffs.ContainsKey(jharcos))
                    {
                        Console.WriteLine(jharcos);
                        if (buffs[jharcos] == "sebzes")
                        {
                            jsebzésh = playercards[i].Item2 * 2;
                            jéleth = playercards[i].Item3;
                        }
                        else
                        {
                            jéleth = playercards[i].Item3 * 2;
                            jsebzésh = playercards[i].Item2;
                        }
                    }
                    else
                    {
                        jéleth = playercards[i].Item3;
                        jsebzésh = playercards[i].Item2;
                    }
                    jtípush = playercards[i].Item4;
                    string labelText = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                    label(labelText,"jharcos",70,50,Xh,Y,8,harctér, Color.LightGray, Color.Black);
                    break;
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

                if (jellenfél == "") //kazamata uj kartyat hoz be
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
                                string labelText = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                                label(labelText, "jellenfél", 70, 50, Xe, Y, 8, harctér, Color.LightGray, Color.Black);

                                string label1Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                                label(label1Text, "jharcos", 70, 50, Xh, Y, 8, harctér, Color.LightGray, Color.Black);

                                if (Y > this.Size.Height)
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
                        if (vezer.Count > 0)
                        {
                            jellenfél = vezer.Dequeue();
                            foreach (int i in vezerkartyak.Keys)
                            {
                                if (vezerkartyak[i].Item1 == jellenfél)
                                {
                                    jélete = vezerkartyak[i].Item3;
                                    jsebzése = vezerkartyak[i].Item2;
                                    jtípuse = vezerkartyak[i].Item4;

                                    string labelText = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                                    label(labelText, "jellenfél", 70, 50, Xe, Y, 8, harctér, Color.LightGray, Color.Black);

                                    string label1Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                                    label(label1Text, "jharcos", 70, 50, Xh, Y, 8, harctér, Color.LightGray, Color.Black);

                                    if (Y > this.Size.Height)
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
                        else break;
                    }
                }
                else if (jellenfél != "" && jharcos != "") //kazamata támad, kivel, mennyi sebzés (típussal), kire, mennyi élet marad
                {
                    if (jtípuse == jtípush)
                    {
                        jéleth -= AppDiff(jsebzése,currentdifficulty,0);
                    }
                    else if ((tipusok[jtípuse] == "Tűz" && tipusok[jtípush] == "Levegő") || (tipusok[jtípuse] == "Levegő" && tipusok[jtípush] == "Tűz") || (tipusok[jtípuse] == "Föld" && tipusok[jtípush] == "Víz") || (tipusok[jtípuse] == "Víz" && tipusok[jtípush] == "Föld"))
                    {
                        jéleth -= AppDiff(jsebzése, currentdifficulty, 0) / 2;
                    }
                    else
                    {
                        jéleth -= AppDiff(jsebzése, currentdifficulty, 0) * 2;
                    }

                    if (jéleth < 0)
                    {
                        jéleth = 0;
                    }

                    string labelText = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                    label(labelText, "jellenfél", 70, 50, Xe, Y, 8, harctér, Color.LightGray, Color.Black);

                    string label1Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                    label(label1Text, "jharcos", 70, 50, Xh, Y, 8, harctér, Color.LightGray, Color.Black);

                    if (Y > this.Size.Height)
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

                if (jharcos == "") //játékos hoz elo kartyat
                {
                    if (harcosok.Count > 0)
                    {
                        jharcos = harcosok.Dequeue();
                        foreach (int i in playercards.Keys)
                        {
                            if (playercards[i].Item1 == jharcos)
                            {
                                if (buffs.ContainsKey(jharcos))
                                {
                                    Console.WriteLine(jharcos);
                                    if (buffs[jharcos] == "sebzes")
                                    {
                                        jsebzésh = playercards[i].Item2 * 2;
                                        jéleth = playercards[i].Item3;
                                    }
                                    else
                                    {
                                        jéleth = playercards[i].Item3 * 2;
                                        jsebzésh = playercards[i].Item2;
                                    }
                                }
                                else
                                {
                                    jéleth = playercards[i].Item3;
                                    jsebzésh = playercards[i].Item2;
                                }
                                jtípush = playercards[i].Item4;

                                string labelText = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                                label(labelText, "jharcos", 70, 50, Xh, Y, 8, harctér, Color.LightGray, Color.Black);

                                string label1Text = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                                label(label1Text, "jellenfél", 70, 50, Xe, Y, 8, harctér, Color.LightGray, Color.Black);


                                if (Y > this.Size.Height)
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
                        jélete -= AppDiff(jsebzésh,currentdifficulty,1);
                    }
                    else if ((tipusok[jtípuse] == "Tűz" && tipusok[jtípush] == "Levegő") || (tipusok[jtípuse] == "Levegő" && tipusok[jtípush] == "Tűz") || (tipusok[jtípuse] == "Föld" && tipusok[jtípush] == "Víz") || (tipusok[jtípuse] == "Víz" && tipusok[jtípush] == "Föld"))
                    {
                        jélete -= AppDiff(jsebzésh, currentdifficulty, 1) / 2;
                    }
                    else
                    { 
                        jélete -= AppDiff(jsebzésh, currentdifficulty, 1) * 2;
                    }

                    if (jélete < 0)
                    {
                        jélete = 0;
                    }

                    string labelText = jellenfél + Environment.NewLine + jsebzése + "/" + jélete + Environment.NewLine + jtípuse;
                    label(labelText, "jellenfél", 70, 50, Xe, Y, 8, harctér, Color.LightGray, Color.Black);


                    string label1Text = jharcos + Environment.NewLine + jsebzésh + "/" + jéleth + Environment.NewLine + jtípush;
                    label(label1Text, "jharcos", 70, 50, Xh, Y, 8, harctér, Color.LightGray, Color.Black);


                    if (Y > this.Size.Height)
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
            back.Click += (s, e) => { this.Controls.Remove(harctér);playerscreen.Show(); };
            harctér.Controls.Add(back);

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
                if (jutalom == "kartya" && kartyak.Count != playercards.Count)
                {
                    int j = 0;
                    foreach (int k in kartyak.Keys)
                    {
                        foreach (int p in playercards.Keys)
                        {
                            if (playercards[p].Item1 == kartyak[k].Item1)
                            {
                                j++;
                                break;
                            }
                        }
                        if (j == 0)
                        {
                            playercards[playercards.Count] = (kartyak[k].Item1, kartyak[k].Item2, kartyak[k].Item3, kartyak[k].Item4);
                            nyeremeny.Text = "Játékos nyert!\nNyeremény: " + kartyak[k].Item1;
                            break;
                        }
                        else {
                            j = 0;
                        }
                    }

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
                            Control[] found = kartyakscreen.Controls.Find(buttonName, true);
                            if (found.Length > 0 && found[0] is Button btn)
                            {
                                btn.Enabled = true;
                            }
                        }
                    }


                }
                else if (jutalom.ToLower() == "sebzés" || jutalom.ToLower() == "életerő" || jutalom.ToLower() == "sebzes" || jutalom.ToLower() == "eletero")
                {
                    foreach (int i in playercards.Keys)
                    {
                        if (playercards[i].Item1 == jharcos)
                        {
                            if (jutalom.ToLower() == "sebzés" || jutalom.ToLower() == "sebzes")
                            {
                                playercards[i] = (playercards[i].Item1, playercards[i].Item2 + 1, playercards[i].Item3, playercards[i].Item4);
                                nyeremeny.Text = "Játékos nyert" + Environment.NewLine + "+1 sebzés: " + playercards[i].Item1;
                            }
                            else if (jutalom.ToLower() == "életerő" || jutalom.ToLower() == "eletero")
                            {
                                playercards[i] = (playercards[i].Item1, playercards[i].Item2, playercards[i].Item3 + 2, playercards[i].Item4);
                                nyeremeny.Text = "Játékos nyert" + Environment.NewLine + "+2 élet: " + playercards[i].Item1;
                            }
                            string buttonName = "gyujtemeny" + i;
                            Control[] found = kartyakscreen.Controls.Find(buttonName, true);
                            if (found.Length > 0 && found[0] is Button btn)
                            {
                                btn.Text = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + playercards[i].Item4;
                            }
                            string buttonName1 = "paklibtn" + i;
                            Control[] found1 = kartyakscreen.Controls.Find(buttonName1, true);
                            if (found1.Length > 0 && found1[0] is Button btn1)
                            {
                                btn1.Text = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + playercards[i].Item4;
                            }
                            break;
                        }
                    }
                }
                else if (jutalom == "powerup")
                {
                    powerupcount += 1;
                    nyeremeny.Text = "Játékos nyert "+Environment.NewLine+"Választható erősítés";
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        /*elso fordulo*/
        /*public Form1()
                {
                    InitializeComponent();

                    //elsofordulo
                    kartyaklbl();
                    Playerlabel();
                    kartyaidbuttons();
                    Paklilabel();
                    vezerek();
                    KazmataGombok();
                }
                int paklix = 5; //pakliba levo kartyak elhelyezesehez kell
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
                /*
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
                            (playercards[10].Item1);

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
                }*/
    }
}
