using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            if( WaywardManager.instance.pages.Count > 0 ) {
                WaywardManager.instance.pages.Last().RemovedFromTop();
            }
            SetBorderColor("HighlightedBorderBrush");
        }

        public FrameworkElement GetElement()
        {
            return element;
        }

        protected virtual void OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            // If no page is already grabbed
            if( WaywardManager.instance.grabbedPage != null ) { return; }

            WaywardManager.instance.grabbedPage = this;
            grabOffset = new Point( Canvas.GetLeft(element), Canvas.GetTop(element) ) - WaywardManager.instance.GetMousePosition();

            // Add handlers to mainCanvas to avoid lose of control when moving mouse too fast
            WaywardManager.instance.SetMouseMoveHandler( OnMouseMove, true );
            WaywardManager.instance.SetLeftMouseUpHandler( OnMouseUp, true );

            Utilities.BringToFrontOfCanvas(WaywardManager.instance.window.mainCanvas, element);
            SetTop();
        }
        protected virtual void OnMouseUp( object sender, MouseButtonEventArgs e )
        {
            if( WaywardManager.instance.grabbedPage != this ) { return; }

            WaywardManager.instance.grabbedPage = null;

            // Remove previously added handlers
            WaywardManager.instance.SetMouseMoveHandler( OnMouseMove, false );
            WaywardManager.instance.SetLeftMouseUpHandler( OnMouseUp, false );
        }

        protected virtual void OnMouseMove( object sender, MouseEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            Point offsetPosition = mousePosition + grabOffset;

            Canvas.SetLeft( element, offsetPosition.X );
            Canvas.SetTop( element, offsetPosition.Y );
        }

        public void SetTop()
        {
            WaywardManager.instance.pages.Last().RemovedFromTop();

            WaywardManager.instance.pages.Remove(this);
            WaywardManager.instance.pages.Add(this);
            Border pageBorder = element as Border;
            SetBorderColor("HighlightedBorderBrush");
        }
        public void RemovedFromTop()
        {
            SetBorderColor("BorderBrush");
        }
        public void SetBorderColor( string brushKey )
        {
            Border pageBorder = element as Border;
            if( pageBorder != null ) {
                pageBorder.BorderBrush = WaywardManager.instance.GetResource<Brush>(brushKey);
            }
        }

        public virtual bool CloseAction()
        {
            WaywardManager.instance.window.mainCanvas.Children.Remove(element);
            WaywardManager.instance.pages.Remove(this);

            if( WaywardManager.instance.pages.Count > 0 ) {
                WaywardManager.instance.pages.Last().SetBorderColor("HighlightedBorderBrush");
            }

            return false;
        }

        public virtual void Clear()
        {
            // Add Clear behaviour here
        }
        public virtual void Update()
        {
            // Add your update behaviour here
        }
    }
}
