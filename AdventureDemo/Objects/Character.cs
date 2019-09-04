using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using WaywardEngine;

namespace AdventureDemo
{
    class Character : Container, IObserver
    {
        public Character( string name, double innerVolume ) 
            : base(name, innerVolume)
        { }
        public Character( string name, double innerVolume, double totalVolume )
            : base(name, innerVolume, totalVolume)
        { }
        public Character( string name, double innerVolume, double totalVolume, double weight )
            : base(name, innerVolume, totalVolume, weight)
        { }

        public bool CanPickUp( GameObject obj )
        {
            if( obj == null || obj == this || obj.container == null ) { return false; }

            if( obj.container == this.container ) {
                return true;
            } else {
                GameObject container = obj.container as GameObject;
                if( container != null ) {
                    if( CanPickUp(container) ) {
                        return true;
                    }
                }
            }

            return false;
        }
        public void PickUp( GameObject obj )
        {
            AddContent(obj);

            WaywardManager.instance.Update();
        }
        public void Drop( GameObject obj )
        {
            if( !DoesContain(obj) ) { return; }

            container.AddContent(obj);

            WaywardManager.instance.Update();
        }

        protected override void GetDescription( GameObjectData data )
        {
            data.text = $"This is {name}";

            data.span.Inlines.Add( "This is " );
            data.span.Inlines.Add( GetData("name").span );
        }

        public bool CanObserve( GameObject obj )
        {
            return true;
        }
        public GameObjectData Observe( GameObject obj, string key )
        {
            GameObjectData data = obj.GetData(key);

            if( this == GameManager.instance.playerObject ) { // XXX: playerObject might not always be the actively controlled object?
                if( DoesContain(obj) ) {
                    Utilities.AddContextMenuItem( data.span, "Drop", delegate { Drop(obj); } );
                } else if( CanPickUp(obj) ) {
                    Utilities.AddContextMenuItem( data.span, "Pickup", delegate { PickUp(obj); } );
                }
            }

            return data;
        }
        public GameObject PointOfView()
        {
            return container as GameObject;
        }
    }
}
