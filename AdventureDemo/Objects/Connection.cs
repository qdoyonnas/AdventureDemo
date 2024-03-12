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
        public Connection connectedConnection {
            get {
                return _connection;
            }
            set {
                _connection = value;
            }
        }
        public AttachmentPoint connectedAttachmentPoint
        {
            get {
                if (_connection == null) {
                    return attachPoint.GetParent().attachPoint;
                }
                else {
                    return _connection.attachPoint;
                }
            }
        }
        public GameObject connectedObject
        {
            get {
                return connectedAttachmentPoint.GetParent();
            }
        }

        double _throughput;
        public double naturalThroughput {
            get {
                return _throughput;
            }
        }
        public double actualThroughput {
             get {
                double actual = _throughput;
                foreach(Physical obj in blockingObjects) {
                    actual -= obj.GetVolume();
                }
                return actual;
            }
        }

        public List<Physical> blockingObjects = new List<Physical>();

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
                _attachPoint = data["parent"] as AttachmentPoint;
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

        public GameObject GetBlocking()
        {
            if (blockingObjects.Count == 0) { return null; }

            Physical largestBlocking = blockingObjects[0];
            foreach(Physical obj in blockingObjects) {
                if (obj.GetVolume() > largestBlocking.GetVolume()) {
                    largestBlocking = obj;
                }
            }
            return largestBlocking;
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
