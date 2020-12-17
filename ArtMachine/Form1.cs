using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArtMachine
{
    public partial class Form1 : Form
    {
        public int Page = 1;
        public int GumpPage = 1;
        public int PerPage = 56;
        public List<GumpImage> GumpImages  = new List<GumpImage>();
        public Form1()
        {
            InitializeComponent();
            LoadGumps();
            //LoadPage();
            LoadGumpPage();
        }

        private void LoadMap(int mapId)
        {
            panel2.Controls.Clear();
            var pictureBox = new PictureBox();
            var image = Ultima.MultiMap.GetFacetImage(mapId);
            pictureBox.Width = image.Width;
            pictureBox.Height = image.Height;
            pictureBox.Location = new Point(0,0);
            pictureBox.Image = image;
            pictureBox.MouseClick += new MouseEventHandler(map_onClick);
            panel2.Controls.Add(pictureBox);
        }

        private void map_onClick(object sender, MouseEventArgs e)
        {
            label2.Text = e.Location.X + ":" + e.Location.Y;
        }

        private void LoadGumps()
        {
            var laptop = @"C:\Users\kline\Dropbox";
            var pc = @"D:\Clouds\Dropbox";
            var server = @"D:\Dropbox";
            var baseDir = Directory.Exists(laptop) ? laptop :
                Directory.Exists(pc) ? pc : server;
            var gumpDir = $@"{baseDir}\blurrydude.com\UOImages\Gumps\";
            var subDirs = Directory.GetDirectories(gumpDir);
            foreach (var subDir in subDirs)
            {
                var subSubDirs = Directory.GetDirectories(subDir);
                foreach (var subSubDir in subSubDirs)
                {
                    var files = Directory.GetFiles(subSubDir, "*.png");
                    foreach (var file in files)
                    {
                        var id = Convert.ToInt32(file.Split('\\').Last().Replace(".png", string.Empty));
                        GumpImages.Add(new GumpImage
                        {
                            Id = id,
                            Path = file
                        });
                    }
                }
            }
        }
        private void LoadGumpPage()
        {
            listView1.Items.Clear();
            int p = GumpPage - 1;
            
            var grid = new ImageList();

            var images = GumpImages.Skip(p*500).Take(500);

            var i = 0;
            foreach (var gumpImage in images)
            {
                var image = Image.FromFile(gumpImage.Path);
                grid.Images.Add(image);
                listView1.Items.Add(new ListViewItem
                {
                    ImageIndex = i,
                    Text = gumpImage.Id.ToString()
                });
                i++;
            }

            listView1.LargeImageList = grid;
        }

        public class GumpImage
        {
            public int Id { get; set; }
            public string Path { get; set; }
        }

        private void LoadPage()
        {
            var laptop = @"C:\Users\kline\Dropbox";
            var pc = @"D:\Clouds\Dropbox";
            var server = @"D:\Dropbox";
            var baseDir = Directory.Exists(laptop) ? laptop :
                Directory.Exists(pc) ? pc : server;
            numericUpDown2.Value = Page;
            int p = Page - 1;
            var s = p * PerPage + 1;
            var e = s + PerPage - 1;
            var grid = new List<TileImage>();
            panel1.Controls.Clear();
            for (var i = s; i <= e && i >= s && i <= e; i++)
            {
                try
                {
                    var dirb = i - (i % 100);
                    var dira = i - (i % 1000);
                    var path = $@"{baseDir}\blurrydude.com\UOImages\{dira}\{dirb}\{i}.png";
                    var pictureBox = new PictureBox();
                    var image = Image.FromFile(path);
                    var w = image.Width;
                    var h = image.Height;
                    pictureBox.Image = image;
                    pictureBox.Name = i.ToString();
                    pictureBox.Width = w;
                    pictureBox.Height = h;
                    pictureBox.BackColor = Color.DarkViolet;
                    pictureBox.Click += new System.EventHandler(PreviewImage);
                    var tile = new TileImage
                    {
                        Index = i,
                        PictureBox = pictureBox
                    };
                    grid.Add(tile);
                } catch(Exception){}
            }

            var tileWidth = grid.Max(x => x.PictureBox.Image.Width) + 4;
            var tileHeight = grid.Max(x => x.PictureBox.Image.Height) + 8;
            var columns = panel1.Width / tileWidth;

            foreach (var tile in grid)
            {
                var i = tile.Index - (p * PerPage);
                var x = i % columns;
                var y = (i - x) / columns;
                tile.PictureBox.Location = new Point(x*tileWidth,y*tileHeight);
                panel1.Controls.Add(tile.PictureBox);
            }
        }

        public class TileImage
        {
            public int Index { get; set; }
            public PictureBox PictureBox { get; set; }
        }

        private void PreviewImage(object sender, EventArgs e)
        {
            var image = ((PictureBox)sender).Image;
            var itemData = Ultima.TileData.ItemTable[Convert.ToInt32(((PictureBox)sender).Name)];
            label1.Text = ((PictureBox)sender).Name + " " + itemData.Name;
            pictureBox1.Image = image;
            pictureBox1.Width = image.Width * 2;
            pictureBox1.Height = image.Height * 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PerPage = (int)numericUpDown1.Value;
            Page++;
            LoadPage();
            button2.Visible = Page > 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PerPage = (int)numericUpDown1.Value;
            if (Page > 1) Page--;
            LoadPage();
            button2.Visible = Page > 1;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            PerPage = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Page = (int)numericUpDown2.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GumpPage++;
            LoadGumpPage();
            button4.Visible = GumpPage > 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (GumpPage > 1) GumpPage--;
            LoadGumpPage();
            button4.Visible = GumpPage > 1;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var laptop = @"C:\Users\kline\Dropbox";
            var pc = @"D:\Clouds\Dropbox";
            var server = @"D:\Dropbox";
            var baseDir = Directory.Exists(laptop) ? laptop :
                Directory.Exists(pc) ? pc : server;
            var gumpDir = $@"{baseDir}\blurrydude.com\UOImages\Gumps\";
            var selectedItem = listView1.SelectedItems[0];
            var id = Convert.ToInt32(selectedItem.Text);
            var dirb = id - (id % 100);
            var dira = id - (id % 1000);
            var path = $@"{gumpDir}{dira}\{dirb}\{id}.png";
            var image = Image.FromFile(path);
            pictureBox2.Width = image.Width;
            pictureBox2.Height = image.Height;
            pictureBox2.Image = image;
            label3.Text = selectedItem.Text;
            label4.Text = $"{image.Width} x {image.Height}";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                default:
                case "Trammel": LoadMap(0); break;
                case "Felucca": LoadMap(1); break;
                case "Ilshenar": LoadMap(2); break;
                case "Malas": LoadMap(3); break;
                case "Tokuno": LoadMap(4); break;
                case "TerMur": LoadMap(5); break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Huify();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Huify();
        }

        private void Huify()
        {
            var image = pictureBox2.Image;
            Ultima.Hues.GetHue((int)numericUpDown3.Value).ApplyTo((Bitmap)image, false);
            pictureBox2.Image = image;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            var image = pictureBox1.Image;
            Ultima.Hues.GetHue((int)numericUpDown3.Value).ApplyTo((Bitmap)image, false);
            pictureBox1.Image = image;
        }
    }
}
