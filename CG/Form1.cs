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

            double[] cameraPosition = { 0, 0, 0 };

            double[] center1 = { 0, -1, 3 };
            double[] center2 = { 2, 0, 4 };
            double[] center3 = { -2, 0, 4 };

            Sphere[] spheres = { new Sphere(center2, 1, Color.Green), new Sphere(center1, 1, Color.Red),
                new Sphere(center3, 1, Color.Blue) };


            for (int x = -result.Width / 2; x < result.Width / 2; x++)
            {
                for (int y = -result.Height / 2; y < result.Height / 2; y++)
                {
                    int[] work = { x, y };
                    double[] direction = RayTracing.CanvasToViewport(result, work);
                    Color color = RayTracing.TraceRay(spheres, cameraPosition, direction, 1, Double.PositiveInfinity);
                    RayTracing.PutPixel(result, x, y, color);

                }
            }

            /*g.DrawEllipse(penDraw, 100, 100, 100, 100);
            g.DrawEllipse(penDraw, 200, 200, 100, 100);
            g.DrawEllipse(penDraw, 300, 250, 100, 100);*/
           

            canvas.Refresh();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void canvas_Click(object sender, EventArgs e)
        {

        }
    }
}
