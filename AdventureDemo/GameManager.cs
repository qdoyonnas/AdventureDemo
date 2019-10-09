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

        public Actor player;
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
            player = new Actor();
            Character playerChar = new Character( "You", 2.5, 65, 11 );
            player.Control(playerChar);

            Container room1 = new Container( "First Room", 1000 );
            AddRoot(room1);
            playerChar.SetContainer(room1);

            Container table = new Container("Table", 70, 120, 7);
            table.SetContainer(room1);
            Container box = new Container("Box", 2, 2.5, 1);
            box.SetContainer(table);
            new Physical("Key", 0.1, 0.1).SetContainer(box);

            Container room2 = new Container( "Second Room", 500 );
            AddRoot(room2);

            PhysicalConnection doorway = new PhysicalConnection( "Doorway", room1, room2, 70 );
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
            page.DisplayObject( player.GetControlled().container as Container ); // XXX: Find better system
        }
        public void DisplayRoots( Point position )
        {
            throw new System.NotImplementedException("GameManager.DisplayRoots is not implemented. Observer needs refactor");
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
