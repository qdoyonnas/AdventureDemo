using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureCore
{
    public abstract class BasicData
    {
        public string id = null;
        public string referenceId = null;

        public BasicData() { }
        public BasicData( BasicData data )
        {
            id = data.id;
            referenceId = data.id;
        }

        public object Create(Dictionary<string, object> context = null)
        {
            object obj = CreateInstance(context);

            if( !string.IsNullOrEmpty(referenceId) ) {
                GameManager.instance.world.SaveObjectReference(referenceId, obj);
            }

            return obj;
        }
        protected abstract object CreateInstance(Dictionary<string, object> context = null);
    }
}
