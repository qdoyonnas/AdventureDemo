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

        public bool CanEnter( GameObject obj )
        {
            Connection connection = obj as Connection;
            if( connection == null ) { return false; }

            return connection.CanContain( this );
        }
        public void Enter( GameObject obj )
        {
            Connection connection = obj as Connection;

            connection.Pass(this, this.container);
        }
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
            obj.SetContainer(this);
        }
        public void Drop( GameObject obj )
        {
            if( !DoesContain(obj) ) { return; }

            obj.SetContainer(container);
        }

        public override GameObjectData GetDescription( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            GameObjectData nameData = GetData("name");
            data.text = $"This is {nameData.text}";

            data.SetSpan(
                new Run("This is "),
                nameData.span,
                new Run(".")
            );

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
                if( CanEnter(obj) ) {
                    Utilities.AddContextMenuItem( data.span, "Enter", delegate { Enter(obj); } );
                } else if( DoesContain(obj) ) {
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
