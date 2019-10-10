using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Threading.Tasks;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    /// <summary>
    /// Connects IContainers allowing GameObjects to transfer from one to the other. 
    /// 'Belongs' to one IContainer and connects to others. The connected IContainer can never be null.
    /// Attempting to set the connected IContainer to null will cause it to point to the container of the parent IContainer.
    /// </summary>
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
            : base(name, first)
        {
            SetConnection(second);

            objectData["connection"] = GetDescriptiveConnection;

            relevantData.Insert(0, GetDescriptiveConnection);
        }

        public override bool SetContainer( IContainer newContainer )
        {
            if( _container != null ) { return false; }

            _container = newContainer;
            return true;
        }
        public virtual void SetConnection( IContainer newContainer )
        {
            if( newContainer == null ) {
                GameObject containerObject = newContainer as GameObject;
                if( containerObject != null ) {
                    _containerB = containerObject.container;
                }
            } else {
                _containerB = newContainer;
            }
        }
        public virtual Connection CreateMatching()
        {
            return new Connection(name, containerB, container);
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
        
        public override GameObjectData GetDescription( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            GameObjectData nameData = GetData("name");
            GameObjectData connectionData1 = GetData("connection 0");
            GameObjectData connectionData2 = GetData("connection 1");
            data.text = $"This is a {nameData.text} connecting {connectionData1.text} and {connectionData2.text}";

            data.SetSpan( new Run("This is a "),
                nameData.span,
                new Run(" connecting "),
                connectionData1.span,
                new Run(" and "),
                connectionData2.span,
                new Run(".")
            );

            return data;
        }
        public virtual GameObjectData GetDescriptiveConnection( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            IContainer target = parameters.Length == 0 || parameters[0] == "0" ? container : containerB;

            GameObject obj = target as GameObject;
            if( obj == null ) { return data; }

            GameObjectData nameData = obj.GetData("name");
            data.text = nameData.text;
            data.span = nameData.span;

            return data;
        }
    }
}
