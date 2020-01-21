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

        public virtual Dictionary<string, object> GenerateData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["name"] = this.name.GetValue(null);
            data["description"] = this.description;

            return data;
        }
        public virtual GameObject Create()
        {
            GameObject gameObject = null;

            try {
                gameObject = new GameObject(GenerateData());
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate GameObject from ObjectData: {e}");
            }

            return gameObject;
        }
    }
}
