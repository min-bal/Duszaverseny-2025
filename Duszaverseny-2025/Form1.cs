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
            {1, ("Priestess Selia", 2, 12, "Föld", "(Vezér)") }
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
            {"Osi Szentely", ("Arin", "Emera", "Selia", "Lord Torak", "életerő") }
        }; 
        Dictionary<string, (string, string, string, string, string, string)> kazamatanagy = new Dictionary<string, (string, string, string, string, string, string)> //név, ellenfél*5, vezér
        {
            {"A melyseg kiralynoje", ("Liora", "Arin", "Selia", "Nerun", "Torak", "Priestess Selia") }
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
                lbl.Name = "kartyalbl" + i.ToString();
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
            info2lbl.Location = new Point(2, 150);
            info2lbl.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(info2lbl);
            foreach (int i in playercards.Keys) { 
                System.Windows.Forms.Label lbl = new System.Windows.Forms.Label();
                lbl.Name = "playerkartyalbl" + i.ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.BorderStyle = BorderStyle.FixedSingle;
                lbl.Text = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + playercards[i].Item4;
                lbl.Size = new Size(85, 100);
                lbl.Location = new Point(x, 180);
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
            info3lbl.Location = new Point(3, 300);
            info3lbl.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(info3lbl);
            foreach (string nev in Pakli) {
                foreach (int i in playercards.Keys)
                {
                    System.Windows.Forms.Label lbl = new System.Windows.Forms.Label();
                    lbl.Name = "paklilbl" + nev.ToString();
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                    lbl.Text = playercards[i].Item1 + Environment.NewLine + playercards[i].Item2 + "/" + playercards[i].Item3 + Environment.NewLine + playercards[i].Item4;
                    lbl.Size = new Size(85, 100);
                    lbl.Location = new Point(x, 330);
                    this.Controls.Add(lbl);
                    x = x + 99;
                }
            }
        }


        public Form1()
        {
            InitializeComponent();
            kartyaklbl();
            Playerlabel();
            Paklilabel();
        }

        private void ÚjPakli_Click(object sender, EventArgs e)
        {

        }
    }
}
