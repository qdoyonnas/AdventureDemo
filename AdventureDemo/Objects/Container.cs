using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaywardEngine;

namespace AdventureDemo
{
    class Container : GameObject, IContainer, IPhysical
    {
        List<IPhysical> contents;
        double weight;
        double volume;
        double innerVolume;

        public double remainingVolume {
            get {
                double totalVolume = 0;
                foreach( IPhysical physical in contents ) {
                    totalVolume += physical.GetVolume();
                }
                return innerVolume - totalVolume;
            }
        }

        public Container( string name, double innerVolume ) 
            : base(name)
        {
            Construct( innerVolume, 0, 0 );
        }
        public Container( string name, double innerVolume, double totalVolume )
            : base(name)
        {
            Construct( innerVolume, totalVolume, 0 );
        }
        public Container( string name, double innerVolume, double totalVolume, double weight )
            : base(name)
        {
            Construct( innerVolume, totalVolume, weight );
        }
        private void Construct( double innerVolume, double totalVolume, double weight )
        {
            contents = new List<IPhysical>();

            this.innerVolume = innerVolume;
            this.volume = totalVolume;
            this.weight = weight;
        }

        public GameObject GetContent( int i )
        {
            IPhysical physical = contents[i];
            GameObject obj = physical as GameObject;
            if( obj == null ) { return null; }

            return obj;
        }
        public int ContentCount()
        {
            return contents.Count;
        }
        public bool CanContain( GameObject obj )
        {
            IPhysical physical = obj as IPhysical;
            if( physical == null ) {
                Utilities.DisplayMessage("Object can not be contained by this sort of container.");
                return false;
            }

            if( physical.GetVolume() > remainingVolume ) {
                Utilities.DisplayMessage("Not enough space.");
                return false;
            }

            return true;
        }
        public bool DoesContain( GameObject obj )
        {
            IPhysical physical = obj as IPhysical;
            if( physical == null ) { return false; }

            return contents.Contains(physical);
        }
        public void AddContent( GameObject obj )
        {
            if( !CanContain(obj) ) { return; }
            IPhysical physical = obj as IPhysical;

            contents.Add(physical);
            obj.container = this;
        }
        public void RemoveContent( GameObject obj )
        {
            IPhysical physical = obj as IPhysical;
            if( physical == null ) { return; }

            // Security check to confirm this came from the relevant gameobject
            if( obj.container == this ) { return; }
            contents.Remove(physical);
        }

        public double GetVolume()
        {
            return volume;
        }
        public double GetWeight()
        {
            double totalWeight = weight;
            foreach( IPhysical physical in contents ) {
                weight += physical.GetWeight();
            }

            return totalWeight;
        }
    }
}
