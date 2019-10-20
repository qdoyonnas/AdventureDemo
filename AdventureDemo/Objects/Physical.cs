using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    class Physical : GameObject, IPhysical
    {
        protected double volume;
        protected double weight;

        public Physical( string name, IContainer container )
            : base(name, container)
        {
            Construct(0, 0);
        }
        public Physical( string name, IContainer container, double volume )
            : base(name, container)
        {
            Construct(volume, 0);
        }
        public Physical( string name, IContainer container, double volume, double weight )
            : base(name, container)
        {
            Construct(volume, weight);
        }
        private void Construct( double volume, double weight )
        {
            this.description = "a solid object";

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
            data.text = $"{GetWeight().ToString()} stones";

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

        public override DescriptivePage DisplayDescriptivePage()
        {
            DescriptivePage page = base.DisplayDescriptivePage();
            
            page.AddSection(new PhysicalDescriptivePageSection());

            return page;
        }

    }
}
