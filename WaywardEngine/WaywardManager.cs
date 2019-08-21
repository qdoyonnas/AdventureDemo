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
        #region Singleton
        private static WaywardManager _instance;
        public static WaywardManager instance {
            get {
                if( _instance == null ) {
                    _instance = new WaywardManager();
                }

                return _instance;
            }
        }
        #endregion

        // Prevents most functionality until after init
        public bool isInitialized = false;

        public Application application;
        public MainWindow window;
        public List<Page> pages;

        // Mouse grab target (to prevent grabbing more than one page at a time)
        public Page grabbedPage;

        private WaywardManager()
        {
            pages = new List<Page>();
        }

        /// <summary>
        /// Initialize the WaywardManager preparing the engine for use.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="resources"></param>
        public void Init( Application app, ResourceDictionary resources )
        {
            application = app;
            window = new MainWindow();

            // Load all WaywardEngine resources and pass them to the application
            resources.MergedDictionaries.Add( new ResourceDictionary {
                Source = new Uri("/WaywardEngine;component/ResourceDictionaries/Pages.xaml", UriKind.RelativeOrAbsolute)
            } );

            isInitialized = true;
        }

        /// <summary>
        /// Return mouse position relative to window.mainCanvas.
        /// </summary>
        /// <returns></returns>
        public Point GetMousePosition()
        {
            return Mouse.GetPosition(window.mainCanvas);
        }

        /// <summary>
        /// Set event handler for MouseMove event on mainCanvas.
        /// </summary>
        /// <param name="handler">MouseEventHandler to be added or removed.</param>
        /// <param name="add">true to add handler, false to remove.</param>
        public void SetMouseMoveHandler( MouseEventHandler handler, bool add )
        {
            if( !isInitialized ) { return; }

            if( add ) {
                window.mainCanvas.MouseMove += handler;
                window.mainCanvas.PreviewMouseMove += handler; // PreviewMouseMove borrows down to lower elements
            } else {
                window.mainCanvas.MouseMove -= handler;
                window.mainCanvas.PreviewMouseMove -= handler;
            }
        }
        /// <summary>
        /// Set event handler for MoveUp event on mainCanvas.
        /// </summary>
        /// <param name="handler">MouseEventHandler to be added or removed.</param>
        /// <param name="add">true to add handler, false to remove.</param>
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

        /// <summary>
        /// Add passed in page to the window.mainCanvas.
        /// </summary>
        /// <param name="page">Page to be added.</param>
        /// <param name="position">Position of page inside mainCanvas.</param>
        public void AddPage( Page page, Point position )
        {
            window.mainCanvas.Children.Add(page.element);

            Canvas.SetLeft(page.element, position.X);
            Canvas.SetTop(page.element, position.Y);

            WaywardManager.instance.pages.Add(page);
        }
    }
}
