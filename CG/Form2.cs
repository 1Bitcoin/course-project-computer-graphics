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
        Dictionary<string, double[]> colors = new Dictionary<string, double[]>();
        Scene tempScene = null;

        public Form2(ref Scene scene)
        {
            InitializeComponent();

            colors.Add("Черный", new double[] { 0, 0, 0 });
            colors.Add("Оранжевый", new double[] { 255, 128, 0 });
            colors.Add("Белый", new double[] { 255, 255, 255 });
            colors.Add("Красный", new double[] { 255, 0, 0 });
            colors.Add("Жёлтый", new double[] { 255, 255, 0 });
            colors.Add("Зеленый", new double[] { 0, 255, 0 });
            colors.Add("Светло-серый", new double[] { 240, 240, 240 });

            comboBox1.Items.Add("Белый");
            comboBox1.Items.Add("Красный");
            comboBox1.Items.Add("Зеленый");
            comboBox1.Items.Add("Оранжевый");
            comboBox1.Items.Add("Черный");
            comboBox1.Items.Add("Жёлтый");
            comboBox1.Items.Add("Светло-серый");

            tempScene = scene;

            comboBox1.SelectedIndex = 0;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox1.SelectedItem.ToString();
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

            double[] color = colors[comboBox1.SelectedItem.ToString()];

            Bitmap texture = null;

            if (openFileDialog1.FileName != "openFileDialog1")
                texture = Image.FromFile(openFileDialog1.FileName) as Bitmap;

            Object obj = new Sphere(position, radius, color, specular, reflective, transparent, 0, texture);

            tempScene.objects.Add(obj);

            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
