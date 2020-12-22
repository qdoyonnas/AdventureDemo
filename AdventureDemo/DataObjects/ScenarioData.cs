using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureCore
{
    class ScenarioData : BasicData
    {
        public string name = "Unnamed Scenario";
        public string description = "No description";

        public string world = null;

        public LocatorDataReference[] objects = new LocatorDataReference[0];

        public LocatorDataReference playerInfo = null;

        public ScenarioData() {}
        public ScenarioData( ScenarioData data )
            : base(data)
        {
            name = data.name;
            description = data.description;

            world = data.world;

            objects = new LocatorDataReference[data.objects.Length];
            for( int i = 0; i < objects.Length; i++ ) {
                objects[i] = new LocatorDataReference(data.objects[i]);
            }
            playerInfo = new LocatorDataReference(data.playerInfo);
        }

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            return null;
        }
    }
}
