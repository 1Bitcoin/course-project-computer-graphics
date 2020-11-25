using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG
{
    abstract class Light
    {
        public double intensity;
        public abstract double GetIntensityOnePoint();
    }

    class AmbientLight : Light // окружающий свет
    {
        public AmbientLight(double intensity)
        {
            this.intensity = intensity;
        }

        public override double GetIntensityOnePoint()
        {
            return intensity;
        }
    }

    class PointLight : Light // точечный свет
    {
        public double[] position;

        public PointLight(double[] position, double intensity)
        {
            this.position = position;
            this.intensity = intensity;
        }

        public override double GetIntensityOnePoint()
        {
            return intensity;
        }
    }

    class DirectionalLight : Light // направленный свет
    {
        public double[] direction;

        public DirectionalLight(double[] direction, double intensity)
        {
            this.direction = direction;
            this.intensity = intensity;
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

        public LightSphere(double[] position, double radius, double intensity)
        {
            this.radius = radius;
            this.position = position;
            this.intensity = intensity;
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

        public LightDisk(double[] position, double radius, double intensity)
        {
            this.radius = radius;
            this.position = position;
            this.intensity = intensity;
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
