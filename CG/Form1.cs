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
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;

namespace CG
{
    public partial class Form1 : Form
    {
        Bitmap result;
        int flag = 0;

        public Form1()
        {
            InitializeComponent();

            result = new Bitmap(canvas.Width, canvas.Height);

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
            double[] behindMajorsphere = { 0, 0.2, 5 };

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

            int recursionDepth = 5;

            //new Sphere(center2, 1, color1, 50, 0.2)
            //new Sphere(center3, 1, color3, 10, 0.4)
            //{ new Sphere(majorSphere, 1, color2, 1000, 0, 1),

            Object[] objects = { new Sphere(test, 1, orange, 1000, 0.5, 0, 1),
                 new Sphere(majorSphere, 1, color2, 1000, 0, 1, 1.33),
                 new Sphere(baseSphere, 0.3, snow, 1000, 0.1, 0, 1),
                 new Sphere(middleSphere, 0.2, snow, 1000, 0.1, 0, 1), new Sphere(highSphere, 0.1, snow, 1000, 0.1, 0, 1),
                 new Sphere(underMajorsphere, 1, color1, 100, 0.01, 0, 1),
                 new Sphere(eye1, 0.025, black, 1000, 0.3, 0, 1), new Sphere(eye2, 0.025, black, 1000, 0.3, 0, 1),
                 new Sphere(nose, 0.025, orange, 1000, 0.3, 0, 1), /*new Sphere(test, 1, color3, 1000, 0.5, 0, 1),*/
                 new Sphere(plane, 5000, color4, 1000, 0.5, 0, 1),

                 /*new Sphere(behindMajorsphere, 1, color2, 1000, 0, 1, 1.33)*/ };

            Light[] lights = { new AmbientLight(0.4), new PointLight(poslight, 0.2), new PointLight(poslight1, 0.2), new PointLight(poslight2, 0.2) };
            //new PointLight(poslight, 0.6)

            int angle = Int32.Parse(textBox1.Text);

            double[,] cameraRotationOY = { 
                                        { Math.Cos(Math.PI * angle / 180.0), 0, Math.Sin(Math.PI * angle / 180.0) }, 
                                        { 0, 1, 0 },
                                        { -Math.Sin(Math.PI * angle / 180.0), 0, Math.Cos(Math.PI * angle / 180.0)}
                                        }; //dell

            //c = Color.FromArgb(80, 20, 86, 20);

            var rect = new Rectangle(0, 0, result.Width, result.Height);
            var data = result.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, result.PixelFormat);
            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

            var buffer = new byte[data.Width * data.Height * depth];

            Stopwatch time = new Stopwatch(); // создаём объект Stopwatch
            time.Start(); // запускаем отсчёт времени

            //copy pixels to buffer
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            Parallel.Invoke(
                () => {
                    //upper-left
                    Process(buffer, 0, 0, data.Width / 2, data.Height, data.Width, depth);
                },
                () => {
                    //lower-right
                    Process(buffer, data.Width / 2, 0, data.Width, data.Height, data.Width, depth);
                }/*,
                () => {
                    //lower-right
                    Process(buffer, data.Width / 2, 0, data.Width, data.Height / 2, data.Width, depth);
                },
                () => {
                    //lower-right
                    Process(buffer, 0, data.Height / 2, data.Width / 2, data.Height, data.Width, depth);
                }*/
            );


            void Process(byte[] my_buffer, int x, int y, int endx, int endy, int width, int my_depth)
            {
                for (int i = x; i < endx; i++)
                {
                    for (int j = y; j < endy; j++)
                    {
                        int[] work = { i - data.Height / 2, j - data.Width / 2 };

                        double[] direction = RayTracing.CanvasToViewport(data.Width, data.Height, work);
                        direction = MyMath.MultiplyMV(cameraRotationOY, direction);

                        double[] color = RayTracing.TraceRay(recursionDepth, lights, objects, cameraPosition, direction, 1, Double.PositiveInfinity, 0);

                        var offset = ((j * data.Width) + i) * depth;

                        buffer[offset + 0] = (byte)color[2];
                        buffer[offset + 1] = (byte)color[1];
                        buffer[offset + 2] = (byte)color[0];
                    }
                }

            }

            result.UnlockBits(data);

            string outputFile1 = @"d:\GO1.bmp";
            string outputFile2 = @"d:\GO2.bmp";

            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);

            if (flag == 0)
            {
                result.Save(outputFile1, ImageFormat.Bmp);
                result = Bitmap.FromFile(outputFile1) as Bitmap;
                flag = 1;
            }
            else
            {
                result.Save(outputFile2, ImageFormat.Bmp);
                result = Bitmap.FromFile(outputFile2) as Bitmap;
                flag = 0;
            }

            canvas.Image = result;
            canvas.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
            canvas.Refresh();

            time.Stop(); // останавливаем работу таймера
            label2.Text = "Work time(milliseconds): " + time.ElapsedMilliseconds; // выводим затраченное время

            
            /*Stopwatch time = new Stopwatch(); // создаём объект Stopwatch
            time.Start(); // запускаем отсчёт времени

            for (int x = -result.Width / 2; x < result.Width / 2; x++)
            {
                for (int y = -result.Height / 2; y < result.Height / 2; y++)
                {
                    int[] work = { x, y };
                    double[] direction = RayTracing.CanvasToViewport(result.Width, result.Height, work);
                    direction = MyMath.MultiplyMV(cameraRotationOY, direction);
                    double[] color = RayTracing.TraceRay(recursionDepth, lights, objects, cameraPosition, direction, 1, Double.PositiveInfinity, 0);
                    RayTracing.PutPixel(result, x, y, RayTracing.Clamp(color));

                }
            }

            canvas.Image = result;
            time.Stop(); // останавливаем работу таймера
            label2.Text = "Work time(milliseconds): " + time.ElapsedMilliseconds; // выводим затраченное время
            */
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

        private void button2_Click_1(object sender, EventArgs e)
        {
            string inputFile = @"d:\1.jpg";
            string outputFile = @"d:\b.jpg";

            Bitmap bmp = Bitmap.FromFile(inputFile) as Bitmap;

            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

            var buffer = new byte[data.Width * data.Height * depth];

            //copy pixels to buffer
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            Parallel.Invoke(
                () => {
            //upper-left
            Process(buffer, 0, 0, data.Width / 2, data.Height / 2, data.Width, depth);
                },
                () => {
            //lower-right
            Process(buffer, data.Width / 2, data.Height / 2, data.Width, data.Height, data.Width, depth);
                }
            );

            //Copy the buffer back to image
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);

            bmp.UnlockBits(data);

            bmp.Save(outputFile, ImageFormat.Jpeg);
        }

        void Process(byte[] buffer, int x, int y, int endx, int endy, int width, int depth)
        {
            for (int i = x; i < endx; i++)
            {
                for (int j = y; j < endy; j++)
                {
                    var offset = ((j * width) + i) * depth;
                    // Dummy work    
                    // To grayscale (0.2126 R + 0.7152 G + 0.0722 B)
                    var b = 0.2126 * buffer[offset + 0] + 0.7152 * buffer[offset + 1] + 0.0722 * buffer[offset + 2];
                    buffer[offset + 0] = buffer[offset + 1] = buffer[offset + 2] = (byte)b;
                }
            }


        }
    }
}
