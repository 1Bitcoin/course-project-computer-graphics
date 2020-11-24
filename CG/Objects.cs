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
        public double reflective;
        public double transparent;
        public double refraction;
        public Bitmap texture;
    }

    class Sphere : Object
    {
        public double radius;

        public Sphere(double[] center, double radius, double[] color, 
                      double specular, double reflective, double transparent, double refraction, Bitmap texture)
        {
            this.center = center;
            this.color = color;
            this.radius = radius;
            this.specular = specular;
            this.reflective = reflective;
            this.transparent = transparent;
            this.refraction = refraction;
            this.texture = texture;

            Console.WriteLine("Sphere was a create");
        }
    }

    class Triangle : Object
    {
        public double[][] points;
        public double[] side1;
        public double[] side2;

        public double u;
        public double v;

        public Triangle(double[][] points, double[] color, double specular, double reflective, double transparent, double refraction, Bitmap texture)
        {
            this.center = points[0];
            this.points = points;
            this.color = color;
            this.specular = specular;
            this.reflective = reflective;
            this.transparent = transparent;
            this.refraction = refraction;
            this.texture = texture;
            this.u = 0;
            this.v = 0;

            this.side1 = MyMath.Subtract(points[1], points[0]);
            this.side2 = MyMath.Subtract(points[2], points[0]);

            Console.WriteLine("Triangle was a create");
        }
    }

    class Ray
    {
        public double[] origin;
        public double[] direcion;

        public Ray(double[] origin, double[] direcion)
        {
            this.origin = origin;
            this.direcion = direcion;
        }
    }
}
