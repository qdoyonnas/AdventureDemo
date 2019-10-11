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

        public PlayerActor player;
        private List<GameObject> rootObjects;

        private GameManager()
        {
            WaywardManager.instance.OnUpdate += Update;

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
            player = new PlayerActor();

            Container space = new Container("Deep Space", null, double.PositiveInfinity);
            Container ship = new Container("Spaceship", space, 10000, 15000, 50*10^8);

            Container room1 = new Container( "Personal Quarters", ship, 1000 );
            new Physical("Bunk", room1, 180, 250);
            
            Container hall1 = new Container( "Hallway", ship, 500 );
            Container toolbox = new Container("Toolbox", hall1, 2, 3, 20);
            new Physical("Spanner", toolbox, 0.5, 2);
            hall1.AddConnection( new PhysicalConnection("Doorway", hall1, room1, 100), true );

            Container room2 = new Container( "Cargo", ship, 4000 );
            Container crate = new Container("Crate", room2, 120, 122, 100);
            new Physical("Robot", crate, 80, 200);
            room2.AddConnection( new PhysicalConnection("Doorway", room2, hall1, 100), true );

            Container room3 = new Container( "Bridge", ship, 2000 );
            new Physical("Console", room3, 200, 300);
            room3.AddConnection( new PhysicalConnection("Doorway", room3, hall1, 100), true );

            Character playerChar = new Character( "You", room1, 2.5, 65, 150 );
            player.Control(playerChar);
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

        public void Update()
        {

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
            page.DisplayObject( player.GetControlled() ); // XXX: Find better system
        }
        public void DisplayRoots( Point position )
        {
            throw new System.NotImplementedException("GameManager.DisplayRoots is not implemented. Observer needs refactor");
        }

        public OverviewPage DisplayOverviewPage( Point position )
        {
            OverviewPage page = new OverviewPage(player);
            WaywardManager.instance.AddPage( page, position );

            return page;
        }
        public DescriptivePage DisplayDescriptivePage( Point position, GameObject target, DescriptivePageSection[] sections )
        {
            DescriptivePage page = new DescriptivePage( player, target, sections );
            WaywardManager.instance.AddPage(page, position);

            return page;
        }
    }
}
