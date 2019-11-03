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

        public WorldBuilder world;

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

            world = new WorldBuilder();
            world.BuildWorld();
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

        public void DisplayPlayerOverview( Point position )
        {
            OverviewPage page = DisplayOverviewPage( position );
            page.DisplayObject( player.GetControlled() ); // XXX: Find better system
        }
        public void DisplayPlayerVerbose( Point position )
        {
            VerbosePage page = DisplayVerbosePage( position, player.GetControlled() );
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
        public VerbosePage DisplayVerbosePage( Point position, GameObject subject )
        {
            VerbosePage page = new VerbosePage( player, subject );
            WaywardManager.instance.AddPage(page, position);

            return page;
        }
        public DescriptivePage DisplayDescriptivePage( GameObject target )
        {
            DescriptivePageSection[] sections = target.DisplayDescriptivePage().ToArray();
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            DescriptivePage page = new DescriptivePage( player, target, sections );
            WaywardManager.instance.AddPage(page, mousePosition);

            return page;
        }
    }
}
