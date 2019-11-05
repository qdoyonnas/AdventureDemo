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
    class Container : Physical
    {
        public double innerVolume {
            get {
                return contents.capacity;
            }
        }

        public double filledVolume {
            get {
                return contents.filledCapacity;
            }
        }
        public double remainingVolume {
            get {
                return contents.remainingCapacity;
            }
        }

        public new double weight {
            get {
                double totalWeight = 0;
                foreach( KeyValuePair<Material, double> material in materials ) {
                    totalWeight += material.Key.GetWeight( (volume - innerVolume) * material.Value );
                }

                return totalWeight;
            }
        }

        protected ContainerAttachmentPoint contents;

        // TODO: This might be a good place to implement staggered spawn lists (see WorldBuilder.cs)
        // TODO: This should be a dictionary of spawnlists, weights pairs
        public List<SpawnList> spawnLists;

        public Container( Dictionary<string, object> data )
            : base(data)
        {
            Construct( data.ContainsKey("innerVolume") ? (double)data["innerVolume"] : 0 );

            if( data.ContainsKey("spawnLists") ) {
                spawnLists = new List<SpawnList>( (SpawnList[])data["spawnLists"] );
                if( data.ContainsKey("spawnNow") && (bool)data["spawnNow"] ) {
                    SpawnContents();
                }
            }
        }
        public Container( string name, AttachmentPoint container, double innerVolume ) 
            : base(name, container)
        {
            Construct(innerVolume);
        }
        public Container( string name, AttachmentPoint container, double innerVolume, double totalVolume, params KeyValuePair<Material, double>[] mats )
            : base(name, container, totalVolume, mats)
        {
            Construct(innerVolume);
        }
        private void Construct( double volume )
        {
            this.description = DescriptionFromVolume(volume);

            contents = new ContainerAttachmentPoint(new Dictionary<string, object>() {
                { "parent", this }, { "capacity", volume }, { "name", "contents" }
            });
            AddAttachmentPoint(contents);
            spawnLists = new List<SpawnList>();

            objectData["innervolume"] = GetDescriptiveInnerVolume;
            objectData["filledvolume"] = GetDescriptiveFilledVolume;
            objectData["remainingvolume"] = GetDescriptiveRemainingVolume;
            objectData["volumeratio"] = GetDescriptionVolumeRatio;

            relevantData.Insert(0, GetDescriptionVolumeRatio );
            relevantData.Add( GetDescriptiveRemainingVolume );
            relevantData.Add( GetDescriptiveInnerVolume );
            relevantData.Add( GetDescriptiveFilledVolume );
        }
        public string DescriptionFromVolume( double volume )
        {
            switch( volume ) {
                case double.PositiveInfinity:
                    return "a boundless space";
                case double v when v >= 100000:
                    return "a massive chamber";
                case double v when v >= 50000:
                    return "a large chamber";
                case double v when v >= 10000:
                    return "an enormous room";
                case double v when v >= 5000:
                    return "a massive room";
                case double v when v >= 1500:
                    return "a large room";
                case double v when v >= 800:
                    return "a room";
                case double v when v >= 400:
                    return "a small room";
                case double v when v >= 200:
                    return "a closet size space";
                default:
                    return "a container";
            }
        }

        public ContainerAttachmentPoint GetContents()
        {
            return contents;
        }
        public int GetContentCount()
        {
            return contents.GetAttachedCount();
        }
        public Physical GetContent(int i)
        {
            return contents.GetAttachedAsPhysical(i);
        }

        public override double GetWeight()
        {
            double totalWeight = weight;
            foreach( Physical physical in contents.GetAttachedPhysicals() ) {
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

        public override List<DescriptivePageSection> DisplayDescriptivePage()
        {
            List<DescriptivePageSection> sections = base.DisplayDescriptivePage();
            
            sections.Add(new ContainerDescriptivePageSection());

            return sections;
        }

        public override bool SetActor( Actor actor, PossessionType possession )
        {
            bool success = base.SetActor(actor, possession);
            if( !success ) { return false; }

            foreach( GameObject obj in contents.GetAttached() ) {
                obj.CollectVerbs(actor, PossessionType.CONTENT);
            }

            return true;
        }

        public Container SpawnContents(double weight = 1)
        {
            foreach( SpawnList list in spawnLists ) {
                list.Spawn(this, weight);
            }

            return this;
        }
        public Container SpawnContents( SpawnList list, double weight = 1, bool spawnNow = true )
        {
            spawnLists.Add(list);

            if( spawnNow ) {
                list.Spawn(this, weight);
            }

            return this;
        }
        public Container SpawnContents( SpawnList[] lists, double weight = 1, bool spawnNow = true )
        {
            foreach( SpawnList list in lists ) {
                SpawnContents(list, weight, spawnNow);
            }

            return this;
        }

        public Connection[] GetConnections()
        {
            return contents.GetConnections();
        }
        public int GetConnectionsCount()
        {
            return contents.GetConnectionsCount();
        }

        public void AddConnection( Connection connection )
        {
            contents.AddConnection( connection );
        }
        public void AddConnection( Dictionary<string, object> data, bool isTwoWay = true )
        {
            contents.AddConnection(data);
        }
        public void AddConnection( ContainerAttachmentPoint second, double throughput = 0, bool isTwoWay = true )
        {
            contents.AddConnection( second, throughput, isTwoWay );
        }

        public void RemoveConnection( Connection connection )
        {
            contents.RemoveConnection( connection );
        }
    }
}
