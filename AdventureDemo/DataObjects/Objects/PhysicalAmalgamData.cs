using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PhysicalAmalgamData : PhysicalData
    {
        public SpawnEntry[] parts = new SpawnEntry[0];

        public PhysicalAmalgamData() { }
        public PhysicalAmalgamData(PhysicalAmalgamData data)
            : base(data)
        {
            parts = new SpawnEntry[data.parts.Length];
            for( int i = 0; i < parts.Length; i++ ) {
                parts[i] = new SpawnEntry(data.parts[i]);
            }
        }

        public override object Create()
        {
            PhysicalAmalgam amalgam = null;

            try {
                amalgam = new PhysicalAmalgam(GenerateData());
                foreach( SpawnEntry entry in parts ) {
                    Physical part = entry.Spawn(1) as Physical;
                    if( part != null ) {
                        amalgam.AddPart(part);
                    }
                }
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate PhysicalAmalgam from PhysicalAmalgamData: {e}");
            }

            return amalgam;
        }
    }
}
