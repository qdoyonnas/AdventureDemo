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

        #region Fields

        // Prevents most functionality until after init
        public bool isInitialized = false;

        public Application application;
        public MainWindow window;
        public List<Page> pages;

        public InputManagerBase inputManager;
        public InputPage inputPage;

        // Mouse grab target (to prevent grabbing more than one page at a time)
        public Page grabbedPage;

        #endregion

        #region Initialization

        private WaywardManager()
        {
            pages = new List<Page>();
        }

        /// <summary>
        /// Initialize the WaywardManager preparing the engine for use.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="resources"></param>
        public void Init( Application app, InputManagerBase inputManager, ResourceDictionary resources )
        {
            application = app;
            window = new MainWindow();

            this.inputManager = inputManager;

            // Load all WaywardEngine resources and pass them to the application
            resources.MergedDictionaries.Add( new ResourceDictionary {
                Source = new Uri("/WaywardEngine;component/ResourceDictionaries/Pages.xaml", UriKind.RelativeOrAbsolute)
            } );

            isInitialized = true;
        }

        /// <summary>
        /// Retrieve resource from application resources."
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetResource<T>( string key )
            where T : class
        {
            if( !isInitialized ) { return null; }

            T resource = application.Resources[key] as T;
            if( resource == null ) {
                throw new System.NullReferenceException($"WaywardManager could not find '{key}' resource");
            }
            return resource;
        }

        #endregion

        #region Update Methods

        public void Update()
        {
            for( int i = pages.Count-1; i >= 0; i-- ) {
                pages[i].Clear();
                pages[i].Update();
            }
        }

        #endregion

        #region Page Methods

        /// <summary>
        /// Add passed in page to the window.mainCanvas.
        /// </summary>
        /// <param name="page">Page to be added.</param>
        /// <param name="position">Position of page inside mainCanvas.</param>
        public void AddPage( Page page, Point position )
        {
            FrameworkElement element = page.GetElement();

            window.mainCanvas.Children.Add(element);
            SetPosition(element, position);

            instance.pages.Add(page);
        }
        public void SetPosition( FrameworkElement element, Point position )
        {
            element.Loaded += (sender, e) =>
            {
                position = new Point(position.X - (element.ActualWidth / 2), position.Y - (element.ActualHeight / 2));
                Canvas.SetLeft(element, position.X);
                Canvas.SetTop(element, position.Y);
            };
        }

        public bool SelectInputPage()
        {
            if( inputPage == null ) {
                Point position = new Point(application.MainWindow.Width / 2,
                                    application.MainWindow.Height * 0.9);

                inputPage = new InputPage(inputManager);
                AddPage(inputPage, position);
            }

            inputPage.Focus();

            return false;
        }

        public void ClearPages()
        {
            for( int i = pages.Count-1; i >= 0; i-- ) {
                pages[i].CloseAction();
            }
        }
        public void ClearPages( Type type )
        {
            if( type == null ) { return; }

            for( int i = pages.Count - 1; i >= 0; i-- ) {
                if( type.IsAssignableFrom(pages[i].GetType()) ) {
                    pages[i].CloseAction();
                }
            }
        }

        public void CloseTopPage()
        {
            pages.Last().CloseAction();
        }

        #region Message Methods

        public Message DisplayMessage( string message )
        {
            return DisplayMessage( message, "Click to close" );
        }
        public Message DisplayMessage( string message, string subtext )
        {
            Point position = new Point( WaywardManager.instance.application.MainWindow.Width / 2,
                                    WaywardManager.instance.application.MainWindow.Height / 4 );

            return DisplayMessage( message, subtext, position );
        }
        public Message DisplayMessage( string message, Point position )
        {
            return DisplayMessage( message, "Click to close", position );
        }
        public Message DisplayMessage( string message, string subtext, Point position )
        {
            Message box = new Message( message, subtext );
            position = new Point( position.X - ( box.GetElement().ActualWidth / 2 ),
                            position.Y - ( box.GetElement().ActualHeight / 2 ) );
            AddPage( box, position );

            return box;
        }

        #endregion

        #endregion

        #region Inputs

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
            } else {
                window.mainCanvas.MouseMove -= handler;
            }
        }
        /// <summary>
        /// Set event handler for MoveUp event on mainCanvas.
        /// </summary>
        /// <param name="handler">MouseEventHandler to be added or removed.</param>
        /// <param name="add">true to add handler, false to remove.</param>
        public void SetLeftMouseUpHandler( MouseButtonEventHandler handler, bool add )
        {
            if( !isInitialized ) { return; }

            if( add ) {
                window.mainCanvas.MouseLeftButtonUp += handler;
            } else {
                window.mainCanvas.MouseLeftButtonUp -= handler;
            }
        }

        #endregion

        #region Utility

        public Point GetRelativeWindowPoint( double x, double y )
        {
            return new Point(WaywardManager.instance.window.ActualWidth * x, WaywardManager.instance.window.ActualHeight * y);
        }

        #endregion

    }
}
