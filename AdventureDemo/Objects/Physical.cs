using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class Physical : GameObject, IPhysical
    {
        double volume;
        double weight;

        public Physical( string name )
            : base(name)
        {
            Construct(0, 0);
        }
        public Physical( string name, double volume )
            : base(name)
        {
            Construct(volume, 0);
        }
        public Physical( string name, double volume, double weight )
            : base(name)
        {
            Construct(volume, weight);
        }
        private void Construct( double volume, double weight )
        {
            this.volume = volume;
            this.weight = weight;
        }

        public double GetVolume()
        {
            return volume;
        }
        public double GetWeight()
        {
            return weight;
        }
    }
}
