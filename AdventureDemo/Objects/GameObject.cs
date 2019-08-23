using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class GameObject
    {
        string name;

        public GameObject( string name )
        {
            this.name = name;
        }

        /// <summary>
        /// Returns a String that best fufills the requested data.
        /// Serves a bridge between UI and objects disconnecting the need for explicit calls.
        /// </summary>
        /// <param name="data">A String identifying the desired data.</param>
        /// <returns></returns>
        public virtual GameObjectData GetData( string data )
        {
            GameObjectData objectData = new GameObjectData();

            switch( data.ToLower() ) {
                case "name":
                    return name;
                default:
                    // No relevant data
                    return null;
            }
        }
    }

    struct GameObjectData
    {
        string text;

    }
}
