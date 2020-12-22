using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class ConnectionDataReference
    {
        public string linkReference = "";

        public string name = "opening";
        public string description = "an opening";

        public DynamicDouble throughput = new DynamicDouble("0");
        public bool isTwoWay = true;

        public ConnectionDataReference() { }
        public ConnectionDataReference( string linked, string name, string description, string throughput, bool isTwoWay )
        {
            linkReference = linked;
            this.name = name;
            this.description = description;
            
            this.throughput = new DynamicDouble(throughput);
            this.isTwoWay = isTwoWay;
        }
        public ConnectionDataReference( ConnectionDataReference data )
        {
            linkReference = data.linkReference;
            name = data.name;
            description = data.description;

            throughput = new DynamicDouble(data.throughput);
            isTwoWay = data.isTwoWay;
        }
    }
}
