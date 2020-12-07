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
    public partial class Form2 : Form
    {
        Scene tempScene = null;
        int tempIndex = 0;
        Bitmap texture = null;

        public Form2(ref Scene scene, int index)
        {
            InitializeComponent();
            tempIndex = index;

            if (tempIndex != -1)
            {
                this.Text = "Редактирование сферы";
                Sphere sphere = (Sphere)scene.objects[tempIndex];

                numericUpDown2.Value = Convert.ToDecimal(sphere.center[0]);
                numericUpDown3.Value = Convert.ToDecimal(sphere.center[1]);
                numericUpDown4.Value = Convert.ToDecimal(sphere.center[2]);

                numericUpDown1.Value = Convert.ToDecimal(sphere.radius);

                numericUpDown6.Value = Convert.ToDecimal(sphere.specular);
                numericUpDown5.Value = Convert.ToDecimal(sphere.reflective);
                numericUpDown7.Value = Convert.ToDecimal(sphere.transparent);

                this.textBox1.Text = sphere.name;

                byte r = Convert.ToByte(sphere.color[0]);
                byte g = Convert.ToByte(sphere.color[1]);
                byte b = Convert.ToByte(sphere.color[2]);

                Color myRgbColor = new Color();
                myRgbColor = Color.FromArgb(r, g, b);

                colorDialog1.Color = myRgbColor;

                this.textBox1.Text = sphere.name;
                texture = sphere.texture;
            }
            else
            {
                int count = scene.GetCountSpheres() + 1;
                this.textBox1.Text = "Сфера #" + count;

            }
          
            tempScene = scene;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double x = decimal.ToDouble(numericUpDown2.Value);
            double y = decimal.ToDouble(numericUpDown3.Value);
            double z = decimal.ToDouble(numericUpDown4.Value);

            double[] position = { x, y, z };

            double radius = decimal.ToDouble(numericUpDown1.Value);

            int specular = decimal.ToInt32(numericUpDown6.Value);
            double reflective = decimal.ToDouble(numericUpDown5.Value);
            double transparent = decimal.ToDouble(numericUpDown7.Value);


            double[] color = { colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B };
         

            if (openFileDialog1.FileName != "openFileDialog1")
                texture = Image.FromFile(openFileDialog1.FileName) as Bitmap;

            Object obj = new Sphere(position, radius, color, specular, reflective, transparent, 0, texture, textBox1.Text);

            if (tempIndex == -1)
                tempScene.objects.Add(obj);
            else
                tempScene.objects[tempIndex] = obj;

            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
