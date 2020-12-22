using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class PhysicalData : ObjectData
    {
        public DynamicDouble volume = new DynamicDouble("0");
        public MaterialReference[] materials;

        public PhysicalData()
        {
            materials = new MaterialReference[0];
        }
        public PhysicalData( PhysicalData data )
            : base(data)
        {
            volume = new DynamicDouble(data.volume);

            materials = new MaterialReference[data.materials.Length];
            for( int i = 0; i < materials.Length; i++ ) {
                materials[i] = new MaterialReference(data.materials[i]);
            }
        }

        public override Dictionary<string, object> GenerateData(Dictionary<string, object> context = null)
        {
            Dictionary<string, object> data = base.GenerateData(context);

            if( materials != null && materials.Length > 0 ) {
                KeyValuePair<Material, double>[] mats = new KeyValuePair<Material, double>[materials.Length];
                for( int i = 0; i < materials.Length; i++ ) {
                    mats[i] = materials[i].GetValue(mats);
                }
                data["materials"] = mats;
            }

            double v = volume.GetValue(data);
            if( v < 0 ) { v = double.PositiveInfinity; }
            data["volume"] = v;

            return data;
        }
        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            Physical physical = null;

            try {
                physical = new Physical(GenerateData(context));

                // XXX: This is a direct copy of the code in ObjectData.create()
                //      The correct way to do this would be to add copy constructor to every object,
                //      and then new constructors to every object taking the closest base class,
                //      at which point the create functions of dataObjects can chain.
                foreach( VerbReference verbReference in verbs ) {
                    KeyValuePair<Verb, PossessionType> verb = verbReference.GetValue();
                    verb.Key.self = physical;
                    physical.AddVerb(verb.Value, verb.Key);
                }
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate Physical from PhysicalData: {e}");
            }

            return physical;
        }
    }
}
