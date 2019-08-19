using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PlaceholderObject
    {
        public List<PlaceholderObject> contents;
        public string name;
        public string holding;
        public string action;

        public PlaceholderObject( string name, string holding, string action )
        {
            contents = new List<PlaceholderObject>();

            this.name = name;
            this.holding = holding;
            this.action = action;
        }
    }
}
