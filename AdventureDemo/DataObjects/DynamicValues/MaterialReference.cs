using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdventureDemo
{
    class MaterialReference
    {
        public DataReference material;
        public double parts;

        public MaterialReference() { }
        public MaterialReference( MaterialReference mat )
        {
            material = new DataReference(mat.material.value);
            parts = mat.parts;
        }

        public KeyValuePair<Material, double> GetValue(KeyValuePair<Material, double>[] currentMats, Dictionary<string, object> context = null)
        {
            Material mat = material.LoadData<Material>(typeof(MaterialData), context);

            return new KeyValuePair<Material, double>(mat, parts);
        }
    }
}
