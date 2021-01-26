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

        SettingModel settingModel = new SettingModel(1000, 0, 0, new double[] { 0, 255, 0 });

        System.TimeSpan totalTime;

        Scene scene = new Scene();

        public Form1()
        {
            InitializeComponent();       
            progressBar1.Value = 0;
            result = new Bitmap(canvas.Width, canvas.Height);
            progressBar1.Maximum = canvas.Width;

            openFileDialog1.Filter = "obj files (*.obj)|*.obj";

            comboBox1.Items.Add("Сфера");
            comboBox1.Items.Add("Треугольник");
            comboBox1.Items.Add("Точечный источник");
            comboBox1.Items.Add("Направленный источник");
            comboBox1.Items.Add("Окружающее освещение");
            comboBox1.Items.Add("Дисковый источник(мягкие тени)");

            comboBox1.SelectedIndex = 0;

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            scene.SetMajorScene();

            string path = System.IO.Directory.GetCurrentDirectory();
            string file1 = path + "\\objects\\el1.obj";
            string file2 = path + "\\objects\\el2.obj";

            LoaderFile.Load(file1);
            LoaderFile.InitializingTriangles(ref scene, settingModel);

            LoaderFile.Load(file2);
            LoaderFile.InitializingTriangles(ref scene, settingModel);

            listBox1.DataSource = scene.objects;
            listBox1.DisplayMember = "MyNameObject";

            listBox2.DataSource = scene.lights;
            listBox2.DisplayMember = "MyNameLight";


            listBox1.SelectedValueChanged += new EventHandler(Listbox1_SelectedValueChanged);
            listBox2.SelectedValueChanged += new EventHandler(Listbox2_SelectedValueChanged);

        }

        void Listbox1_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            if (listBox2.SelectedItem != null)
                listBox2.SetSelected((listBox2.Items.IndexOf(listBox2.SelectedItem)), false);

        }

        void Listbox2_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            if (listBox1.SelectedItem != null)
                listBox1.SetSelected((listBox1.Items.IndexOf(listBox1.SelectedItem)), false);

        }

        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox1.SelectedItem.ToString();
            //MessageBox.Show(selectedState);
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

            string timeName = "Время рендера: ";
            string tsOut = totalTime.ToString(@"hh\:mm\:ss\.ff");

            label2.Text = timeName + tsOut;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            progressBar1.Value = 0;

            workFlag = 1;

            result = new Bitmap(canvas.Width, canvas.Height);

            int angleX = 0;
            int angleY = 0;
            int angleZ = 0;

            try
            {
                angleY = Int32.Parse(textBox1.Text);
                angleX = Int32.Parse(textBox5.Text);
                angleZ = Int32.Parse(textBox6.Text);
            }
            catch
            {
                MessageBox.Show("Проверьте введенные данные поворота камеры(только целые положительные и отрицательные числа)");
                return;
            }

            int posX = 0;
            int posY = 0;
            int posZ = 0;

            try
            {
                posX = Int32.Parse(textBox2.Text);
                posY = Int32.Parse(textBox3.Text);
                posZ = Int32.Parse(textBox4.Text);
            }
            catch
            {
                MessageBox.Show("Проверьте введенные данные позиции камеры камеры(только целые положительные и отрицательные числа)");
                return;
            }

            double[] cameraPosition = { posX, posY, posZ };

            int recursionDepth = 3;
            int countThread = 1;

            try
            {
                countThread = Int32.Parse(textBox8.Text);
            }
            catch
            {
                MessageBox.Show("Проверьте введенные данные количества потоков(только целые положительные)");
                return;
            }

            try
            {
                recursionDepth = Int32.Parse(textBox7.Text);
            }
            catch
            {
                MessageBox.Show("Проверьте введенные данные количества потоков(только целые положительные)");
                return;
            }

            if (countThread <= 0)
            {
                MessageBox.Show("Число потоков должно быть сторого больше нуля");
                return;
            }

            if (recursionDepth < 0)
            {
                MessageBox.Show("Глубина рекурсии должна быть отрицательной");
                return;
            }

            int codeError = scene.CheckScene();

            if (codeError == 1)
            {
                MessageBox.Show("Суммарная интенсивность всех источников света должна быть не больше 1");
                return;
            }

            if (codeError == 2)
            {
                countThread = 1;
            }

            if (codeError == 3)
            {
                MessageBox.Show("На сцене может быть только один источник окружающего освещения!");
                return;
            }

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
                        int transparentBuffer = 0;
                        int countRays = 0;

                        double[] direction = RayTracing.CanvasToViewport(width, height, work);
                        direction = MyMath.MultiplyMV(MyMath.getMatrixOx(angleX), direction);
                        direction = MyMath.MultiplyMV(MyMath.getMatrixOy(angleY), direction);
                        direction = MyMath.MultiplyMV(MyMath.getMatrixOz(angleZ), direction);

                        double[] color = RayTracing.TraceRay(recursionDepth, scene.lights, scene.objects, 
                            cameraPosition, direction, 1, Double.PositiveInfinity, 0, ref transparentBuffer, ref countRays);

                        var offset = ((j * width) + i) * depth;

                        buffer[offset + 0] = (byte)color[2];
                        buffer[offset + 1] = (byte)color[1];
                        buffer[offset + 2] = (byte)color[0];


                    }

                    backgroundWorker1.ReportProgress(1);


                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker1.ReportProgress(0);
                        return;
                    }
                }

            }

            string path = Directory.GetCurrentDirectory();
            string outputFile1 = path + "\\temp1.bmp";
            string outputFile2 = path + "\\temp2.bmp";

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

            totalTime = time.Elapsed;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку

            LoaderFile.Load(filename);
            LoaderFile.InitializingTriangles(ref scene, settingModel);

            listBox1.DataSource = null;
            listBox1.DataSource = scene.objects;
            listBox1.DisplayMember = "MyNameObject";

            MessageBox.Show("Файл открыт");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Сфера")
            {
                Form2 newForm = new Form2(ref scene, -1);
                newForm.ShowDialog();

                listBox1.DataSource = null;
                listBox1.DataSource = scene.objects;
                listBox1.DisplayMember = "MyNameObject";

            }

            if (comboBox1.SelectedItem.ToString() == "Треугольник")
            {
                Form3 newForm = new Form3(ref scene, -1);
                newForm.ShowDialog();

                listBox1.DataSource = null;
                listBox1.DataSource = scene.objects;
                listBox1.DisplayMember = "MyNameObject";

            }

            if (comboBox1.SelectedItem.ToString() == "Точечный источник")
            {
                Form4 newForm = new Form4(ref scene, -1);
                newForm.ShowDialog();

                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";

            }

            if (comboBox1.SelectedItem.ToString() == "Направленный источник")
            {
                Form5 newForm = new Form5(ref scene, -1);
                newForm.ShowDialog();

                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";

            }

            if (comboBox1.SelectedItem.ToString() == "Окружающее освещение")
            {
                Form6 newForm = new Form6(ref scene, -1);
                newForm.ShowDialog();

                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";
            }

            if (comboBox1.SelectedItem.ToString() == "Дисковый источник(мягкие тени)")
            {
                Form7 newForm = new Form7(ref scene, -1);
                newForm.ShowDialog();

                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is Object obj)
            {
                scene.objects.Remove(obj);
                listBox1.DataSource = null;
                listBox1.DataSource = scene.objects;
                listBox1.DisplayMember = "MyNameObject";
            }

            if (listBox2.SelectedItem is Light light)
            {
                scene.lights.Remove(light);
                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form8 newForm = new Form8(ref settingModel);
            newForm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int indexObject = scene.objects.IndexOf((Object)listBox1.SelectedItem);

            if (listBox1.SelectedItem is Sphere)
            {
                Form2 newForm = new Form2(ref scene, scene.objects.IndexOf((Object)listBox1.SelectedItem));
                newForm.ShowDialog();

                listBox1.DataSource = null;
                listBox1.DataSource = scene.objects;
                listBox1.DisplayMember = "MyNameObject";
            }

            if (listBox1.SelectedItem is Triangle)
            {
                Form3 newForm = new Form3(ref scene, scene.objects.IndexOf((Object)listBox1.SelectedItem));
                newForm.ShowDialog();

                listBox1.DataSource = null;
                listBox1.DataSource = scene.objects;
                listBox1.DisplayMember = "MyNameObject";
            }

            if (listBox2.SelectedItem is PointLight)
            {
                Form4 newForm = new Form4(ref scene, scene.lights.IndexOf((Light)listBox2.SelectedItem));
                newForm.ShowDialog();

                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";

            }

            if (listBox2.SelectedItem is DirectionalLight)
            {
                Form5 newForm = new Form5(ref scene, scene.lights.IndexOf((Light)listBox2.SelectedItem));
                newForm.ShowDialog();

                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";

            }

            if (listBox2.SelectedItem is AmbientLight)
            {
                Form6 newForm = new Form6(ref scene, scene.lights.IndexOf((Light)listBox2.SelectedItem));
                newForm.ShowDialog();

                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";

            }

            if (listBox2.SelectedItem is LightDisk)
            {
                Form7 newForm = new Form7(ref scene, scene.lights.IndexOf((Light)listBox2.SelectedItem));
                newForm.ShowDialog();

                listBox2.DataSource = null;
                listBox2.DataSource = scene.lights;
                listBox2.DisplayMember = "MyNameLight";

            }

        }

        private void listBox1_Click(object sender, MouseEventArgs e)
        {
            
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

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
