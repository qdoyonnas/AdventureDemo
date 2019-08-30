using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Input;

namespace WaywardEngine
{
    public class Page
    {
        // FrameworkElement that this Page is connected to
        protected FrameworkElement element;
        
        Vector grabOffset;

        public Page( string resourceKey )
        {
            // Initialize Page Framework Element
            element = WaywardManager.instance.GetResource<FrameworkElement>(resourceKey);

            // Mouse drag handlers
            element.MouseLeftButtonDown += OnMouseDown;
            element.MouseLeftButtonUp += OnMouseUp;

            // Newest Page is always in front (also assures no overlapping zIndex values)
            int maxZ = Utilities.GetMaxZOfCanvas( WaywardManager.instance.window.mainCanvas );
            Canvas.SetZIndex( element, maxZ + 1 );
        }

        public FrameworkElement GetElement()
        {
            return element;
        }

        protected virtual void OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            Console.WriteLine("IN Page.OnMouseDown");
            // If no page is already grabbed
            if( WaywardManager.instance.grabbedPage != null ) { return; }

            WaywardManager.instance.grabbedPage = this;
            grabOffset = new Point( Canvas.GetLeft(element), Canvas.GetTop(element) ) - WaywardManager.instance.GetMousePosition();

            // Add handlers to mainCanvas to avoivd lose of control when moving mouse too fast
            WaywardManager.instance.SetMouseMoveHandler( OnMouseMove, true );
            WaywardManager.instance.SetLeftMouseUpHandler( OnMouseUp, true );

            Utilities.BringToFrontOfCanvas(WaywardManager.instance.window.mainCanvas, element);
        }
        protected virtual void OnMouseUp( object sender, MouseButtonEventArgs e )
        {
            Console.WriteLine("IN Page.OnMouseUp");
            if( WaywardManager.instance.grabbedPage != this ) { return; }

            WaywardManager.instance.grabbedPage = null;

            // Remove previously added handlers
            WaywardManager.instance.SetMouseMoveHandler( OnMouseMove, false );
            WaywardManager.instance.SetLeftMouseUpHandler( OnMouseUp, false );
        }

        protected virtual void OnMouseMove( object sender, MouseEventArgs e )
        {
            Console.WriteLine("IN Page.OnMouseMove");
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            Point offsetPosition = mousePosition + grabOffset;

            Canvas.SetLeft( element, offsetPosition.X );
            Canvas.SetTop( element, offsetPosition.Y );
        }
        
        public void CloseAction( object sender, RoutedEventArgs e )
        {
            WaywardManager.instance.window.mainCanvas.Children.Remove(element);
            WaywardManager.instance.pages.Remove(this);
        }

        public virtual void Update()
        {
            // Add your update behaviour here
        }
    }
}
