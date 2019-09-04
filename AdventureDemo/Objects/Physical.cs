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
            this.volume = volume;
            this.weight = weight;
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
                case "weight":
                    GetDescriptiveWeight(data);
                    break;
                case "volume":
                    GetDescriptiveVolume(data);
                    break;
                default:
                    break;
            }

            return data;
        }
        protected virtual void GetDescriptiveWeight( GameObjectData data )
        {
            data.text = GetWeight().ToString();

            data.span.Inlines.Add( data.text );
        }
        protected virtual void GetDescriptiveVolume( GameObjectData data )
        {
            data.text = GetVolume().ToString();

            data.span.Inlines.Add( data.text );
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
