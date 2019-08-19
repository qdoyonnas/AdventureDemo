using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WaywardEngine
{
    public class Page
    {
        // FrameworkElement that this Page is connected to
        public FrameworkElement element;

        Vector grabOffset;

        public Page(FrameworkElement element)
        {
            this.element = element;

            // Mouse drag handlers
            element.PreviewMouseDown += OnMouseDown;
            element.PreviewMouseUp += OnMouseUp;

            // Newest Page is always in front (also assures no overlapping zIndex values)
            int maxZ = Utilities.GetMaxZOfCanvas( WaywardManager.instance.window.mainCanvas );
            Canvas.SetZIndex( element, maxZ + 1 );

            SetupContextMenu();
        }

        /// <summary>
        /// Adds default context menu with a close item to element.
        /// </summary>
        private void SetupContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            element.ContextMenu = contextMenu;
            MenuItem closeMenuItem = new MenuItem();
            closeMenuItem.Header = "Close";
            closeMenuItem.Click += CloseAction;
            element.ContextMenu.Items.Insert(0, closeMenuItem );
        }

        private void OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            // If no page is already grabbed
            if( WaywardManager.instance.grabbedPage != null ) { return; }

            WaywardManager.instance.grabbedPage = this;
            grabOffset = new Point( Canvas.GetLeft(element), Canvas.GetTop(element) ) - WaywardManager.instance.GetMousePosition();

            // Add handlers to mainCanvas to avoivd lose of control when moving mouse too fast
            WaywardManager.instance.SetMouseMoveHandler( OnMouseMove, true );
            WaywardManager.instance.SetMouseUpHandler( OnMouseUp, true );

            Utilities.BringToFrontOfCanvas(WaywardManager.instance.window.mainCanvas, element);
        }
        private void OnMouseUp( object sender, MouseButtonEventArgs e )
        {
            if( WaywardManager.instance.grabbedPage != this ) { return; }

            WaywardManager.instance.grabbedPage = null;

            // Remove previously added handlers
            WaywardManager.instance.SetMouseMoveHandler( OnMouseMove, false );
            WaywardManager.instance.SetMouseUpHandler( OnMouseUp, false );
        }

        private void OnMouseMove( object sender, MouseEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            Point offsetPosition = mousePosition + grabOffset;

            Canvas.SetLeft( element, offsetPosition.X );
            Canvas.SetTop( element, offsetPosition.Y );
        }
        
        private void CloseAction( object sender, RoutedEventArgs e )
        {
            WaywardManager.instance.window.mainCanvas.Children.Remove(element);
            WaywardManager.instance.pages.Remove(this);
        }
    }
}
