using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG
{
    public class SettingModel
    {
        public double specular;
        public double reflective;
        public double transparent;

        public double[] color;

        public SettingModel(double specular, double reflective, double transparent, double[] color)
        {
            this.specular = specular;
            this.reflective = reflective;
            this.transparent = transparent;

            this.color = color;

            Console.WriteLine("SettingModel was a create");

        }
    }
}
