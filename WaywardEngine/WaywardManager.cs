﻿using System;
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

        /// <summary>
        /// Add passed in page to the window.mainCanvas.
        /// </summary>
        /// <param name="page">Page to be added.</param>
        /// <param name="position">Position of page inside mainCanvas.</param>
        public void AddPage( Page page, Point position )
        {
            window.mainCanvas.Children.Add(page.GetElement());

            Canvas.SetLeft(page.GetElement(), position.X);
            Canvas.SetTop(page.GetElement(), position.Y);

            WaywardManager.instance.pages.Add(page);
        }

        public void DisplayMessage( string message )
        {
            DisplayMessage( message, "Click to close" );
        }
        public void DisplayMessage( string message, string subtext )
        {
            Point position = new Point( WaywardManager.instance.application.MainWindow.Width / 2,
                                    WaywardManager.instance.application.MainWindow.Height / 4 );

            DisplayMessage( message, subtext, position );
        }
        public void DisplayMessage( string message, Point position )
        {
            DisplayMessage( message, "Click to close", position );
        }
        public void DisplayMessage( string message, string subtext, Point position )
        {
            Message box = new Message( message, subtext );
            position = new Point( position.X - ( box.GetElement().ActualWidth / 2 ),
                            position.Y - ( box.GetElement().ActualHeight / 2 ) );
            AddPage( box, position );
        }

        public void StartTutorial()
        {
            DisplayMessage("Welcome to the Wayward Engine!");
        }

        public delegate void UpdateDelegate();
        public event UpdateDelegate OnUpdate;
        public void Update()
        {
            OnUpdate();

            foreach( Page page in pages ) {
                page.Update();
            }
        }
    }
}
