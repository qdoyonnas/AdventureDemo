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
    class Material : GameObject, IEquatable<Material>
    {
        double weightPerLiter;

        string color;

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

        public string GetColor()
        {
            return color;
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

        public bool Equals(Material other)
        {
            if( Object.ReferenceEquals(other, null) ) {
                return false;
            }
            if( Object.ReferenceEquals(this, other) ) {
                return true;
            }
            if( this.GetType() != other.GetType() ) {
                return false;
            }

            if( GetWeight() != other.GetWeight() ) {
                return false;
            }
            if( GetName().text != other.GetName().text ) {
                return false;
            }
            if( GetDescription().text != other.GetDescription().text ) {
                return false;
            }

            return true;
        }

        public static bool operator ==(Material lhs, Material rhs)
        {
            if( Object.ReferenceEquals(lhs, null) ) {
                if( Object.ReferenceEquals(rhs, null) ) {
                    return true;
                }
                return false;
            }

            return lhs.Equals(rhs);
        }
        public static bool operator !=(Material lhs, Material rhs)
        {
            return !(lhs == rhs);
        }
    }
}
