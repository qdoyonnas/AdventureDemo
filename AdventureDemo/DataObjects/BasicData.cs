using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureDemo
{
    abstract class BasicData
    {
        public string id = null;

        public BasicData() { }
        public BasicData( BasicData data )
        {
            id = data.id;
        }

        public abstract object Create();
    }
}
