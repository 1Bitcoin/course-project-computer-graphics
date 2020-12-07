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
    public partial class Form4 : Form
    {
        Scene tempScene = null;
        int tempIndex = 0;

        public Form4(ref Scene scene, int index)
        {
            InitializeComponent();
            tempScene = scene;
            tempIndex = index;

            if (tempIndex != -1)
            {
                this.Text = "Редактирование точечного источника света";
                PointLight light = (PointLight)scene.lights[tempIndex];

                numericUpDown2.Value = Convert.ToDecimal(light.position[0]);
                numericUpDown3.Value = Convert.ToDecimal(light.position[1]);
                numericUpDown4.Value = Convert.ToDecimal(light.position[2]);

                numericUpDown5.Value = Convert.ToDecimal(light.intensity);

                numericUpDown5.Value = Convert.ToDecimal(light.intensity);

                this.textBox1.Text = light.name;

            }
            else
            {
                int count = tempScene.GetCountPointLights() + 1;
                this.textBox1.Text = "точечный ист. #" + count;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double x = decimal.ToDouble(numericUpDown2.Value);
            double y = decimal.ToDouble(numericUpDown3.Value);
            double z = decimal.ToDouble(numericUpDown4.Value);

            double[] position = { x, y, z };

            double intensity = decimal.ToDouble(numericUpDown5.Value);

            Light light = new PointLight(position, intensity, textBox1.Text);

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
    }
}
