﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureDemo
{
    [JsonConverter(typeof(DataReferenceConverter))]
    class DataReference
    {
        string value;

        public DataReference( string v )
        {
            value = v;
        }

        public BasicData GetData(Type type)
        {
            return DataManager.instance.GetData(value, type);
        }
        public T LoadData<T>(Type dataType)
            where T : class
        {
            return DataManager.instance.LoadObject<T>(value, dataType);
        }
    }

    class DataReferenceConverter : JsonConverter
    {
        public override bool CanConvert( Type objectType )
        {
            return typeof(DataReference).IsAssignableFrom(objectType);
        }

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            JToken token = JToken.Load(reader);

            return new DataReference(token.ToString());
        }

        public override bool CanWrite => false;
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            throw new NotImplementedException();
        }
    }
}