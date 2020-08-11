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
    public partial class Form1 : Form
    {
        Graphics g;
        Pen testPen = new Pen(Color.Red);

        public Form1()
        {
            InitializeComponent();
            
            g = panel1.CreateGraphics();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            g.DrawLine(testPen, 0, 0, 20, 20);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
