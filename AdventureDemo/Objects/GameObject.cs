using System;
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
        string name;

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
        public virtual GameObjectData GetData( string key )
        {
            GameObjectData data = new GameObjectData();

            switch( key.ToLower() ) {
                case "name":
                    data.text = name;
                    data.span.Inlines.Add( new Run(data.text) );
                    data.span.Style = GameManager.instance.GetResource<Style>("Link");
                    data.span.MouseLeftButtonUp += DisplayDescriptivePage;
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

        public virtual void DisplayDescriptivePage( object sender, MouseButtonEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            DescriptivePage page = new DescriptivePage(this);
            WaywardManager.instance.AddPage(page, mousePosition);
        }
    }

    public class GameObjectData
    {
        public string text;
        public Span span;

        public GameObjectData()
        {
            text = string.Empty;
            span = new Span();
        }
    }
}
