using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CG
{
    abstract class Object
    {
        public double[] center;
        public double[] color;
        public double specular;

    }

    class Sphere : Object
    {
        public double radius;

        public Sphere(double[] center, double radius, double[] color, double specular)
        {
            this.center = center;
            this.color = color;
            this.radius = radius;
            this.specular = specular;

            Console.WriteLine("Sphere was a create");
        }
    }
}
