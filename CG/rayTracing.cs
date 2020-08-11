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
            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
            {
                return;
            }

            map.SetPixel(map.Width / 2 + x, map.Height / 2 - y - 1, color);
        }

        public static int DotProduct(int[] v1, int[] v2)
        {
            return v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
        }

        public static int[] Subtract(int[] v1, int[] v2)
        {
            int[] ans = { v1[0] - v2[0], v1[1] - v2[1], v1[2] - v2[2] };

            return ans;
        }

        public static int[] CanvasToViewport(Bitmap map, int[] p2d)
        {
            int viewportSize = 1;
            int projectionPlane_z = 1;

            int[] ans = { p2d[0] * viewportSize / map.Width, p2d[1] * viewportSize / map.Height,
                projectionPlane_z };

            return ans;
        }


        public static double[] IntersectRaySphere(int[] origin, int[] direction, Sphere sphere)
        {
            int[] oc = Subtract(origin, sphere.center);

            int k1 = DotProduct(direction, direction);
            int k2 = 2 * DotProduct(oc, direction);
            int k3 = DotProduct(oc, oc) - sphere.radius * sphere.radius;

            int discriminant = k2 * k2 - 4 * k1 * k3;

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


}









}
}
