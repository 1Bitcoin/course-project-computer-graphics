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
    public partial class Form3 : Form
    {
        Scene tempScene = null;
        Bitmap texture = null;
        int tempIndex = 0;


        public Form3(ref Scene scene, int index)
        {
            InitializeComponent();
            tempIndex = index;
            tempScene = scene;

            if (tempIndex != -1)
            {
                this.Text = "Редактирование треугольника";
                Triangle triangle = (Triangle)scene.objects[tempIndex];

                numericUpDown2.Value = Convert.ToDecimal(triangle.points[0][0]);
                numericUpDown1.Value = Convert.ToDecimal(triangle.points[0][1]);
                numericUpDown3.Value = Convert.ToDecimal(triangle.points[0][2]);

                numericUpDown4.Value = Convert.ToDecimal(triangle.points[1][0]);
                numericUpDown8.Value = Convert.ToDecimal(triangle.points[1][1]);
                numericUpDown9.Value = Convert.ToDecimal(triangle.points[1][2]);

                numericUpDown10.Value = Convert.ToDecimal(triangle.points[2][0]);
                numericUpDown11.Value = Convert.ToDecimal(triangle.points[2][1]);
                numericUpDown12.Value = Convert.ToDecimal(triangle.points[2][2]);


                numericUpDown6.Value = Convert.ToDecimal(triangle.specular);
                numericUpDown5.Value = Convert.ToDecimal(triangle.reflective);
                numericUpDown7.Value = Convert.ToDecimal(triangle.transparent);

                this.textBox1.Text = triangle.name;

                byte r = Convert.ToByte(triangle.color[0]);
                byte g = Convert.ToByte(triangle.color[1]);
                byte b = Convert.ToByte(triangle.color[2]);

                Color myRgbColor = new Color();
                myRgbColor = Color.FromArgb(r, g, b);

                colorDialog1.Color = myRgbColor;

                texture = triangle.texture;
            }
            else
            {
                int count = tempScene.GetCountTriangles() + 1;
                this.textBox1.Text = "Треугольник #" + count;
            }          
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double x1 = decimal.ToDouble(numericUpDown2.Value);
            double y1 = decimal.ToDouble(numericUpDown1.Value);
            double z1 = decimal.ToDouble(numericUpDown3.Value);

            double x2 = decimal.ToDouble(numericUpDown4.Value);
            double y2 = decimal.ToDouble(numericUpDown8.Value);
            double z2 = decimal.ToDouble(numericUpDown9.Value);

            double x3 = decimal.ToDouble(numericUpDown10.Value);
            double y3 = decimal.ToDouble(numericUpDown11.Value);
            double z3 = decimal.ToDouble(numericUpDown12.Value);

            double[][] positionTriangle =
            {
                new double[] { x1, y1, z1 },
                new double[] { x2, y2, z2 },
                new double[] { x3, y3, z3 }
            };


            int specular = decimal.ToInt32(numericUpDown6.Value);
            double reflective = decimal.ToDouble(numericUpDown5.Value);
            double transparent = decimal.ToDouble(numericUpDown7.Value);

            double[] color = { colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B };

            if (openFileDialog1.FileName != "openFileDialog1")
                texture = Image.FromFile(openFileDialog1.FileName) as Bitmap;

            Object obj = new Triangle(positionTriangle, color, specular, reflective, transparent, 0, texture, 0, textBox1.Text);

            if (tempIndex == -1)
                tempScene.objects.Add(obj);
            else
                tempScene.objects[tempIndex] = obj;

            Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();

        }
    }
}
