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

        new public static Dictionary<string, object> ParseData( ObjectData objectData )
        {
            Dictionary<string, object> data = GameObject.ParseData(objectData);

            try {
                data["weight"] = double.Parse(objectData.data["weight"]);
            } catch( KeyNotFoundException e ) {
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Loading material data '{objectData.id}' at 'weight' field: {e}");
            }

            try {
                data["color"] = objectData.data["color"];
            } catch( KeyNotFoundException e ) {
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Loading material data '{objectData.id}' at 'color' field: {e}");
            }

            return data;
        }

        public Material( Dictionary<string, object> data )
            : base( data )
        {
            weightPerLiter = data.ContainsKey("weight") ? (double)data["weight"] : 0.0;
            color = data.ContainsKey("color") ? (string)data["color"] : "#ffffff";
        }
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
