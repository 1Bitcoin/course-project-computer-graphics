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

        public Form1()
        {
            InitializeComponent();

            result = new Bitmap(canvas.Width, canvas.Height);
            g = Graphics.FromImage(result);
            canvas.Image = result;
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
            double[] center4 = { 0, -5001, 0 };


            double[] color1 = { 255, 0, 0 };
            double[] color2 = { 0, 0, 255 };
            double[] color3 = { 0, 255, 0 };
            double[] color4 = { 255, 255, 0 };

            Object[] objects = { new Sphere(center2, 1, color1), new Sphere(center1, 1, color2),
                new Sphere(center3, 1, color3), new Sphere(center4, 5000, color4) };

            double[] poslight = { 0, 2, 4 };
            double[] directionlight = { 1, 4, 4 };

            Light[] lights = { new AmbientLight(0.1), new PointLight(poslight, 0.6), new DirectionalLight(directionlight, 0.2) };


            for (int x = -result.Width / 2; x < result.Width / 2; x++)
            {
                for (int y = -result.Height / 2; y < result.Height / 2; y++)
                {
                    int[] work = { x, y };
                    double[] direction = RayTracing.CanvasToViewport(result, work);
                    Color color = RayTracing.TraceRay(lights, objects, cameraPosition, direction, 1, Double.PositiveInfinity);
                    RayTracing.PutPixel(result, x, y, color);

                }
            }      
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
