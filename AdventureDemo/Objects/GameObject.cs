using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;
using WaywardEngine;

namespace AdventureDemo
{
    class GameObject
    {
        protected string name;
        IContainer _container;
        public IContainer container {
            get {
                return _container;
            }
            set {
                // Security check
                if( !value.DoesContain(this) ) { return; }
                IContainer oldContainer = _container;
                _container = value;
                if( oldContainer != null ) { oldContainer.RemoveContent(this); }

                WaywardManager.instance.Update(); // XXX: This probably shouldnt be handled here
            }
        }

        public GameObject( string name )
        {
            this.name = name;
        }

        /// <summary>
        /// Returns a String that best fufills the requested data.
        /// Serves a bridge between UI and objects disconnecting the need for explicit calls.
        /// </summary>
        /// <param name="data">A String identifying the desired data.</param>
        /// <returns></returns>
        public virtual GameObjectData GetData( string key ) // XXX: Internals of GetData should be divided to individually overridable functions
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
                    data.text = $"This is a {name}";

                    data.span.Inlines.Add( "This is a " );
                    data.span.Inlines.Add( GetData("name").span );

                    break;
                default:
                    // No relevant data
                    break;
            }

            return data;
        }

        public virtual void DisplayDescriptivePage( object sender, RoutedEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            DescriptivePage page = new DescriptivePage(this);
            WaywardManager.instance.AddPage(page, mousePosition);
        }
    }

    public class GameObjectData
    {
        public string text;
        public TextBlock span;

        public GameObjectData()
        {
            text = string.Empty;
            span = new TextBlock();
        }
    }
}
