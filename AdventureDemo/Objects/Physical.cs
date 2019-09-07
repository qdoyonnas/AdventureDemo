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

        public Physical( string name )
            : base(name)
        {
            Construct(0, 0);
        }
        public Physical( string name, double volume )
            : base(name)
        {
            Construct(volume, 0);
        }
        public Physical( string name, double volume, double weight )
            : base(name)
        {
            Construct(volume, weight);
        }
        private void Construct( double volume, double weight )
        {
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

        public override void DisplayDescriptivePage( object sender, RoutedEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            GameManager.instance.DisplayDescriptivePage( mousePosition, this, new DescriptivePageSection[] {
                new GameObjectDescriptivePageSection(),
                new PhysicalDescriptivePageSection()
            } );
        }

    }
}
