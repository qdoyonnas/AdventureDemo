using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdventureDemo
{
    class Connection : GameObject, IVerbSuggest
    {
        ContainerAttachmentPoint _secondContainer;
        public ContainerAttachmentPoint secondContainer {
            get {
                return _secondContainer;
            }
            set {
                if( value != null ) {
                    _secondContainer = value;
                } else {
                    ContainerAttachmentPoint parentContainer = container.GetParent().container as ContainerAttachmentPoint;
                    _secondContainer = parentContainer;
                }
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
            _secondContainer = null;
        }
        public Connection( Dictionary<string, object> data )
            : base(data)
        {
            Construct();

            if( data.ContainsKey("second") ) {
                _secondContainer = data["second"] as ContainerAttachmentPoint;
            }

            if( data.ContainsKey("throughput") ) { 
                _throughput = (double)data["throughput"];
            }

            if( !data.ContainsKey("description") ) {
                description = "an opening";
            }
        }
        public Connection( ContainerAttachmentPoint second, double throughput )
        {
            Construct();

            _throughput = throughput;
            _secondContainer = second;
        }
        
        void Construct()
        {
            tags.Add("connection");

            _throughput = 0;
            _secondContainer = null;
        }

        public bool DisplayVerb(Verb verb, FrameworkContentElement span)
        {
            return false;
        }

        public bool SetDefaultVerb(Verb verb, FrameworkContentElement span)
        {
            if( verb is TraversalVerb ) {
                span.MouseLeftButtonDown += delegate { verb.Register(this, true); };
                return true;
            }

            return false;
        }
    }
}
