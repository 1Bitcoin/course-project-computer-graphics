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

        // Length of a 3D vector.
        public static double Length(double[] v)
        {
            return Math.Sqrt(DotProduct(v, v));
        }

        // Computes k * vec.
        public static double[] Multiply(double k, double[] v)
        {
            double[] ans = { k * v[0], k * v[1], k * v[2] };
            return ans;
        }

        // Computes v1 + v2.
        public static double[] Add(double[] v1, double[] v2)
        {
            double[] ans = { v1[0] + v2[0], v1[1] + v2[1], v1[2] + v2[2] };
            return ans;
        }

        public static double[] Subtract(double[] v1, double[] v2)
        {
            double[] ans = { v1[0] - v2[0], v1[1] - v2[1], v1[2] - v2[2] };

            return ans;
        }

        // Clamps a color to the canonical color range.
        public static double[] Clamp(double[] v)
        {
            double[] ans = { Math.Min(255, Math.Max(0, v[0])),
                             Math.Min(255, Math.Max(0, v[1])),
                             Math.Min(255, Math.Max(0, v[2])) };
            return ans;
        }

        public static double ComputeLighting(Light[] lights, double[] point, double[] normal)
        {
            double intensity = 0;
            var length_n = Length(normal);  // Should be 1.0, but just in case...

            for (int i = 0; i < lights.Length; i++)
            {
                var light = lights[i];

                if (light.Showtype() == "ambient")
                {
                    intensity += light.intensity;
                }
                else
                {
                    double[] vec_l;
                    if (light.Showtype() == "point")
                    {
                        vec_l = Subtract(light.Showposition(), point);
                    }
                    else
                    {  // Light.DIRECTIONAL
                        vec_l = light.Showposition();
                    }

                    double n_dot_l = DotProduct(normal, vec_l);
                    if (n_dot_l > 0)
                    {
                        intensity += light.intensity * n_dot_l / (length_n * Length(vec_l));
                    }
                }

            }
            return intensity;
        }             

        public static double[] CanvasToViewport(Bitmap map, int[] p2d)
        {
            double viewportSize = 1;
            double projectionPlane_z = 1;

            double[] ans = { p2d[0] * viewportSize / map.Width, p2d[1] * viewportSize / map.Height, projectionPlane_z };

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


        public static Color TraceRay(Light[] lights, Sphere[] spheres, double[] origin, 
                                    double[] direction, double min_t, double max_t)
        {
            double tClosest = Double.PositiveInfinity;
            Sphere closestSphere = null;

            for (int i = 0; i < spheres.Length; i++)
            {
                double[] ts = IntersectRaySphere(origin, direction, spheres[i]);

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


            double[] point = Add(origin, Multiply(tClosest, direction));
            double[] normal = Subtract(point, closestSphere.center);

            normal = Multiply(1.0 / Length(normal), normal);
            double[] temp = Multiply(ComputeLighting(lights, point, normal), closestSphere.color);

            Color myRgbColor = new Color();

            myRgbColor = Color.FromArgb((int)temp[0], (int)temp[1], (int)temp[2]);

            return myRgbColor;
        }
    }

}
