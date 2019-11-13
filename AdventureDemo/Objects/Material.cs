using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace AdventureDemo
{
    class Material : GameObject
    {
        double weightPerLiter;

        public string color;

        public Material( string name, double weight, string color = "#ffffff" )
            : base( name )
        {
            weightPerLiter = weight;
            this.color = color;
        }

        public double GetWeight( double volume = 1 )
        {
            return volume * weightPerLiter;
        }

        public override GameObjectData GetName( string[] parameters )
        {
            GameObjectData data = base.GetName(parameters);

            Style linkStyle = GameManager.instance.GetResource<Style>("Link");
            Setter setter = (Setter)linkStyle.Setters[0];
            setter.Value = WaywardEngine.WaywardTextParser.ColorFromString(color);
            data.span.Style = linkStyle;

            return data;
        }
    }
}
