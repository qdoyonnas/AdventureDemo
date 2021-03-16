using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureCore
{
    [JsonConverter(typeof(ScriptReferenceConverter))]
	class ScriptReference
	{
        public string text;

        public ScriptReference() { }
        public ScriptReference( string t )
        {
            text = t;
        }
        public ScriptReference( ScriptReference reference )
        {
            text = reference.text;
        }

        public string GetCode()
        {
            ScriptData data = DataManager.instance.GetScript(text);

            return data.code;
        }
    }

	class ScriptReferenceConverter : JsonConverter
    {
		public override bool CanConvert(Type objectType)
        {
            return typeof(ScriptReference).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            
            return new ScriptReference(token.ToString());
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
	}
}
