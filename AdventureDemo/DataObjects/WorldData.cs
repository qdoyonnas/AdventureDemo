using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class WorldData : BasicData
    {
        public string name = "Unnamed World";
        public string description = "No description";

        public Dictionary<string, DataReference[]> roots;

        public WorldData()
        {
            roots = new Dictionary<string, DataReference[]>();
        }
        public WorldData( WorldData data )
            : base(data)
        {
            name = data.name;
            description = data.description;
            roots = new Dictionary<string, DataReference[]>(data.roots);
        }

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            return null;
        }
    }
}
