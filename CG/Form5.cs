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
    public partial class Form5 : Form
    {
        Scene tempScene = null;
        int tempIndex = 0;


        public Form5(ref Scene scene, int index)
        {
            InitializeComponent();
            tempIndex = index;
            tempScene = scene;

            if (tempIndex != -1)
            {
                this.Text = "Редактирование направленного источника света";
                tempIndex = index;
                DirectionalLight light = (DirectionalLight)scene.lights[tempIndex];

                numericUpDown2.Value = Convert.ToDecimal(light.direction[0]);
                numericUpDown3.Value = Convert.ToDecimal(light.direction[1]);
                numericUpDown4.Value = Convert.ToDecimal(light.direction[2]);

                numericUpDown5.Value = Convert.ToDecimal(light.intensity);

                this.textBox1.Text = light.name;

            }
            else
            {
                int count = tempScene.GetCountDirectionLights() + 1;
                this.textBox1.Text = "Направл. ист. #" + count;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double x = decimal.ToDouble(numericUpDown2.Value);
            double y = decimal.ToDouble(numericUpDown3.Value);
            double z = decimal.ToDouble(numericUpDown4.Value);

            double[] position = { x, y, z };

            double intensity = decimal.ToDouble(numericUpDown5.Value);

            Light light = new DirectionalLight(position, intensity, textBox1.Text);

            if (tempIndex == -1)
                tempScene.lights.Add(light);
            else
                tempScene.lights[tempIndex] = light;

            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
