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
        void AddContent( GameObject obj );
        void RemoveContent( GameObject obj );
    }
}
