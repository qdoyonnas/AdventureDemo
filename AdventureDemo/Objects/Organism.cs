using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class Organism : PhysicalAmalgam
    {
        public Organism( Dictionary<string, object> data )
            : base(data)
        {
            Construct();
        }
        public Organism( string name )
            : base()
        {
            Construct();
        }
        private void Construct()
        {
            tags.Add("organism");
        }
    }
}
