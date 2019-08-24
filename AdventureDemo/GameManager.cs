using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Container room1 = new Container( "Dim Room" );
            AddRoot(room1);
            room1.AddContent( new GameObject("You") );
            room1.AddContent( new GameObject("Door") );

            Container table = new Container("Table");
            room1.AddContent(table);
            table.AddContent( new GameObject("Candle") );
            table.AddContent( new GameObject("Mouse") );

            room1.AddContent( new GameObject("Torch") );

            Container room2 = new Container( "Round Room" );
            AddRoot(room2);
            room2.AddContent( new GameObject("Door") );
            room2.AddContent( new Container("Chest") );
            room2.AddContent( new GameObject("Torch") );
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
    }
}
