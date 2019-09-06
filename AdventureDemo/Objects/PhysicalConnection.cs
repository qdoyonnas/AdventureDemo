using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PhysicalConnection : Connection, IPhysical
    {
        double volume;

        public PhysicalConnection( string name, IContainer first, IContainer second, double volume )
            : base( name, first, second )
        {
            this.volume = volume;
        }

        public override bool CanContain( GameObject obj )
        {
            if( !base.CanContain(obj) ) { return false; }

            IPhysical physical = obj as IPhysical;
            if( physical == null ) { return false; }

            if( physical.GetVolume() > volume ) {
                return false;
            }

            return true;
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
                case "connection":
                case "connection1":
                    GetDescriptiveConnection(data, 0);
                    break;
                case "connection2":
                    GetDescriptiveConnection(data, 1);
                    break;
                default:
                    break;
            }

            return data;
        }
        public override void GetDescription( GameObjectData data )
        {
            base.GetDescription(data);

            GameObjectData volumeData = GetData("volume");

            data.span.Inlines.Add( " It allows " );
            data.span.Inlines.Add( volumeData.span );
            data.span.Inlines.Add(" through.");
        }
        public void GetDescriptiveVolume( GameObjectData data )
        {
            data.text = $"{volume.ToString()} L";

            data.span.Inlines.Add( data.text );
        }
        public void GetDescriptiveWeight( GameObjectData data )
        {
            data.text = "--";

            data.span.Inlines.Add( data.text );
        }

        public double GetVolume()
        {
            return 0;
        }
        public double GetWeight()
        {
            return 0;
        }
    }
}
