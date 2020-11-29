using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG
{
    class MyMath
    {
        public static double[,] getMatrixOx(double angleX)
        {
            double[,] cameraRotationOX = {
                                        { 1, 0, 0 },
                                        { 0, Math.Cos(Math.PI * angleX / 180.0), -Math.Sin(Math.PI * angleX / 180.0) },
                                        { 0, Math.Sin(Math.PI * angleX / 180.0), Math.Cos(Math.PI * angleX / 180.0)}
                                        };
            return cameraRotationOX;
        }

        public static double[,] getMatrixOy(double angleY)
        {
            double[,] cameraRotationOY = {
                                        { Math.Cos(Math.PI * angleY / 180.0), 0, Math.Sin(Math.PI * angleY / 180.0) },
                                        { 0, 1, 0 },
                                        { -Math.Sin(Math.PI * angleY / 180.0), 0, Math.Cos(Math.PI * angleY / 180.0)}
                                        };
            return cameraRotationOY;
        }


        public static double[,] getMatrixOz(double angleZ)
        {
            double[,] cameraRotationOZ = {
                                        { Math.Cos(Math.PI * angleZ / 180.0), -Math.Sin(Math.PI * angleZ / 180.0), 0 },
                                        { Math.Sin(Math.PI * angleZ / 180.0), Math.Cos(Math.PI * angleZ / 180.0), 0 },
                                        { 0, 0, 1}
                                        };
            return cameraRotationOZ;
        }

        public static double angleVectors(double[] v1, double[] v2)
        {
            double numerator = v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
            double denominator = Math.Sqrt(v1[0] * v1[0] + v1[1] * v1[1] + v1[2] * v1[2]) * 
                                 Math.Sqrt(v2[0] * v2[0] + v2[1] * v2[1] + v2[2] * v2[2]);

            return Math.Acos(numerator / denominator);
        }

        public static double Clamp(double lo, double hi, double v) 
        {
            return Math.Max(lo, Math.Min(hi, v));
        }

        public static void Refract(ref double[] direction, double[] normal, double refraction)
        {
            double cosi = Clamp(-1, 1, DotProduct(normal, direction));
            double etai = 1, etat = refraction;
            double[] n = normal;

            if (cosi < 0)
            {
                cosi = -cosi;
            }
            else
            {
                Swap(ref etai, ref etat);
                ChangeSign(ref n);
            }
            double eta = etai / etat;
            double k = 1 - eta * eta * (1 - cosi * cosi);

            if (k >= 0)
                direction = Add(Multiply(eta, direction), 
                            Multiply((eta * cosi - Math.Sqrt(k)), n));
        }

        public static void Swap(ref double first, ref double second)
        {
            double buf = first;

            first = second;

            second = buf;
        }

        public static void ChangeSign(ref double[] mass) 
        {
            for (int i = 0; i < mass.Length; i++)
            {
                mass[i] = -mass[i];
            }
        }

        public static double DotProduct(double[] v1, double[] v2)
        {
            return v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
        }

        // Длина 3д вектора.
        public static double Length(double[] v)
        {
            return Math.Sqrt(DotProduct(v, v));
        }

        // Перекрестное умножение векторов
        public static double[] Cross(double[] A, double[] B)
        {
            double resultX = A[1] * B[2] - A[2] * B[1];
            double resultY = A[2] * B[0] - A[0] * B[2];
            double resultZ = A[0] * B[1] - A[1] * B[0];

            double[] result = { resultX, resultY, resultZ };

            return result;
        }


        // Вычисляем k * vec.
        public static double[] Multiply(double k, double[] v)
        {
            double[] ans = { k * v[0], k * v[1], k * v[2] };
            return ans;
        }

        // Вычисляем v1 + v2.
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

        // матрица поворота на вектор
        public static double[] MultiplyMV(double[,] mat, double[] vec)
        {
            double[] result = { 0, 0, 0 };

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result[i] += vec[j] * mat[i, j];

            return result;

        }
    }
}
