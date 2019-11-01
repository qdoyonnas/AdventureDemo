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
        public Character( Dictionary<string, object> data )
            : base(data)
        {
            Construct();
        }
        public Character( string name, AttachmentPoint container, double innerVolume ) 
            : base(name, container, innerVolume)
        {
            Construct();
        }
        public Character( string name, AttachmentPoint container, double innerVolume, double totalVolume )
            : base(name, container, innerVolume, totalVolume)
        {
            Construct();
        }
        public Character( string name, AttachmentPoint container, double innerVolume, double totalVolume, double weight )
            : base(name, container, innerVolume, totalVolume, weight)
        {
            Construct();
        }
        private void Construct()
        {
            this.description = "an entity";
        }

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