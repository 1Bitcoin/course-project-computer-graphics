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

        //3d vectors
        public static double[] MultiplyMV(double[,] mat, double[] vec)
        {
            double[] result = { 0, 0, 0 };

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result[i] += vec[j] * mat[i, j];

            return result;

        }

        // Clamps a color to the canonical color range.
        public static int[] Clamp(double[] v)
        {
            int[] ans = { Math.Min(255, Math.Max(0, (int)v[0])),
                             Math.Min(255, Math.Max(0, (int)v[1])),
                             Math.Min(255, Math.Max(0, (int)v[2])) };
            return ans;
        }

        public static double ComputeLighting(Object[] objects, Light[] lights, double[] point, double[] normal, double[] view, double specular)
        {
            double intensity = 0;
            var length_n = Length(normal);  // Should be 1.0, but just in case...
            var length_v = Length(view);

            for (int i = 0; i < lights.Length; i++)
            {
                Light light = lights[i];

                if (light is AmbientLight ambientLight)
                    intensity += light.intensity;

                else
                {
                    double[] vec_l = { 0, 0, 0 };

                    if (light is PointLight pointLight)
                        vec_l = Subtract(pointLight.position, point);

                    if (light is DirectionalLight directionalLight)
                        vec_l = directionalLight.direction;

                    double n_dot_l = DotProduct(normal, vec_l);

                    // Проверка тени

                    double tClosest = Double.PositiveInfinity;
                    Object closestObject = null;

                    ClosestIntersection(objects, ref tClosest, ref closestObject, point, vec_l, 0.001, 
                                        Double.PositiveInfinity); // fix eps

                    if (closestObject != null)
                    {
                        continue;
                    }

                    //Диффузное отражение
                    if (n_dot_l > 0) // иначе не имеет физ.смысла - освещается задняя точка поверхности
                        intensity += light.intensity * n_dot_l / (length_n * Length(vec_l));

                    //Зеркальное отражение
                    if (specular != -1)
                    {
                        var vec_r = Subtract(Multiply(2.0 * DotProduct(normal, vec_l), normal), vec_l);
                        var r_dot_v = DotProduct(vec_r, view);

                        if (r_dot_v > 0)
                        {
                            intensity += light.intensity * Math.Pow(r_dot_v / (Length(vec_r) * length_v), specular);
                        }
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

        // Find the closest intersection between a ray and the spheres in the scene.
        public static void ClosestIntersection(Object[] objects, ref double tClosest, ref Object closestObject, 
                                          double[] origin, double[] direction, double min_t, double max_t)
        {
            tClosest = Double.PositiveInfinity;
            closestObject = null;

            for (int i = 0; i < objects.Length; i++)
            {
                double[] ts = { 0, 0 }; // здесь будут значения t_1, t_2, являющиеся искомыми (пересечение) 
                                        // P = O + t * direction

                if (objects[i] is Sphere sphere)
                    ts = IntersectRaySphere(origin, direction, sphere);

                // поиск ближайшей точки пересечения луча с объектом
                if (ts[0] < tClosest && min_t < ts[0] && ts[0] < max_t)
                {
                    tClosest = ts[0];
                    closestObject = objects[i];
                }

                if (ts[1] < tClosest && min_t < ts[1] && ts[1] < max_t)
                {
                    tClosest = ts[1];
                    closestObject = objects[i];
                }
            }
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


        public static Color TraceRay(Light[] lights, Object[] objects, double[] origin, 
                                    double[] direction, double min_t, double max_t)
        {
            double tClosest = Double.PositiveInfinity;
            Object closestObject = null;

            ClosestIntersection(objects, ref tClosest, ref closestObject, origin, direction, min_t, max_t);

            if (closestObject == null)
                return Color.Black; // background color!!


            double[] point = Add(origin, Multiply(tClosest, direction)); // берем ближайшую точку пересечения лучем объекта
            double[] normal = Subtract(point, closestObject.center); // тут считается нормаль ONLY для сферы в точке point(см.выше)

            normal = Multiply(1.0 / Length(normal), normal); // нормализуем
            var view = Multiply(-1, direction);

            double[] temp = Multiply(ComputeLighting(objects, lights, point, normal, view, closestObject.specular), closestObject.color); 
                                                                                               // вычисляем интенсивность в точке
                                                                                               // и умножаем ее на RGB массив

            int[] newColor = Clamp(temp); // проверка (максимум это (255, 255, 255))

            Color myRgbColor = new Color(); // переводим RGB в Color color

            myRgbColor = Color.FromArgb(newColor[0], newColor[1], newColor[2]);

            return myRgbColor;
        }
    }
  

}
