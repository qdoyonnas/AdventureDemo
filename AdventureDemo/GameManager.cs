using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class GameManager
    {
        public static GameManager _instance;
        public static GameManager instance {
            get {
                if( _instance == null ) {
                    _instance = new GameManager();
                }

                return _instance;
            }
        }
        public bool isInitialized = false;

        private GameManager()
        {

        }

        public void Init()
        {

            isInitialized = true;
        }
    }
}
