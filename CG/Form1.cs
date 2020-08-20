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

            double[] cameraPosition = { Int32.Parse(textBox2.Text), Int32.Parse(textBox3.Text), Int32.Parse(textBox4.Text) };

            double[] eye1 = { 0.05, 0.17, 2.92 };
            double[] eye2 = { -0.05, 0.17, 2.92 };

            double[] nose = { 0, 0.13, 2.92 };

            double[] baseSphere = { 0, -0.5, 3 };
            double[] middleSphere = { 0, -0.1, 3 };
            double[] highSphere = { 0, 0.17, 3 };

            double[] majorSphere = { 0, -0.4, 3 };
            double[] underMajorsphere = { 0, -1.6, 3 };
            double[] plane = { 0, -5001, 0 };
            double[] behindMajorsphere = { 0, 0.5, 6 };

            double[] test = { -2.1, 0.5, 3 };

            double[] black = { 0, 0, 0 };
            double[] orange = { 255, 128, 0 };
            double[] snow = { 255, 255, 255 };

            double[] color1 = { 254, 239, 210 };
            double[] color2 = { 240, 240, 240 };
            double[] color3 = { 0, 255, 0 };
            double[] color4 = { 255, 255, 0 };

            double[] poslight = { 2, 2, -1 };
            double[] poslight1 = { -1, 1, 3 };
            double[] poslight2 = { 0, 5, 4 };

            double[] directionlight = { 0, 0, -2 };

            int recursionDepth = 3;

            //new Sphere(center2, 1, color1, 50, 0.2)
            //new Sphere(center3, 1, color3, 10, 0.4)
            //{ new Sphere(majorSphere, 1, color2, 1000, 0, 1),
            Object[] objects = { new Sphere(majorSphere, 1, color2, 1000, 0, 1), new Sphere(baseSphere, 0.3, snow, 1000, 0.1, 0),
                 new Sphere(middleSphere, 0.2, snow, 1000, 0.1, 0), new Sphere(highSphere, 0.1, snow, 1000, 0.1, 0),
                 new Sphere(plane, 5000, color4, 1000, 0.5, 0), new Sphere(underMajorsphere, 1, color1, 100, 0.01, 0),
                 new Sphere(eye1, 0.025, black, 1000, 0.3, 0), new Sphere(eye2, 0.025, black, 1000, 0.3, 0),
                 new Sphere(nose, 0.025, orange, 1000, 0.3, 0), new Sphere(test, 1, color3, 1000, 0.5, 0),
                 new Sphere(behindMajorsphere, 1, orange, 1000, 0, 0) };

            Light[] lights = { new AmbientLight(0.4), new PointLight(poslight, 0.2), new PointLight(poslight1, 0.2), new PointLight(poslight2, 0.2) };
            //new PointLight(poslight, 0.6)

            int angle = Int32.Parse(textBox1.Text);

            double[,] cameraRotation = { 
                                        { Math.Cos(Math.PI * angle / 180.0), 0, Math.Sin(Math.PI * angle / 180.0) }, 
                                        { 0, 1, 0 },
                                        { -Math.Sin(Math.PI * angle / 180.0), 0, Math.Cos(Math.PI * angle / 180.0)}
                                        }; //dell

            //c = Color.FromArgb(80, 20, 86, 20);
            Stopwatch time = new Stopwatch(); // создаём объект Stopwatch
            time.Start(); // запускаем отсчёт времени

            for (int x = -result.Width / 2; x < result.Width / 2; x++)
            {
                for (int y = -result.Height / 2; y < result.Height / 2; y++)
                {
                    int[] work = { x, y };
                    double[] direction = RayTracing.CanvasToViewport(result, work);
                    direction = RayTracing.MultiplyMV(cameraRotation, direction);
                    double[] color = RayTracing.TraceRay(recursionDepth, lights, objects, cameraPosition, direction, 1, Double.PositiveInfinity, 0);
                    RayTracing.PutPixel(result, x, y, RayTracing.Clamp(color));
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
