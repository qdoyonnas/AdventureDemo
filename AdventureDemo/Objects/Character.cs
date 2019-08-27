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
    class Character : GameObject, IContainer, IObserver
    {
        List<GameObject> contents;

        public Character( string name ) : base(name)
        {
            contents = new List<GameObject>();
        }

        public void PickUp( GameObject obj )
        {
            obj.container = this;
        }
        public void Drop( GameObject obj )
        {
            if( !DoesContain(obj) ) { return; }

            obj.container = container;
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

        public GameObject GetContent( int i )
        {
            throw new NotImplementedException();
        }
        public int ContentCount()
        {
            return contents.Count;
        }
        public bool DoesContain( GameObject obj )
        {
            return contents.Contains( obj );
        }
        public void AddContent( GameObject obj )
        {
            if( obj.container != this ) { return; }

            contents.Add(obj);
        }
        public void RemoveContent( GameObject obj )
        {
            if( obj.container == this ) { return; }

            contents.Remove(obj);
        }

        public bool CanObserve( GameObject obj )
        {
            return true;
        }
        public GameObjectData Observe( GameObject obj, string key )
        {
            return obj.GetData(key);
        }
        public GameObject PointOfView()
        {
            return container as GameObject;
        }
    }
}
