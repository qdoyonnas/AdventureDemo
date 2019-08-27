using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    interface IObserver
    {
        bool CanObserve( GameObject obj );
        GameObjectData Observe( GameObject obj, string key );
        GameObject PointOfView();
    }
}
