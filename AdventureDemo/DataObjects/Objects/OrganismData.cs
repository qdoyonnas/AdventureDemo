using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class OrganismData : PhysicalAmalgamData
    {
        public OrganismData() { }
        public OrganismData( OrganismData data )
            : base(data)
        { }

        protected override object CreateInstance()
        {
            Organism organism = null;

            try {
                organism = new Organism(GenerateData());
                foreach( SpawnEntry entry in parts ) {
                    GameObject[] parts = entry.Spawn(1);
                    foreach( GameObject part in parts ) {
                        Physical physicalPart = part as Physical;
                        if( physicalPart != null ) {
                            organism.AddPart(physicalPart);
                        }
                    }
                }
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate Organism from OrganismData: {e}");
            }

            return organism;
        }
    }
}
