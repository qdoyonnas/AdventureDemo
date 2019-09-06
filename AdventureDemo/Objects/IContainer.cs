using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    interface IContainer
    {
        GameObject GetContent( int i );
        int ContentCount();
        bool CanContain( GameObject obj );
        bool DoesContain( GameObject obj );
        bool AddContent( GameObject obj );
        bool RemoveContent( GameObject obj );

        List<Connection> GetConnections();
        void AddConnection( Connection connection );
        void RemoveConnection( Connection connection );
    }
}
