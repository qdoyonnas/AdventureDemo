using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    class Physical : GameObject
    {
        protected double volume;
        protected double weight;

        protected List<AttachmentPoint> attachmentPoints;

        public Physical( Dictionary<string, object> data )
            : base(data)
        {
            Construct(
                data.ContainsKey("volume") ? (double)data["volume"] : 0,
                data.ContainsKey("weight") ? (double)data["weight"] : 0
            );
        }
        public Physical( string name, Container container )
            : base( name, container )
        {
            Construct(0, 0);
        }
        public Physical( string name, AttachmentPoint container )
            : base(name, container)
        {
            Construct(0, 0);
        }
        public Physical( string name, AttachmentPoint container, double volume )
            : base(name, container)
        {
            Construct(volume, 0);
        }
        public Physical( string name, AttachmentPoint container, double volume, double weight )
            : base(name, container)
        {
            Construct(volume, weight);
        }
        private void Construct( double volume, double weight )
        {
            this.description = "a solid object";

            attachmentPoints = new List<AttachmentPoint>();

            objectData["weight"] = GetDescriptiveWeight;
            objectData["volume"] = GetDescriptiveVolume;

            relevantData.Add(GetDescriptiveVolume);
            relevantData.Add(GetDescriptiveWeight);

            this.volume = volume;
            this.weight = weight;
        }

        public virtual GameObjectData GetDescriptiveWeight( string[] parameters )
        {
            GameObjectData data = new GameObjectData();
            data.text = $"{GetWeight().ToString()} pounds";

            data.SetSpan( data.text );

            return data;
        }
        public virtual GameObjectData GetDescriptiveVolume( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            data.text = $"{GetVolume().ToString()} L";
            data.SetSpan( data.text );

            return data;
        }

        public virtual double GetWeight()
        {
            return weight;
        }
        public virtual double GetVolume()
        {
            return volume;
        }

        public override List<DescriptivePageSection> DisplayDescriptivePage()
        {
            List<DescriptivePageSection> sections = base.DisplayDescriptivePage();

            sections.Add( new PhysicalDescriptivePageSection() );
            sections.Add( new PhysicalAttachmentDescriptivePageSection() );

            return sections;
        }
        
        public AttachmentPoint[] GetAttachmentPoints()
        {
            return attachmentPoints.ToArray();
        }
        public int GetAttachmentCount()
        {
            return attachmentPoints.Count;
        }
        public void AddAttachmentPoint( Dictionary<string, object> data )
        {
            if( !data.ContainsKey("parent") ) {
                data.Add("parent", this);
            }

            attachmentPoints.Add(new PhysicalAttachmentPoint(data) );
        }
        public void AddAttachmentPoint( AttachmentPoint point )
        {
            attachmentPoints.Add(point);
        }
        public void RemoveAttachmentPoint( AttachmentPoint point )
        {
            attachmentPoints.Remove(point);
        }
    }
}
