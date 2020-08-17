using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

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

            progressBar1.Maximum = result.Width * result.Height;
            progressBar1.Value = 0;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;

            double[] cameraPosition = { 0, 0, 0 };

            double[] center1 = { 0, -1, 5 };
            double[] center2 = { -1.5, 0.4, 4 };
            double[] center3 = { 1.8, 0, 3.2 };
            double[] center4 = { 0, -5001, 0 };


            double[] color1 = { 255, 0, 0 };
            double[] color2 = { 0, 0, 255 };
            double[] color3 = { 0, 255, 0 };
            double[] color4 = { 255, 255, 0 };

            double[] poslight = { 2, 2, -1 };
            double[] directionlight = { 0, 0, -2 };

            Object[] objects = { new Sphere(center2, 1, color1, 50), new Sphere(center1, 1, color2, 500),
                new Sphere(center3, 1, color3, 10), new Sphere(center4, 5000, color4, 1000) };

            Light[] lights = { new AmbientLight(0.2), new PointLight(poslight, 0.6) };
            //new PointLight(poslight, 0.6)

            double[,] cameraRotation = { 
                                        { 0.984, 0, -0.1736 }, 
                                        { 0, 1, 0 },
                                        { 0.1736, 0, 0.984 }
                                        }; //dell

            Stopwatch time = new Stopwatch(); // создаём объект Stopwatch
            time.Start(); // запускаем отсчёт времени

            for (int x = -result.Width / 2; x < result.Width / 2; x++)
            {
                for (int y = -result.Height / 2; y < result.Height / 2; y++)
                {
                    int[] work = { x, y };
                    double[] direction = RayTracing.CanvasToViewport(result, work);
                    //direction = RayTracing.MultiplyMV(cameraRotation, direction);
                    Color color = RayTracing.TraceRay(lights, objects, cameraPosition, direction, 1, Double.PositiveInfinity);
                    RayTracing.PutPixel(result, x, y, color);
                    //label1.Text = color.ToString();
                    progressBar1.Value++;
                }
            }

            time.Stop(); // останавливаем работу таймера
            label2.Text = "Work time(milliseconds): " + time.ElapsedMilliseconds; // выводим затраченное время

            canvas.Refresh();

        }



        private void canvas_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
