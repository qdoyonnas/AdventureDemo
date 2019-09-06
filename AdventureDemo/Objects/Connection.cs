using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class Connection : GameObject, IContainer
    {
        protected IContainer _containerB;
        public IContainer containerB
        {
            get {
                return _containerB;
            }
        }

        GameObject contained;

        public Connection( string name, IContainer first, IContainer second )
            : base(name)
        {
            SetContainer(first);
            SetSecondContainer(second);
        }

        public virtual bool SetSecondContainer( IContainer newContainer )
        {
            if( newContainer == _containerB ) { return true; }
            if( !newContainer.CanContain(this) ) { return false; }

            if( _containerB == null || _containerB.RemoveContent(this) ) {
                if( newContainer.AddContent(this) ) {
                    _containerB = newContainer;
                } else {
                    _containerB.AddContent(this);
                    return false;
                }
            }

            return true;
        }

        public virtual void Pass( GameObject obj, IContainer origin )
        {
            if( !CanContain(obj) ) { return; }

            IContainer target = origin == container ? containerB : container;

            obj.SetContainer(target);
        }

        public virtual GameObject GetContent( int i )
        {
            return contained;
        }
        public virtual int ContentCount()
        {
            return 1;
        }
        public virtual bool CanContain( GameObject obj )
        {
            if( contained != null ) { return false; }

            return true;
        }
        public virtual bool DoesContain( GameObject obj )
        {
            return obj == contained;
        }
        public virtual bool AddContent( GameObject obj )
        {
            if( !CanContain(obj) ) { return false; }
            
            obj.SetContainer(this);

            return true;
        }
        public virtual bool RemoveContent( GameObject obj )
        {
            contained = null;
            return true;
        }

        public virtual List<Connection> GetConnections()
        {
            throw new NotImplementedException();
        }
        public virtual void AddConnection( Connection connection )
        {
            throw new NotImplementedException();
        }
        public virtual void RemoveConnection( Connection connection )
        {
            throw new NotImplementedException();
        }

        public override GameObjectData GetData( string key )
        {
            GameObjectData data = new GameObjectData();

            switch( key ) {
                case "name":
                    GetName(data);
                    break;
                case "description":
                    GetDescription(data);
                    break;
                case "connection":
                case "connection1":
                    GetDescriptiveConnection(data, 0);
                    break;
                case "connection2":
                    GetDescriptiveConnection(data, 1);
                    break;
                default:
                    break;
            }

            return data;
        }
        public override void GetDescription( GameObjectData data )
        {
            GameObjectData nameData = GetData("name");
            GameObjectData connectionData1 = GetData("connection1");
            GameObjectData connectionData2 = GetData("connection2");
            data.text = $"This is a {nameData.text} connecting {connectionData1.text} and {connectionData2.text}";

            data.span.Inlines.Add( "This is a " );
            data.span.Inlines.Add( nameData.span );
            data.span.Inlines.Add( " connecting " );
            data.span.Inlines.Add( connectionData1.span );
            data.span.Inlines.Add( " and " );
            data.span.Inlines.Add( connectionData2.span );
            data.span.Inlines.Add(".");
        }
        public virtual void GetDescriptiveConnection( GameObjectData data, int i )
        {
            IContainer target = i == 0 ? container : containerB;

            GameObject obj = target as GameObject;
            if( obj == null ) { return; }

            GameObjectData nameData = obj.GetData("name");
            data.text = nameData.text;
            data.span = nameData.span;
        }
    }
}
