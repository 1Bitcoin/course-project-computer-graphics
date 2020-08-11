using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CG
{
    class Objects
    {
        public int[] center;
        public Color color;
    }

    class Sphere : Objects  
    {
        public int radius;

        public Sphere(int[] center, int radius, Color color)
        {
            this.center = center;
            this.color = color;
            this.radius = radius;

            Console.WriteLine("Sphere(int center, int radius, Color color)");
        }

    }
}
