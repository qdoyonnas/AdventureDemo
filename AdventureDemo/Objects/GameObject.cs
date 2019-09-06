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
        protected IContainer _container;
        public virtual IContainer container {
            get {
                return _container;
            }
        }

        public GameObject( string name )
        {
            this.name = name;
        }

        public virtual bool SetContainer( IContainer newContainer )
        {
            if( newContainer == container ) { return true; }
            if( !newContainer.CanContain(this) ) { return false; }

            if( _container == null || _container.RemoveContent(this) ) {
                if( newContainer.AddContent(this) ) {
                    _container = newContainer;
                } else {
                    _container.AddContent(this);
                    return false;
                }
            }

            WaywardManager.instance.Update();
            return true;
        }

        /// <summary>
        /// Returns a String that best fufills the requested data.
        /// Serves a bridge between UI and objects disconnecting the need for explicit calls.
        /// </summary>
        /// <param name="data">A String identifying the desired data.</param>
        /// <returns></returns>
        public virtual GameObjectData GetData( string key )
        {
            GameObjectData data = new GameObjectData();

            // XXX: The styling of the text should be done through a WaywardEngine parser
            switch( key.ToLower() ) {
                case "name":
                    GetName(data);
                    break;
                case "description":
                    GetDescription(data);
                    break;
                default:
                    // No relevant data
                    break;
            }

            return data;
        }
        public virtual void GetName( GameObjectData data )
        {
            data.text = name;

            data.span.Inlines.Add( new Run(data.text) );
            data.span.Style = GameManager.instance.GetResource<Style>("Link");
            data.span.MouseLeftButtonUp += DisplayDescriptivePage;

            Utilities.AddContextMenuItem( data.span, "View", DisplayDescriptivePage );
        }
        public virtual void GetDescription( GameObjectData data )
        {
            GameObjectData nameData = GetData("name");
            data.text = $"This is a {nameData.text}";

            data.span.Inlines.Add( "This is a " );
            data.span.Inlines.Add( nameData.span );
            data.span.Inlines.Add(".");
        }

        public virtual void DisplayDescriptivePage( object sender, RoutedEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            GameManager.instance.DisplayDescriptivePage( mousePosition, this, new DescriptivePageSection[] {
                new GameObjectDescriptivePageSection()
            });
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
