using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureDemo
{
    class ScenarioData : BasicData
    {
        public string name = "Unnamed Scenario";
        public string description = "No description";
        public Dictionary<string, DataReference[]> roots;

        public ScenarioData()
        {
            roots = new Dictionary<string, DataReference[]>();
        }
        public ScenarioData( ScenarioData data )
            : base(data)
        {
            name = data.name;
            description = data.description;
            roots = new Dictionary<string, DataReference[]>(data.roots);
        }

        public override object Create()
        {
            return new WorldManager(this);
        }
    }
}
