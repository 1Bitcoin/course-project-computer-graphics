using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CG
{
    class RayTracing
    {
        public static void PutPixel(Bitmap map, int x, int y, Color color)
        {
            x = map.Width / 2 + x;
            y = map.Height / 2 - y - 1;

            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
            {
                Console.WriteLine("wtf");

                return;
            }

            map.SetPixel(x, y, color);
        }

        public static double DotProduct(double[] v1, double[] v2)
        {
            return v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
        }

        public static double[] Subtract(double[] v1, double[] v2)
        {
            double[] ans = { v1[0] - v2[0], v1[1] - v2[1], v1[2] - v2[2] };

            return ans;
        }

        public static double[] CanvasToViewport(Bitmap map, int[] p2d)
        {
            double viewportSize = 1;
            double projectionPlane_z = 1;

            double[] ans = { p2d[0] * viewportSize / map.Width, p2d[1] * viewportSize / map.Height, projectionPlane_z };

            if (p2d[0] == 100 && p2d[1] == 100)
            {
                Console.WriteLine(ans[0]);
                Console.WriteLine(ans[1]);

            }

            if (p2d[0] == 123 && p2d[1] == 155)
            {
                Console.WriteLine(ans[0]);
                Console.WriteLine(ans[1]);

            }


            return ans;
        }


        public static double[] IntersectRaySphere(double[] origin, double[] direction, Sphere sphere)
        {
            double[] oc = Subtract(origin, sphere.center);

            double k1 = DotProduct(direction, direction);
            double k2 = 2 * DotProduct(oc, direction);
            double k3 = DotProduct(oc, oc) - sphere.radius * sphere.radius;

            double discriminant = k2 * k2 - 4 * k1 * k3;

            if (discriminant < 0)
            {
                double[] badAns = { Double.PositiveInfinity, Double.PositiveInfinity };
                return badAns;
            }

            double t1 = (-k2 + Math.Sqrt(discriminant)) / (2 * k1);
            double t2 = (-k2 - Math.Sqrt(discriminant)) / (2 * k1);

            double[] goodAns = { t1, t2 };

            return goodAns;
        }


        public static Color TraceRay(Sphere[] spheres, double[] origin, double[] direction, double min_t, double max_t)
        {
            double tClosest = Double.PositiveInfinity;
            Sphere closestSphere = null;

            for (int i = 0; i < spheres.Length; i++)
            {
                var ts = IntersectRaySphere(origin, direction, spheres[i]);

                if (ts[0] < tClosest && min_t < ts[0] && ts[0] < max_t)
                {
                    tClosest = ts[0];
                    closestSphere = spheres[i];
                }

                if (ts[1] < tClosest && min_t < ts[1] && ts[1] < max_t)
                {
                    tClosest = ts[1];
                    closestSphere = spheres[i];
                }
            }

            if (closestSphere == null)
                return Color.Black; // background color!!

            return closestSphere.color;
        }
    }

}
