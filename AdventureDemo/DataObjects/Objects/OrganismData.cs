using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class OrganismData : PhysicalAmalgamData
    {
        public OrganismData() { }
        public OrganismData( OrganismData data )
            : base(data)
        { }

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            Organism organism = null;

            try {
                organism = new Organism(GenerateData(context));
                
                PostInstantiate(organism, context);
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate Organism from OrganismData: {e}");
            }

            return organism;
        }
    }
}
