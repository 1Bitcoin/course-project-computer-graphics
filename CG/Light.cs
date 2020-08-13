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
    }

    class AmbientLight : Light // окружающий свет
    {
        public AmbientLight(double intensity)
        {
            this.intensity = intensity;
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
    }

    class DirectionalLight : Light // направленный свет
    {
        public double[] direction;

        public DirectionalLight(double[] direction, double intensity)
        {
            this.direction = direction;
            this.intensity = intensity;
        }
    }
}
