using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaywardEngine
{
    // XXX: Deprecated for now until a system like this is needed;
    public class WaywardTime
    {
        public readonly static double momentsInPeriod = 100000;
        public readonly static double periodsInEra = 1000000;

        public double epoch { get; private set; }
        public double era { get; private set; }
        public double period { get; private set; }
        public double moment { get; private set; }

        public void SetEpoch( double epoch )
        {
            if( epoch < 0 ) { return; }

            this.epoch = epoch;
        }

        public void SetEra( double era )
        {
            if( era < 0 ) { return; }

            this.era = era;
            Evaluate();
        }

        private void Evaluate()
        {
            while( moment >= momentsInPeriod ) {
                moment -= momentsInPeriod;
                period++;
            }
        }
    }
}
