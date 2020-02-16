using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureDemo
{
    class ObjectData : BasicData
    {
        public string type = "ObjectData";

        public DynamicString name = new DynamicString("unknown object");
        public string description = "a strange object";

        public VerbReference[] verbs;

        public ObjectData() { }
        public ObjectData( ObjectData data )
            : base(data)
        {
            type = data.type;
            name = new DynamicString(data.name);
        }

        public virtual Dictionary<string, object> GenerateData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["name"] = this.name.GetValue(null);
            data["description"] = this.description;

            return data;
        }
        public override object Create()
        {
            GameObject gameObject = null;

            try {
                gameObject = new GameObject(GenerateData());
                foreach( VerbReference verbReference in verbs ) {
                    KeyValuePair<Verb, PossessionType> verb = verbReference.GetValue();
                    verb.Key.self = gameObject;
                    gameObject.AddVerb(verb.Value, verb.Key);
                }
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate GameObject from ObjectData: {e}");
            }

            return gameObject;
        }
    }
}
