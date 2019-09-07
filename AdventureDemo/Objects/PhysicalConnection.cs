using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
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

            objectData["volume"] = GetDescriptiveVolume;

            relevantData.Insert(1, GetDescriptiveVolume);
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
        
        public override GameObjectData GetDescription( string[] parameters )
        {
            GameObjectData data = base.GetDescription( parameters );

            GameObjectData volumeData = GetData("volume");

            data.AddSpan(
                new Run(" It allows "),
                volumeData.span,
                new Run(" through.")
            );

            return data;
        }
        public GameObjectData GetDescriptiveVolume( string[] parameters )
        {
            GameObjectData data = new GameObjectData();
            data.text = $"{volume.ToString()} L";

            data.SetSpan( data.text );

            return data;
        }
        public GameObjectData GetDescriptiveWeight( string[] parameters )
        {
            GameObjectData data = new GameObjectData();
            data.text = "--";

            data.SetSpan( data.text );

            return data;
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