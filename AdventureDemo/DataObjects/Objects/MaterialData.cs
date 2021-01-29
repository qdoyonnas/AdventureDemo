using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class MaterialData : ObjectData
    {
        public double weight = 1;
        public string color = "#ffffff";

        public MaterialData() { }
        public MaterialData( MaterialData data )
        {
            weight = data.weight;
            color = data.color;
        }

        public override Dictionary<string, object> GenerateData(Dictionary<string, object> context = null)
        {
            Dictionary<string, object> data = base.GenerateData();

            data["weight"] = weight;
            data["color"] = color;

            return data;
        }

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            Material material = null;

            try {
                material = new Material(GenerateData(context));

                PostInstantiate(material, context);
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate Material from MaterialData: {e}");
            }

            return material;
        }
    }
}
