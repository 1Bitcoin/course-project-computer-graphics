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
        public abstract string Showtype();
        public abstract double[] Showposition();
    }

    class AmbientLight : Light // окружающий свет
    {
        public string type = "ambient";

        public AmbientLight(double intensity)
        {
            this.intensity = intensity;
        }

        public override string Showtype()
        {
            return type;
        }

        public override double[] Showposition()
        {
            double[] ans = { 0, 0, 0 };
            return ans;
        }

    }

    class PointLight : Light // точечный свет
    {
        public string type = "point";
        public double[] position;

        public PointLight(double[] position, double intensity)
        {
            this.position = position;
            this.intensity = intensity;
        }

        public override string Showtype()
        {
            return type;
        }

        public override double[] Showposition()
        {
            return this.position;
        }
    }

    class DirectionalLight : Light // направленный свет
    {
        public string type = "direction";
        public double[] direction;

        public DirectionalLight(double[] direction, double intensity)
        {
            this.direction = direction;
            this.intensity = intensity;
        }

        public override string Showtype()
        {
            return type;
        }

        public override double[] Showposition()
        {
            return this.direction; // костыль
        }
    }
}
