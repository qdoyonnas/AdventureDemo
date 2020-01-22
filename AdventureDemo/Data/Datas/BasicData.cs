using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureDemo
{
    abstract class BasicData
    {
        public string id = null;

        public abstract object Create();
    }
}
