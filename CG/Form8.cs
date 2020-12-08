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
    public partial class Form8 : Form
    {
        SettingModel tempSettingModel = null;

        public Form8(ref SettingModel settingModel)
        {
            InitializeComponent();
            tempSettingModel = settingModel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double specular = decimal.ToDouble(numericUpDown6.Value);
            double reflective = decimal.ToDouble(numericUpDown5.Value);
            double transparent = decimal.ToDouble(numericUpDown7.Value);

            double[] color = { colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B };

            tempSettingModel.specular = specular;
            tempSettingModel.reflective = reflective;
            tempSettingModel.transparent = transparent;

            tempSettingModel.color = color;


            Close();
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
