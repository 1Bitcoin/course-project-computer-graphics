using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG
{
    public partial class Form1 : Form
    {
        Bitmap result;
        Graphics g;
        Pen penDraw;

        public Form1()
        {
            InitializeComponent();

            result = new Bitmap(canvas.Width, canvas.Height);
            g = Graphics.FromImage(result);
            canvas.Image = result;

            penDraw = new Pen(Color.Black);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            int[] cameraPosition = { 0, 0, 0 };

            int[] backgroundColor = { 255, 255, 255 };

            int[] center1 = { 0, -1, 3 };
            int[] center2 = { 2, 0, 4 };
            int[] center3 = { -2, 0, 4 };

            Sphere[] spheres = { new Sphere(center1, 1, Color.Green), new Sphere(center2, 1, Color.Red),
                new Sphere(center3, 1, Color.Blue) };


            /*for (int i = 0; i < 100; i++)
                result.SetPixel(i, i, Color.Black);

            g.DrawEllipse(penDraw, 20, 20, 30, 100);*/

            canvas.Refresh();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
