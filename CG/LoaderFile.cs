using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Globalization;

namespace CG
{
    class LoaderFile
    {
        struct Vertex
        {
            public double X { set; get; }
            public double Y { set; get; }
            public double Z { set; get; }

            public Vertex(double x, double y, double z) : this()
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
        }

        public static int numbersPolygons = 0;
        static List<Vertex> vertices = new List<Vertex>();  // список вершин
        public static List<Tuple<int, int, int>> polygons = new List<Tuple<int, int, int>>();  // список номеров полигонов

        public static void Load()
        {
            string[] lines = File.ReadAllLines(@"D:\14.obj");
            foreach (string line in lines)
            {
                // строки с вершинами
                if (line.ToLower().StartsWith("v "))
                {
                    double[] vx = ConvertString(line);
                    Console.Write(vx);
                    vertices.Add(new Vertex(200 * vx[0], 200 * vx[1], 200 * vx[2]));

                }
                // строки с номерами
                else if (line.ToLower().StartsWith("f"))
                {
                    var vx = line.Split(' ')
                        .Skip(1)
                        .Select(v => Int32.Parse(v))
                        .ToArray();
                    polygons.Add(new Tuple<int, int, int>(vx[0], vx[1], vx[2]));
                    numbersPolygons++;
                }
            }           
        }

        public static void InitializingTriangles(List<Object> objects)
        {
            double[] tempColor = { 128, 0, 128 };
            Console.WriteLine(numbersPolygons);

            for (int i = 0; i < numbersPolygons; i++)
            {
                int index1 = polygons[i].Item1 - 1;
                int index2 = polygons[i].Item2 - 1;
                int index3 = polygons[i].Item3 - 1;

                /*Console.WriteLine(index1 + 1);
                Console.WriteLine(index2 + 1);
                Console.WriteLine(index3 + 1);*/

                double[][] coordTriangle = {
                                            new double[] { vertices[index1].X, vertices[index1].Y, vertices[index1].Z },
                                            new double[] { vertices[index2].X, vertices[index2].Y, vertices[index2].Z },
                                            new double[] { vertices[index3].X, vertices[index3].Y, vertices[index3].Z }
                };

                objects.Add(new Triangle(coordTriangle, tempColor, 0, 0, 0, 0, null));
                /*Console.WriteLine("triangle: ");
                Console.Write(vertices[index1].X);
                Console.Write(" ");

                Console.Write(vertices[index1].Y);
                Console.Write(" ");

                Console.Write(vertices[index1].Z);
                Console.Write(" ");*/


                /*Console.Write(vertices[index2].X);
                Console.Write(" ");

                Console.Write(vertices[index2].Y);
                Console.Write(" ");

                Console.Write(vertices[index2].Z);
                Console.Write(" ");


                Console.Write(vertices[index3].X);
                Console.Write(" ");

                Console.Write(vertices[index3].Y);
                Console.Write(" ");

                Console.Write(vertices[index3].Z);
                Console.Write(" ");*/

            
                Console.WriteLine();


            }
            
            numbersPolygons = 0;
        }

        public static double[] ConvertString(string s)
        {
            int i = 0;

            while (s[i] == 'v' || s[i] == ' ')
            {
                i++;
            }

            s = s.Substring(i);

            var splitedString = s.Split(' ');
            
         

            double[] answer = { ConvertToDouble(splitedString[0]), ConvertToDouble(splitedString[1]), ConvertToDouble(splitedString[2]) };

            return answer;
         }

        public static double ConvertToDouble(string s)
        {
            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
            }
            catch (Exception e)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        throw new Exception("Wrong string-to-double format");
                    }
                }
            }
            return result;
        }

    }
}

