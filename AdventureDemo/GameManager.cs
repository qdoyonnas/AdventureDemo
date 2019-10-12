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
            Container ship = new Container("Spaceship", space, 100000, 150000, 50*10^8);

            Container mainElevator = new Container( "Elevator", ship, 1500, 1550 );

            Container crewHallway = new Container( "Quarters Hallway", ship, 1000, 1050 );
            crewHallway.AddConnection( new PhysicalConnection("Entrance", crewHallway, mainElevator, 100) );
            
            Container quartersA1 = new Container( "Quarters A1", ship, 500, 550 );
            quartersA1.AddConnection( new PhysicalConnection("Doorway", quartersA1, crewHallway, 100), true );
            Container quartersA2 = new Container( "Quarters A2", ship, 500, 550 );
            quartersA2.AddConnection( new PhysicalConnection("Doorway", quartersA2, crewHallway, 100), true );
            Container quartersA3 = new Container( "Quarters A3", ship, 500, 550 );
            quartersA3.AddConnection( new PhysicalConnection("Doorway", quartersA3, crewHallway, 100), true );
            Container quartersB1 = new Container( "Quarters B1", ship, 500, 550 );
            quartersB1.AddConnection( new PhysicalConnection("Doorway", quartersB1, crewHallway, 100), true );
            Container quartersB2 = new Container( "Quarters B2", ship, 500, 550 );
            quartersB2.AddConnection( new PhysicalConnection("Doorway", quartersB2, crewHallway, 100), true );
            Container quartersB3 = new Container( "Quarters B3", ship, 500, 550 );
            quartersB3.AddConnection( new PhysicalConnection("Doorway", quartersB3, crewHallway, 100), true );

            Container messhall = new Container( "Mess Hall", ship, 1200, 1250 );
            messhall.AddConnection( new PhysicalConnection("Doorway", messhall, crewHallway, 100), true );
            Container kitchen = new Container( "Kitchen", ship, 800, 850 );
            kitchen.AddConnection( new PhysicalConnection("Doorway", kitchen, messhall, 100), true);

            Container cargoHallway = new Container("Cargo Hallway", ship, 1000, 1050);
            cargoHallway.AddConnection( new PhysicalConnection("Entrance", cargoHallway, mainElevator, 100) );

            Container cargo1 = new Container("Cargo Bay 1", ship, 2000, 2050);
            cargo1.AddConnection( new PhysicalConnection("Bay doors", cargo1, cargoHallway, 100) );
            Container cargo2 = new Container("Cargo Bay 2", ship, 2000, 2050);
            cargo1.AddConnection( new PhysicalConnection("Bay doors", cargo2, cargoHallway, 100) );
            Container cargo3 = new Container("Cargo Bay 3", ship, 2000, 2050);
            cargo1.AddConnection( new PhysicalConnection("Bay doors", cargo3, cargoHallway, 100) );
            Container cargo4 = new Container("Cargo Bay 4", ship, 2000, 2050);
            cargo1.AddConnection( new PhysicalConnection("Bay doors", cargo4, cargoHallway, 100) );

            Character playerChar = new Character( "You", crewHallway, 2.5, 65, 150 );
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
