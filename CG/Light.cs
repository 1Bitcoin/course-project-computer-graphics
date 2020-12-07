using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG
{
    public abstract class Light
    {
        public double intensity;
        public abstract double GetIntensityOnePoint();
        public string name;

        public string GetName()
        {
            return this.name;
        }

        public string MyNameLight
        {
            get { return GetName(); }
        }
    }

    class AmbientLight : Light // окружающий свет
    {
        public AmbientLight(double intensity, string name)
        {
            this.intensity = intensity;
            this.name = name;

        }

        public override double GetIntensityOnePoint()
        {
            return intensity;
        }
    }

    class PointLight : Light // точечный свет
    {
        public double[] position;

        public PointLight(double[] position, double intensity, string name)
        {
            this.position = position;
            this.intensity = intensity;
            this.name = name;

        }

        public override double GetIntensityOnePoint()
        {
            return intensity;
        }
    }

    class DirectionalLight : Light // направленный свет
    {
        public double[] direction;

        public DirectionalLight(double[] direction, double intensity, string name)
        {
            this.direction = direction;
            this.intensity = intensity;
            this.name = name;

        }

        public override double GetIntensityOnePoint()
        {
            return intensity;
        }
    }

    class LightSphere : Light
    {
        public double[] position;
        public int countLightpoints = 0;
        public double radius;

        public LightSphere(double[] position, double radius, double intensity, string name)
        {
            this.radius = radius;
            this.position = position;
            this.intensity = intensity;
            this.name = name;

        }

        public override double GetIntensityOnePoint()
        {
            double answer = intensity / countLightpoints;

            return answer;
        }

        public void SetCountPoints(int count)
        {
            this.countLightpoints = count;
        }
    }

    class LightTriangle : Light
    {
        public double[][] points;
        public int countLightpoints = 3;

        public LightTriangle(double[][] points, double intensity)
        {
            this.points = points;
            this.intensity = intensity;
        }

        public override double GetIntensityOnePoint()
        {
            double answer = intensity / countLightpoints;

            return answer;
        }
    }

    class LightDisk : Light
    {
        public double[] position;
        public int countLightpoints = 0;
        public double radius;

        public Triangle triangle;

        public LightDisk(double[] position, double radius, double intensity, string name)
        {
            this.radius = radius;
            this.position = position;
            this.intensity = intensity;        
            this.name = name;

            double lengthSide = 6 * radius / Math.Sqrt(3);

            double[][] positionTriangle =
            {
                new double[] { position[0] - lengthSide / 2, position[1], position[2] - radius },
                new double[] { position[0] + lengthSide / 2, position[1], position[2] - radius },
                new double[] { position[0], position[1], position[2] + Math.Sqrt(Math.Pow(lengthSide / 2, 2) + Math.Pow(radius, 2)) }
            };

            double[] snow = { 255, 255, 255 };

            this.triangle = new Triangle(positionTriangle, snow, 0, 0, 0, 0, null, 1, "helper");

        }

        public override double GetIntensityOnePoint()
        {
            double answer = intensity / countLightpoints;

            return answer;
        }

        public void SetCountPoints(int count)
        {
            this.countLightpoints = count;
        }
    }
}
