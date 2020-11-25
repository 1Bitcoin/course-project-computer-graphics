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

        // Проверочка.
        public static Color Clamp(double[] color)
        {
            int[] ans = { Math.Min(255, Math.Max(0, (int)color[0])),
                             Math.Min(255, Math.Max(0, (int)color[1])),
                             Math.Min(255, Math.Max(0, (int)color[2])) };

            Color myRgbColor = new Color(); // переводим RGB в Color color
            myRgbColor = Color.FromArgb(ans[0], ans[1], ans[2]);

            return myRgbColor;
        }

        // Отраженный относительно нормали вектор.
        public static double[] ReflectRay(double[] vector, double[] normal)
        {
            return MyMath.Subtract(MyMath.Multiply(2 * MyMath.DotProduct(vector, normal), normal), vector);
        }

        public static double ComputeLighting(List<Object> objects, Light[] lights, double[] point, double[] normal, 
                                             double[] view, Object myObject, double[] prevPoint, int flag)
        {
            double intensity = 0;
            var length_n = MyMath.Length(normal);  // Should be 1.0, but just in case...
            var length_v = MyMath.Length(view);

            double t_max = 1;

            for (int i = 0; i < lights.Length; i++)
            {
                Light light = lights[i];

                if (light is AmbientLight ambientLight)
                    intensity += light.intensity;

                else
                {
                    double[] vec_l = { 0, 0, 0 };

                    if (light is PointLight pointLight)
                    {
                        vec_l = MyMath.Subtract(pointLight.position, point);

                        t_max = 1;
                    }

                    if (light is DirectionalLight directionalLight) // check this
                    {
                        vec_l = directionalLight.direction;

                        t_max = Double.PositiveInfinity;
                    }

                    if (light is LightDisk lightdisk)
                    {
                        List<Ray> result = generateDiskRaySet(point, lightdisk);

                        for (int j = 0; j < result.Count; j++)
                        {
                            double[] vector = result[j].direcion;
                        
                            ComputeColor(objects, light, point, normal, view, myObject, 
                                prevPoint, flag, vector, t_max, ref intensity, length_n, length_v);

                        }
                    }
                    else
                    {
                        ComputeColor(objects, light, point, normal, view, myObject, 
                            prevPoint, flag, vec_l, t_max, ref intensity, length_n, length_v);
                    }

                }
            }
            return intensity;
        }

        public static void ComputeColor(List<Object> objects, Light light, double[] point, double[] normal,
                                             double[] view, Object myObject, double[] prevPoint, int flag, double[] vec_l, double t_max,
                                              ref double intensity, double length_n, double length_v)
        {
            double n_dot_l = MyMath.DotProduct(normal, vec_l);

            // Проверка тени
            double tClosest = Double.PositiveInfinity;
            Object closestObject = null;

            ClosestIntersection(objects, ref tClosest, ref closestObject, point, vec_l, 0.001,
                                t_max, flag); // fix eps

            // у прозрачных объектов нет тени (должна быть, зависит от прозрачности!)
            if (closestObject != null)
            {
                /*if (closestObject.transparent > 0)
                {
                    if (intensity != 0)
                    {
                        intensity *= 1.1 + (closestObject.transparent - 1);
                    }

                    if (intensity == 0)
                    {

                    }

                }*/
                return;
            }
            // Диффузное отражение
            if (n_dot_l > 0) // иначе не имеет физ.смысла - освещается задняя точка поверхности
                intensity += light.GetIntensityOnePoint() * n_dot_l / (length_n * MyMath.Length(vec_l));

            // Зеркальное отражение
            if (myObject.specular != 0)
            {
                var vec_r = ReflectRay(vec_l, normal);
                var r_dot_v = MyMath.DotProduct(vec_r, view);

                if (r_dot_v > 0)
                {
                    intensity += light.GetIntensityOnePoint() * Math.Pow(r_dot_v / (MyMath.Length(vec_r) * length_v), myObject.specular);
                }
            }
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public static List<Ray> generateDiskRaySet(double[] origin, LightDisk disk)
        {
            // Результат
            List<Ray> result = new List<Ray>();
            Random random = new Random();

            int segmentationLevel = (int) (disk.radius * 500);
            int countRays = 0;

            for (int i = 0; i < segmentationLevel; i++)
            {
                //Получить случайное число (в диапазоне от 0 до 2pi)
                double angle = random.NextDouble() * 2 * Math.PI;

                //Получить случайное число (в диапазоне от 0 до r)
                double r = disk.radius * Math.Sqrt(random.NextDouble());

                double[] randomPointOnDisk = { disk.position[0] + r * Math.Cos(angle), disk.position[1], disk.position[2] + r * Math.Sin(angle) };

                // Формируем вектор
                double[] vector = MyMath.Subtract(randomPointOnDisk, origin);

                result.Add(new Ray(origin, vector));
                countRays++;
            }


            disk.SetCountPoints(countRays); 

            return result;
        }

        public static List<Ray> generateTriangleRaySet(double[] origin, double[][] points)
        {
            double[] firstRay = MyMath.Subtract(points[0], origin);

            double[] secondRay = MyMath.Subtract(points[1], origin);

            double[] thirdRay = MyMath.Subtract(points[2], origin);

            List<Ray> result = new List<Ray>();

            result.Add(new Ray(origin, firstRay));
            result.Add(new Ray(origin, secondRay));
            result.Add(new Ray(origin, thirdRay));          

            return result;
        }

        public static List<Ray> generateSphereRaySet(double[] origin, LightSphere sphere)
        {
            int segmentationLevel = 6;
            int perSegment = 1;

            // Центральный вектор направленный в сторону сферы
            double[] toSphereCenter = MyMath.Subtract(sphere.position, origin);

            // Нормализованный центральный вектор
            double[] central = MyMath.Multiply(1.0 / MyMath.Length(toSphereCenter), toSphereCenter);

            // Расстрояние до цента источника от начала
            double distance = MyMath.Length(toSphereCenter);

            // Результат
            List<Ray> result = new List<Ray>();
            result.Add(new Ray(origin, toSphereCenter));

            // Получаем первые 4 основные вектора ортогональных основному вектору (направленному к центру сферы)
            double[] deviantion = { 1e-3, 1e-3, 1e-3 };
            double[] buf = MyMath.Add(central, deviantion);

            buf = MyMath.Cross(central, buf);
            double[] v1 = MyMath.Multiply(1.0 / MyMath.Length(buf), buf);

            buf = MyMath.Cross(central, v1);
            double[] v2 = MyMath.Multiply(1.0 / MyMath.Length(buf), buf);

            double[] v3 = MyMath.Multiply(-1, v1);
            double[] v4 = MyMath.Multiply(-1, v2);

            List<double[]> vectors = new List<double[]>();

            // Добавляем вектора в связанный список
            vectors.Add(v1);
            vectors.Add(v2);
            vectors.Add(v3);
            vectors.Add(v4);


            // Если нужно сегментировать (получить вектора находящиеся между)
            if (segmentationLevel > 0)
            {
                // Каждая итерация удваивает кол-во векторов, таким образом окружность сегменитируется
                // и появляется возможность для большей точности
                for (int l = 0; l < segmentationLevel; l++)
                {
                    double[] newVector;
                    int j;
                    for (j = 0; j < vectors.Count - 1; j++)
                    {
                        newVector = MyMath.Add(vectors[j], vectors[j + 1]);
                        vectors.Insert(j + 1, newVector);
                        j++;
                    }
                    newVector = MyMath.Add(vectors[j], vectors[0]);
                    vectors.Insert(0, newVector);

                }
            }

            // Коэффициент пропорциональности, отношение радиуса диска сферы и длинны до центра
            // Он дает понять какое нужно отклонение (какой должна быть длина перпедикулярного вектора) добавить к
            // базовому чтобы новый вектор указывал в сторону границ диска сферы

            double fullBias = (sphere.radius - 0.01f) / distance;

            // Создаем лучи
            int countRays = 0;

            foreach (double[] segmentVector in vectors)
            {
                for (int i = 0; i < perSegment; i++)
                {
                    double cof = (i + 1) / perSegment * fullBias;
                    double[] newBuf = MyMath.Multiply(cof, segmentVector);
                    newBuf = MyMath.Add(newBuf, central);

                    result.Add(new Ray(origin, newBuf));
                    countRays++;
                }
            }
            sphere.SetCountPoints(countRays);

            return result;
        }
        
        public static double[] CanvasToViewport(int width, int height, int[] p2d)
        {
            double viewportSize = 1;
            double projectionPlane_z = 1;

            double[] ans = { p2d[0] * viewportSize / width, p2d[1] * viewportSize / height, projectionPlane_z };

            return ans;
        }

        // Поиск ближайшего пересечения между объектом и лучом.
        public static void ClosestIntersection(List<Object> objects, ref double tClosest, ref Object closestObject, 
                                          double[] origin, double[] direction, double min_t, double max_t, int flag)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                double[] ts = { 0, 0 }; // здесь будут значения t_1, t_2, являющиеся искомыми (пересечение) 
                                        // P = O + t * direction

                if (objects[i] is Sphere sphere)
                    if (flag != 1 || sphere.transparent <= 0)
                        ts = IntersectRaySphere(origin, direction, sphere);

                if (objects[i] is Triangle triangle)
                    if (flag != 1 || triangle.transparent <= 0)
                        ts = IntersectRayTriangle(origin, direction, triangle);

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
            double[] oc = MyMath.Subtract(origin, sphere.center);

            double k1 = MyMath.DotProduct(direction, direction);
            double k2 = 2 * MyMath.DotProduct(oc, direction);
            double k3 = MyMath.DotProduct(oc, oc) - sphere.radius * sphere.radius;

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

        public static double[] IntersectRayTriangle(double[] origin, double[] direction, Triangle triangle)
        {
            double[] answer = { Double.PositiveInfinity, Double.PositiveInfinity };
            double[] pvec = MyMath.Cross(direction, triangle.side2);
            double det = MyMath.DotProduct(triangle.side1, pvec);

            double eps = 1e-6;

            if (det < eps && det > -eps)           
                return answer;
            
            double inv_det = 1.0 / det;
            double[] tvec = MyMath.Subtract(origin, triangle.points[0]);
            double u = inv_det * MyMath.DotProduct(tvec, pvec);

            if (u < 0 || u > 1)
                return answer;

            double[] qvec = MyMath.Cross(tvec, triangle.side1);
            double v = MyMath.DotProduct(direction, qvec) * inv_det;

            if (v < 0 || u + v > 1)          
                return answer;

            double t = MyMath.DotProduct(triangle.side2, qvec) * inv_det;

            answer[0] = t;
            answer[1] = t;

            triangle.u = u;
            triangle.v = v;

            return answer;
        }

        public static double[] ComputeNormalTriangle(Triangle triangle)
        {
            double[] normal = MyMath.Cross(triangle.side1, triangle.side2);
            return normal;
        }

        public static double[] TraceRay(int recursionDepth, Light[] lights, List<Object> objects, double[] origin, 
                                    double[] direction, double min_t, double max_t, int flag)
        {
            //int[] flags = new int[recursionDepth];

            double tClosest = Double.PositiveInfinity;           
            Object closestObject = null;

            ClosestIntersection(objects, ref tClosest, ref closestObject, origin, direction, min_t, max_t, flag);

            if (closestObject == null)
            {
                double[] background = { 0, 0, 0 }; // background color!!
                return background;
            }

            // вычисляем ближайшую точку пересечения лучем объекта
            double[] point = MyMath.Add(origin, MyMath.Multiply(tClosest, direction)); 
            double[] pointEps = MyMath.Add(origin, MyMath.Multiply(tClosest + 0.001, direction));

            double[] normal = { 0, 0, 0 };
            // тут считается нормаль ONLY для сферы в точке point(см.выше)
            if (closestObject is Sphere sphere)
            {
                normal = MyMath.Subtract(point, closestObject.center);
                normal = MyMath.Multiply(1.0 / MyMath.Length(normal), normal); // нормализуем

                if (sphere.texture != null)
                {
                    double u = 0.5 + Math.Atan2(normal[2], normal[0]) / (2 * Math.PI);

                    double v = 0.5 - Math.Asin(normal[1]) / Math.PI;

                    double width = u * closestObject.texture.Width;
                    double height = v * closestObject.texture.Height;

                    Color textureColor = closestObject.texture.GetPixel((int)width, (int)height);

                    byte r = textureColor.R;
                    byte g = textureColor.G;
                    byte b = textureColor.B;

                    double[] res_color = { r, g, b };

                    return res_color;
                }
            }

            if (closestObject is Triangle triangle)
            {
                normal = ComputeNormalTriangle(triangle);

                if (triangle.texture != null)
                {
                    double width = triangle.u * triangle.texture.Width;
                    double height = triangle.v * triangle.texture.Height;

                    Color textureColor = triangle.texture.GetPixel((int)width, (int)height);

                    byte r = textureColor.R;
                    byte g = textureColor.G;
                    byte b = textureColor.B;

                    double[] res_color = { r, g, b };

                    return res_color;
                }
            }          


            var view = MyMath.Multiply(-1, direction);

            double[] newTransparentcolor = { 0, 0, 0 };
            double[] temp = { 0, 0, 0 };

            int newFlag = 0;

            // Прозрачность
            if (closestObject.transparent > 0)
            {
                MyMath.Refract(ref direction, normal, closestObject.refraction);
                newTransparentcolor = TraceRay(recursionDepth, lights, objects, pointEps, direction, 0.001, Double.PositiveInfinity, 1);
                newFlag = 1;
            }
            
            
            // Локальный цвет
            temp = MyMath.Multiply(ComputeLighting(objects, lights, point, normal, view, closestObject, origin, flag),
                                closestObject.color);
            // вычисляем интенсивность в точке
            // и умножаем ее на RGB массив 

            if (newFlag == 1)
                temp = MyMath.Multiply(0.3, temp);



            if (closestObject.reflective <= 0 || recursionDepth <= 0)
            {
                double[] newTemp = MyMath.Add(temp, MyMath.Multiply(closestObject.transparent * 0.6, newTransparentcolor));
                return newTemp;
            }

            // Отраженный цвет
            var reflected_ray = ReflectRay(view, normal);
            var reflected_color = TraceRay(recursionDepth - 1, lights, objects, point, reflected_ray, 
                                           0.001, Double.PositiveInfinity, 0); // fix eps
      
            double[] test = MyMath.Add(MyMath.Multiply(1 - closestObject.reflective, temp),
                   MyMath.Multiply(closestObject.reflective, reflected_color));

            return MyMath.Add(test, MyMath.Multiply(closestObject.transparent, newTransparentcolor));
        }
    } 
}
