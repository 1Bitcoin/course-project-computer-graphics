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
        int workFlag = 1;

        long totalTime = 0;
        Scene scene = new Scene();

        public Form1()
        {
            InitializeComponent();       
            progressBar1.Value = 0;
            result = new Bitmap(canvas.Width, canvas.Height);
            progressBar1.Maximum = result.Width * result.Height;           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                progressBar1.Value = 0;               
                backgroundWorker1.RunWorkerAsync();
            }     
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value += e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                workFlag = 0;
            }

            canvas.Image = result;
            canvas.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
            canvas.Refresh();
            label2.Text = "Work time(milliseconds): " + totalTime;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            progressBar1.Value = 0;

            workFlag = 1;

            result = new Bitmap(canvas.Width, canvas.Height);


            Bitmap texture1 = Image.FromFile(@"d:\1.bmp") as Bitmap;

            int angleY = Int32.Parse(textBox1.Text);
            int angleX = Int32.Parse(textBox5.Text);
            int angleZ = Int32.Parse(textBox6.Text);

            double[] cameraPosition = { Int32.Parse(textBox2.Text), Int32.Parse(textBox3.Text), Int32.Parse(textBox4.Text) };

            int recursionDepth = Int32.Parse(textBox7.Text);
            int countThread = Int32.Parse(textBox8.Text);
            
            scene.SetMajorScene();

            var rect = new Rectangle(0, 0, result.Width, result.Height);
            var data = result.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, result.PixelFormat);
            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

            int width = data.Width;
            int height = data.Height;

            var buffer = new byte[width * height * depth];

            Stopwatch time = new Stopwatch(); // создаём объект Stopwatch
            time.Start(); // запускаем отсчёт времени

            //copy pixels to buffer
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            int step = data.Width / countThread;

            Thread[] t = new Thread[countThread];

            int x1 = 0;
            int x2 = step;

            int y1 = 0;
            int y2 = data.Height;

            for (int i = 0; i < countThread; i++)
            {
                AllParameters p = new AllParameters(x1, y1, x2, y2, ref workFlag);

                t[i] = new Thread(Process);
                t[i].Start(p);

                x1 = x2;
                x2 += step;
            }

            foreach (Thread thread in t)
            {
                thread.Join();
            }

            result.UnlockBits(data);

            void Process(object obj)
            {
                AllParameters p = (AllParameters)obj;

                int x = p.x;
                int y = p.y;

                int endx = p.endx;
                int endy = p.endy;

                for (int i = x; i < endx & workFlag == 1; i++)
                {
                    for (int j = y; j < endy & workFlag == 1; j++)
                    {
                        int[] work = { i - height / 2, j - width / 2 };

                        double[] direction = RayTracing.CanvasToViewport(width, height, work);
                        direction = MyMath.MultiplyMV(MyMath.getMatrixOx(angleX), direction);
                        direction = MyMath.MultiplyMV(MyMath.getMatrixOy(angleY), direction);
                        direction = MyMath.MultiplyMV(MyMath.getMatrixOz(angleZ), direction);

                        double[] color = RayTracing.TraceRay(recursionDepth, scene.lights, scene.objects, 
                            cameraPosition, direction, 1, Double.PositiveInfinity, 0);

                        var offset = ((j * width) + i) * depth;

                        buffer[offset + 0] = (byte)color[2];
                        buffer[offset + 1] = (byte)color[1];
                        buffer[offset + 2] = (byte)color[0];

                        //backgroundWorker1.ReportProgress(1);
                    }
                  
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker1.ReportProgress(0);
                        return;
                    }
                }

            }
          
            string outputFile1 = @"d:\temp1.bmp";
            string outputFile2 = @"d:\temp2.bmp";

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

            time.Stop(); // останавливаем работу таймера

            totalTime = time.ElapsedMilliseconds;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку

            LoaderFile.Load(filename);
            LoaderFile.InitializingTriangles(scene.objects);

            MessageBox.Show("Файл открыт");

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

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

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

    }
}
