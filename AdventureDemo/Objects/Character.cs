using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using WaywardEngine;

namespace AdventureDemo
{
    class Character : Container
    {
        public Character( string name, double innerVolume ) 
            : base(name, innerVolume)
        { }
        public Character( string name, double innerVolume, double totalVolume )
            : base(name, innerVolume, totalVolume)
        { }
        public Character( string name, double innerVolume, double totalVolume, double weight )
            : base(name, innerVolume, totalVolume, weight)
        { }

        public override GameObjectData GetDescription( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            GameObjectData nameData = GetData("name");
            data.text = $"This is {nameData.text}";

            data.SetSpan(
                new Run("This is "),
                nameData.span,
                new Run(".")
            );

            return data;
        }
    }
}