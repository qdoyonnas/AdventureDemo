using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdventureDemo
{
    //[JsonConverter(typeof(MaterialReferenceConverter))]
    class MaterialReference
    {
        public DataReference material;
        public double parts;

        public KeyValuePair<Material, double> GetValue(KeyValuePair<Material, double>[] currentMats)
        {
            Material mat = material.LoadData<Material>(typeof(MaterialData));

            return new KeyValuePair<Material, double>(mat, parts);
        }
    }

    /*class MaterialReferenceConverter : JsonConverter
    {
        public override bool CanConvert( Type objectType )
        {
            return typeof(MaterialReference).IsAssignableFrom(objectType);
        }

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {

        }

        public override bool CanWrite => false;
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            throw new NotImplementedException();
        }
    }*/
}
