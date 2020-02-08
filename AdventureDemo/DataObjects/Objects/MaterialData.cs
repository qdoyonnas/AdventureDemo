using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class MaterialData : ObjectData
    {
        public double weight = 1;
        public string color = "#ffffff";

        public override Dictionary<string, object> GenerateData()
        {
            Dictionary<string, object> data = base.GenerateData();

            data["weight"] = weight;
            data["color"] = color;

            return data;
        }

        public override object Create()
        {
            Material material = null;

            try {
                material = new Material(GenerateData());
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate Material from MaterialData: {e}");
            }

            return material;
        }
    }
}
