using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdventureCore
{
    public class Connection : GameObject, IVerbSuggest
    {
        protected Connection _connection;
        public Connection connection {
            get {
                return _connection;
            }
            set {
                _connection = value;
            }
        }

        double _throughput;
        public double throughput {
             get {
                return _throughput;
            }
        }

        public Connection()
            : base()
        {
            Construct();

            _throughput = 0;
            _connection = null;
        }
        public Connection( Dictionary<string, object> data )
            : base(data)
        {
            Construct();

            if( data.ContainsKey("parent") ) {
                _container = data["parent"] as AttachmentPoint;
            }

            if( data.ContainsKey("connection") ) {
                _connection = data["connection"] as Connection;
            }

            if( data.ContainsKey("throughput") ) { 
                _throughput = (double)data["throughput"];
            }

            if( !data.ContainsKey("description") ) {
                description = "an opening";
            }
        }
        public Connection( Connection connection, double throughput )
        {
            Construct();

            _throughput = throughput;
            _connection = connection;
        }
        
        void Construct()
        {
            tags.Add("connection");

            _throughput = 0;
            _connection = null;
        }

        public bool DisplayVerb(Verb verb, FrameworkContentElement span)
        {
            return false;
        }

        public bool SetDefaultVerb(Verb verb, FrameworkContentElement span)
        {
            if( verb.type == "TraversalVerb" ) {
                span.MouseLeftButtonUp += delegate { verb.Register(new Dictionary<string, object>() {{ "target", this }}, true); };
                return true;
            }

            return false;
        }
    }
}
