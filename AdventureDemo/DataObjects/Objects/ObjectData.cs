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

        public AttachmentType[] attachmentTypes = new AttachmentType[0];

        public VerbReference[] verbs = new VerbReference[0];

        public ObjectData() { }
        public ObjectData( ObjectData data )
            : base(data)
        {
            type = data.type;

            name = new DynamicString(data.name);
            description = data.description;

            attachmentTypes = new AttachmentType[data.attachmentTypes.Length];
            Array.Copy(data.attachmentTypes, attachmentTypes, attachmentTypes.Length);

            verbs = new VerbReference[data.verbs.Length];
            for( int i = 0; i < verbs.Length; i++ ) {
                verbs[i] = new VerbReference(data.verbs[i]);
            }
        }

        public virtual Dictionary<string, object> GenerateData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["name"] = this.name.GetValue(null);
            data["description"] = this.description;

            data["attachmentTypes"] = this.attachmentTypes;

            return data;
        }
        protected override object CreateInstance()
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
