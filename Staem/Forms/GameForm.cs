﻿using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Staem.Forms
{
    public partial class GameForm : Form
    {
        Game game;
        User user;

        int currentN = 0, maxN = 0;
        string temp, hry;

        List<string> poleHier = new List<string>();

        PictureBox nahladHry;
        Button kupit;

        PrivateFontCollection font = new PrivateFontCollection();
        string fontPath = Path.GetFullPath("BrunoAceSC.ttf");

        public GameForm(Game game, User user)
        {
            InitializeComponent();

            this.game = game;   
            this.user = user;

            this.AutoScaleMode = AutoScaleMode.None;

            font.AddFontFile(fontPath);

            foreach (Control c in this.Controls)
            {
                c.Font = new Font(font.Families[0], 16, FontStyle.Regular);
            }


            //zaciatok vypisovania
            currentN = 0;

            // nacita nahladove obrazky do listu (ked som to skusal v metode tak pisalo chybu ArgumentOutOfRangeException, ale neviem preco)
            List<string> nahladoveObrazky = new List<string>();
            Database.dbConnect();
            MySqlCommand cmd = new MySqlCommand($"SELECT nahladObrazok FROM games WHERE nazov = '{game.Name}'", Database.connection);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                temp = reader["nahladObrazok"].ToString();
            }
            Database.dbClose();

            nahladoveObrazky = temp.Split(';').ToList();
            temp = "";
            maxN = nahladoveObrazky.Count;



            //tieto veci pridavaju nove controls
            Panel hlavnyPanel = new Panel
            {
                Location = new Point((Screen.PrimaryScreen.Bounds.Width / 2) - (840 / 2) + 200, 100),
                AutoSize = true,
                MaximumSize = new Size(840, 0),
                BackColor = Color.FromArgb(38, 62, 85),
                Cursor = Cursors.Default,
                Padding = new Padding(0,0,0,20)
            };

            Label nazovHry = new Label
            {
                Location = new Point(100, 50),
                AutoSize = true,
                ForeColor = Color.White,
                Text = game.Name,
                Font = new Font(font.Families[0], 20, FontStyle.Bold)
            };

            Label vyvojar = new Label
            {
                Location = new Point(100, 410),
                AutoSize = true,
                ForeColor = Color.White,
                Text = game.Developer,
                Font = new Font(font.Families[0], 15, FontStyle.Italic)
            };

            Label hraKategoria = new Label
            {
                Location = new Point(100, 450),
                AutoSize = true,
                ForeColor = Color.White,
                Text = game.Category,
                Font = new Font(font.Families[0], 15, FontStyle.Regular)
            };

            nahladHry = new PictureBox
            {
                Image = Image.FromFile("obrazky/nahlady/" + nahladoveObrazky[0]),
                Location = new Point(100, 100),
                Size = new Size(600, 300),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // buttony na prepinanie nahladovych obrazkov
            TransparentButton next = new TransparentButton
            {
                Size = new Size(72, 72),
                Location = new Point(600 - 72, 150 - (72 / 2)),
                Image = Image.FromFile("obrazky/next.png"),
            };

            TransparentButton back = new TransparentButton
            {
                Size = new Size(72, 72),
                Location = new Point(0, 150 - (72 / 2)),
                Image = Image.FromFile("obrazky/prev.png"),
            };
            // buttony na prepinanie nahladovych obrazkov

            Label popis = new Label
            {
                Location = new Point(20, 20),
                AutoSize = true,
                MaximumSize = new Size(800, 0),
                ForeColor = Color.Aqua,
                Text = game.Description,
                Font = new Font(font.Families[0], 13, FontStyle.Regular)
            };

            Controls.Add(popis);
            Size labelSize = popis.GetPreferredSize(Size.Empty);

            kupit = new Button
            {
                AutoSize = true,
                Location = new Point(600 - 25, 488),
                ForeColor = Color.Aqua,
                Text = game.Price,
                Cursor = Cursors.Hand,
                Font = new Font(font.Families[0], 13, FontStyle.Regular),
                BackColor = Color.FromArgb(38, 62, 85),
            };

            if (game.Price != "Zadarmo")
            {
                kupit.Text = $"{game.Price} €";
            }

            //divoky zapis na passnutie argumentu do event handleru
            kupit.Click += (sender, e) => kupit_Click(game);

            // nechytat sa toho, lebo takto to funguje
            Controls.Add(nazovHry);
            Controls.Add(vyvojar);

            nahladHry.Controls.Add(next);
            next.Click += (sender, e) => next_Click(nahladoveObrazky);
            nahladHry.Controls.Add(back);
            back.Click += (sender, e) => back_Click(nahladoveObrazky);

            hlavnyPanel.Controls.Add(popis);

            Controls.Add(kupit);
            Controls.Add(hraKategoria);
            Controls.Add(nahladHry);
            Controls.Add(hlavnyPanel);

            
            MySqlCommand cmd1 = new MySqlCommand($"SELECT hry FROM users WHERE email = '{user.Email}';", Database.connection);
            Database.dbConnect();
            MySqlDataReader reader1 = cmd1.ExecuteReader();

            while (reader1.Read())
            {
                hry = reader1["hry"].ToString();
            }
            Database.dbClose();

            poleHier = hry.Split(';').ToList();

            poleHier.RemoveAt(poleHier.Count - 1);

            if (poleHier != null)
            {               
                foreach (string item in poleHier)
                {
                    if (item == game.Name)
                    {
                        kupit.Enabled = false;
                        kupit.Text = "Zakúpené";
                    }
                }
            }
        }

        private void next_Click(List<string> nahladoveObrazky)
        {
            currentN++;

            if (currentN > 1)
            {
                currentN = 1;
            }
            else if (currentN < maxN)
            {
                nahladHry.Image = Image.FromFile("obrazky/nahlady/" + nahladoveObrazky[currentN]);
            }
        }

        private void back_Click(List<string> nahladoveObrazky)
        {
            currentN--;

            if (currentN < 0)
            {
                currentN = 0;
            }
            else if (currentN < maxN)
            {
                nahladHry.Image = Image.FromFile("obrazky/nahlady/" + nahladoveObrazky[currentN]);
            }
        }

        //nechytat
        private void kupit_Click(Game game)
        {
            float balance;
            string temp = "";

            Database.dbConnect();
            MySqlCommand sql = new MySqlCommand($"SELECT balance FROM users WHERE email='{user.Email}';", Database.connection);
            MySqlDataReader reader = sql.ExecuteReader();

            while (reader.Read())
            {
                temp = reader["balance"].ToString();
            }

            Database.dbClose();

            balance = float.Parse(temp);
            
            if(game.Price != "Zadarmo")
            {
                if (balance < float.Parse(game.Price))
                {
                    MessageBox.Show("Máš málo peňazí");
                    return;
                }
            }
                

            //vdaka tomuto pridavam do databazy hry
            hry += $"{game.Name};";

            if(game.Price != "Zadarmo")
            {
                balance -= float.Parse(game.Price);
            }
            
            temp = balance.ToString("#.##");

            Database.dbConnect();
            MySqlCommand cmd1 = new MySqlCommand($"UPDATE users SET hry = '{hry}', balance = {temp} WHERE email = '{user.Email}';", Database.connection);
            cmd1.ExecuteNonQuery();
            Database.dbClose();

            Database.dbConnect();
            MySqlCommand cmd2 = new MySqlCommand($"SELECT hry FROM users WHERE email='{user.Email}';", Database.connection);
            MySqlDataReader reader2 = cmd2.ExecuteReader();

            while(reader2.Read())
            {
                poleHier = reader2["hry"].ToString().Split(';').ToList();
            }

            Database.dbClose();


            //toto kontroluje ci je hra zakupena (mohlo to byt aj vo funkcii ale vzhladom na error ktory neviem pochopit to tam byt nemoze)

            if (poleHier != null)
            {
                foreach (string item in poleHier)
                {
                    if (item == game.Name)
                    {
                        kupit.Enabled = false;
                        kupit.Text = "Zakúpené";
                    }
                }
            }

        }
    }
}
