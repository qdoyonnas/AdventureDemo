using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    class GameManager
    {
        #region Singleton
        public static GameManager _instance;
        public static GameManager instance {
            get {
                if( _instance == null ) {
                    _instance = new GameManager();
                }

                return _instance;
            }
        }
        #endregion

        public AdventureApp application;

        // Prevents most functionality until after init
        public bool isInitialized = false;

        public Character playerObject;
        private List<GameObject> rootObjects;

        private GameManager()
        {
            rootObjects = new List<GameObject>();
        }

        /// <summary>
        /// Init the GameManager and start the game.
        /// </summary>
        /// <param name="app"></param>
        public void Init(AdventureApp app)
        {
            application = app;
            SetupGame();
            isInitialized = true;
        }

        /// <summary>
        /// Generate the world.
        /// </summary>
        private void SetupGame()
        {
            playerObject = new Character( "You", 1, 3 );

            Container room1 = new Container( "First Room", 100 );
            AddRoot(room1);
            room1.AddContent(playerObject);

            Container table = new Container("Table", 6, 12);
            room1.AddContent(table);
            Container box = new Container("Box", 0.5, 0.6);
            table.AddContent(box);
            box.AddContent( new Physical("Key", 0.5) );
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
                throw new System.NullReferenceException($"GameManager could not find '{key}' resource");
            }
            return resource;
        }

        public void AddRoot( GameObject obj )
        {
            rootObjects.Add( obj );
        }
        public void RemoveRoot( GameObject obj )
        {
            rootObjects.Remove( obj );
        }
        public int RootCount()
        {
            return rootObjects.Count;
        }
        public GameObject GetRoot( int i )
        {
            return rootObjects[i];
        }

        public void DisplayPerspectives( Point position )
        {
            OverviewPage page = DisplayOverviewPage( position );
            page.DisplayObject( playerObject );

            page.AddEventPanel("main"); // XXX: This must be made dynamic
            page.DisplayEvent("main", "You wake up.");
        }

        public OverviewPage DisplayOverviewPage( Point position )
        {
            OverviewPage page = new OverviewPage();
            WaywardManager.instance.AddPage( page, position );

            return page;
        }
        public DescriptivePage DisplayDescriptivePage( Point position, GameObject target, DescriptivePageSection[] sections )
        {
            DescriptivePage page = new DescriptivePage( target, sections );
            WaywardManager.instance.AddPage(page, position);

            return page;
        }
    }
}
