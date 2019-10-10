﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using WaywardEngine;

namespace AdventureDemo
{
    class Container : Physical, IContainer
    {
        List<Connection> connections;

        List<IPhysical> contents;
        double innerVolume;

        public double filledVolume {
            get {
                double totalVolume = 0;
                foreach( IPhysical physical in contents ) {
                    totalVolume += physical.GetVolume();
                }

                return totalVolume;
            }
        }
        public double remainingVolume {
            get {
                return innerVolume - filledVolume;
            }
        }

        public Container( string name, IContainer container, double innerVolume ) 
            : base(name, container)
        {
            Construct(innerVolume);
        }
        public Container( string name, IContainer container, double innerVolume, double totalVolume )
            : base(name, container, totalVolume)
        {
            Construct(innerVolume);
        }
        public Container( string name, IContainer container, double innerVolume, double totalVolume, double weight )
            : base(name, container, totalVolume, weight)
        {
            Construct(innerVolume);
        }
        private void Construct( double volume )
        {
            connections = new List<Connection>();
            contents = new List<IPhysical>();
            innerVolume = volume;

            objectData["innervolume"] = GetDescriptiveInnerVolume;
            objectData["filledvolume"] = GetDescriptiveFilledVolume;
            objectData["remainingvolume"] = GetDescriptiveRemainingVolume;
            objectData["volumeratio"] = GetDescriptionVolumeRatio;

            relevantData.Insert(0, GetDescriptionVolumeRatio );
            relevantData.Add( GetDescriptiveRemainingVolume );
            relevantData.Add( GetDescriptiveInnerVolume );
            relevantData.Add( GetDescriptiveFilledVolume );
        }

        public GameObject GetContent( int i )
        {
            IPhysical physical = contents[i];
            GameObject obj = physical as GameObject;
            if( obj == null ) { return null; }

            return obj;
        }
        public int ContentCount()
        {
            return contents.Count;
        }
        public bool CanContain( GameObject obj )
        {
            IPhysical physical = obj as IPhysical;
            if( physical == null ) {
                WaywardManager.instance.DisplayMessage("Object can not be contained by this sort of container.");
                return false;
            }

            if( physical.GetVolume() > remainingVolume ) {
                WaywardManager.instance.DisplayMessage("Not enough space.");
                return false;
            }

            return true;
        }
        public bool DoesContain( GameObject obj )
        {
            IPhysical physical = obj as IPhysical;
            if( physical == null ) { return false; }

            return contents.Contains(physical);
        }
        public bool AddContent( GameObject obj )
        {
            if( !CanContain(obj) ) { return false; }

            IPhysical physical = obj as IPhysical;
            if( physical == null ) { return false; }

            contents.Add(physical);
            return true;
        }
        public bool RemoveContent( GameObject obj )
        {
            IPhysical physical = obj as IPhysical;
            if( physical == null ) { return false; }

            contents.Remove(physical);
            return true;
        }
        
        public List<Connection> GetConnections()
        {
            return connections;
        }
        public void AddConnection( Connection connection, bool createMatchingConnection )
        {
            AddConnection( connection );

            if( createMatchingConnection ) {
                Container containerB = connection.containerB as Container;
                if( containerB == null ) { return; }

                containerB.AddConnection(connection.CreateMatching());
            }
        }
        public void AddConnection( Connection connection )
        {
            connections.Add( connection );
        }
        public void RemoveConnection( Connection connection )
        {
            connections.Remove( connection );
        }

        public override double GetWeight()
        {
            double totalWeight = weight;
            foreach( IPhysical physical in contents ) {
                totalWeight += physical.GetWeight();
            }

            return totalWeight;
        }
        
        public virtual GameObjectData GetDescriptiveInnerVolume( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            data.text = $"{innerVolume.ToString()} L";
            data.SetSpan( data.text );

            return data;
        }
        public virtual GameObjectData GetDescriptiveFilledVolume( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            data.text = $"{filledVolume.ToString()} L";
            data.SetSpan( data.text );

            return data;
        }
        public virtual GameObjectData GetDescriptiveRemainingVolume( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            data.text = $"{remainingVolume.ToString()} L";
            data.SetSpan( data. text );

            return data;
        }
        public virtual GameObjectData GetDescriptionVolumeRatio( string[] parameeters )
        {
            GameObjectData data = new GameObjectData();

            GameObjectData filledData = GetData("filledvolume");
            GameObjectData innerData = GetData("innervolume");

            data.text = $"{filledData.text} / {innerData.text}";
            data.SetSpan(
                filledData.span,
                new Run(" / "),
                innerData.span
            );

            return data;
        }

        public override DescriptivePage DisplayDescriptivePage()
        {
            DescriptivePage page = base.DisplayDescriptivePage();
            
            page.AddSection(new ContainerDescriptivePageSection());

            return page;
        }

        public override bool SetActor( Actor actor, PossessionType possession )
        {
            bool success = base.SetActor(actor, possession);
            if( !success ) { return false; }

            foreach( IPhysical content in contents ) {
                GameObject contentObj = content as GameObject;
                if( contentObj != null ) {
                    contentObj.CollectVerbs(actor, PossessionType.CONTENT);
                }
            }

            return true;
        }
    }
}
