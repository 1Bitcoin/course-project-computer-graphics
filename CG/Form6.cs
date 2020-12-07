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
    public partial class Form6 : Form
    {
        Scene tempScene = null;
        int tempIndex = 0;

        public Form6(ref Scene scene, int index)
        {
            InitializeComponent();
            tempScene = scene;
            tempIndex = index;

            if (tempIndex != -1)
            {
                this.Text = "Редактирование окружающего освещения";
                AmbientLight light = (AmbientLight)scene.lights[tempIndex];

                numericUpDown5.Value = Convert.ToDecimal(light.intensity);

                this.textBox1.Text = light.name;

            }
            else
                this.textBox1.Text = "Окружающее освещ.";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double intensity = decimal.ToDouble(numericUpDown5.Value);

            Light light = new AmbientLight(intensity, textBox1.Text);

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
