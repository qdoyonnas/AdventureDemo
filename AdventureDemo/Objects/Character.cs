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
        }
        public void Drop( GameObject obj )
        {
            if( !DoesContain(obj) ) { return; }

            container.AddContent(obj);
        }

        /// <summary>
        /// Returns a String that best fufills the requested data.
        /// Serves a bridge between UI and objects disconnecting the need for explicit calls.
        /// </summary>
        /// <param name="data">A String identifying the desired data.</param>
        /// <returns></returns>
        public override GameObjectData GetData( string key ) // XXX: ...for this reason (see GameObject)
        {
            GameObjectData data = new GameObjectData();

            // XXX: The styling of the text should be done through a WaywardEngine parser
            switch( key.ToLower() ) {
                case "name":
                    data.text = name;

                    data.span.Inlines.Add( new Run(data.text) );
                    data.span.Style = GameManager.instance.GetResource<Style>("Link");
                    data.span.MouseLeftButtonUp += DisplayDescriptivePage;

                    Utilities.AddContextMenuItem( data.span, "View", DisplayDescriptivePage );

                    break;
                case "description":
                    data.text = $"This is {name}";

                    data.span.Inlines.Add( "This is " );
                    data.span.Inlines.Add( GetData("name").span );

                    break;
                default:
                    // No relevant data
                    break;
            }

            return data;
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
