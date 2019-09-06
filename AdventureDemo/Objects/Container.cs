using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    class Container : Physical, IContainer
    {
        List<Connection> connections;

        List<IPhysical> contents;
        double innerVolume;

        public double filledVolume {
            get {
                double totalVolume = 0;
                foreach( IPhysical physical in contents ) {
                    totalVolume += physical.GetVolume();
                }

                return totalVolume;
            }
        }
        public double remainingVolume {
            get {
                return innerVolume - filledVolume;
            }
        }

        public Container( string name, double innerVolume ) 
            : base(name)
        {
            Construct(innerVolume);
        }
        public Container( string name, double innerVolume, double totalVolume )
            : base(name, totalVolume)
        {
            Construct(innerVolume);
        }
        public Container( string name, double innerVolume, double totalVolume, double weight )
            : base(name, totalVolume, weight)
        {
            Construct(innerVolume);
        }
        private void Construct( double volume )
        {
            connections = new List<Connection>();
            contents = new List<IPhysical>();
            innerVolume = volume;
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
        public bool AddContent( GameObject obj )
        {
            if( !CanContain(obj) ) { return false; }

            IPhysical physical = obj as IPhysical;
            if( physical == null ) { return false; }

            contents.Add(physical);
            return true;
        }
        public bool RemoveContent( GameObject obj )
        {
            IPhysical physical = obj as IPhysical;
            if( physical == null ) { return false; }

            contents.Remove(physical);
            return true;
        }
        
        public List<Connection> GetConnections()
        {
            return connections;
        }
        public void AddConnection( Connection connection )
        {
            connections.Add( connection );
        }
        public void RemoveConnection( Connection connection )
        {
            connections.Remove( connection );
        }

        public override double GetWeight()
        {
            double totalWeight = weight;
            foreach( IPhysical physical in contents ) {
                totalWeight += physical.GetWeight();
            }

            return totalWeight;
        }

        public override GameObjectData GetData( string key )
        {
            GameObjectData data = new GameObjectData();

            switch( key ) {
                case "name":
                    GetName(data);
                    break;
                case "description":
                    GetDescription(data);
                    break;
                case "weight":
                    GetDescriptiveWeight(data);
                    break;
                case "volume":
                    GetDescriptiveVolume(data);
                    break;
                case "innervolume":
                    GetInnerVolume(data);
                    break;
                case "filledvolume":
                    GetFilledVolume(data);
                    break;
                case "remainingvolume":
                    GetRemainingVolume(data);
                    break;
                default:
                    break;
            }

            return data;
        }
        public virtual void GetInnerVolume( GameObjectData data )
        {
            data.text = $"{innerVolume.ToString()} L";
            data.span.Inlines.Add( data.text );
        }
        public virtual void GetFilledVolume( GameObjectData data )
        {
            data.text = $"{filledVolume.ToString()} L";
            data.span.Inlines.Add( data.text );
        }
        public virtual void GetRemainingVolume( GameObjectData data )
        {
            data.text = $"{remainingVolume.ToString()} L";
            data.span.Inlines.Add( data. text );
        }

        public override void DisplayDescriptivePage( object sender, RoutedEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            GameManager.instance.DisplayDescriptivePage( mousePosition, this, new DescriptivePageSection[] {
                new GameObjectDescriptivePageSection(),
                new PhysicalDescriptivePageSection(),
                new ContainerDescriptivePageSection()
            } );
        }
    }
}
