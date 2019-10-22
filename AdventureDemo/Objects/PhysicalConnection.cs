﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PhysicalConnection : Connection
    {
        double _volume;
        public double volume {
            get {
                return _volume;
            }
        }

        public PhysicalConnection( Dictionary<string, object> data )
            : base( data )
        {
            Construct( data.ContainsKey("volume") ? (double)data["volume"] : 0 );
        }
        public PhysicalConnection( string name, IContainer first, IContainer second, double volume )
            : base( name, first, second )
        {
            Construct(volume);
        }
        void Construct( double volume )
        {
            this._volume = volume;

            objectData["volume"] = GetDescriptiveVolume;

            relevantData.Insert(1, GetDescriptiveVolume);
        }

        public override Connection CreateMatching()
        {
            return new PhysicalConnection(name, secondContainer, container, _volume);
        }

        public override bool CanContain( GameObject obj )
        {
            if( !base.CanContain(obj) ) { return false; }

            IPhysical physical = obj as IPhysical;
            if( physical == null || physical.GetVolume() > _volume ) { return false; }

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
            data.text = $"{_volume.ToString()} L";

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
    }
}