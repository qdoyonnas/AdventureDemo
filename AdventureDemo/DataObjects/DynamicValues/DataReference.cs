using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureCore
{
    [JsonConverter(typeof(DataReferenceConverter))]
    class DataReference
    {
        public string value = null;

        public DataReference() { }
        public DataReference( string v )
        {
            value = v;
        }
        public DataReference( DataReference data )
        {
            value = data.value;
        }

        public virtual BasicData GetData( Type type )
        {
            return DataManager.instance.GetData(value, type);
        }
        public virtual T LoadData<T>( Type type, Dictionary<string, object> context = null )
            where T : class
        {
            if( type == null ) { return null; }

            return DataManager.instance.LoadObject<T>(value, type, context);
        }
    }

    class DataReferenceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataReference).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            
            return new DataReference(token.ToString());
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
