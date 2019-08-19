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
    public class WaywardManager
    {
        private static WaywardManager _instance;
        public static WaywardManager instance {
            get {
                if( _instance == null ) {
                    _instance = new WaywardManager();
                }

                return _instance;
            }
        }

        public bool isInitialized = false;

        public Application application;
        public MainWindow window;
        public List<Page> pages;

        public Page grabbedPage;

        private WaywardManager()
        {
            pages = new List<Page>();
        }

        public void Init( Application app )
        {
            application = app;
            window = new MainWindow();

            isInitialized = true;
        }

        public Point GetMousePosition()
        {
            return Mouse.GetPosition(window.mainCanvas);
        }

        public void SetMouseMoveHandler( MouseEventHandler handler, bool add )
        {
            if( !isInitialized ) { return; }

            if( add ) {
                window.mainCanvas.MouseMove += handler;
                window.mainCanvas.PreviewMouseMove += handler;
            } else {
                window.mainCanvas.MouseMove -= handler;
                window.mainCanvas.PreviewMouseMove -= handler;
            }
        }
        public void SetMouseUpHandler( MouseButtonEventHandler handler, bool add )
        {
            if( !isInitialized ) { return; }

            if( add ) {
                window.mainCanvas.MouseUp += handler;
                window.mainCanvas.PreviewMouseUp += handler;
            } else {
                window.mainCanvas.MouseUp -= handler;
                window.mainCanvas.PreviewMouseUp -= handler;
            }
        }

        public void AddPage( Page page, Point position )
        {
            window.mainCanvas.Children.Add(page.element);

            Canvas.SetLeft(page.element, position.X);
            Canvas.SetTop(page.element, position.Y);
        }
    }
}
