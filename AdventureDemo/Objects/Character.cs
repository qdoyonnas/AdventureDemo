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
    class Character : Physical
    {
        public Character( Dictionary<string, object> data )
            : base(data)
        {
            Construct();
        }
        public Character( string name, AttachmentPoint container ) 
            : base(name, container)
        {
            Construct();
        }
        public Character( string name, AttachmentPoint container, double totalVolume )
            : base(name, container, totalVolume)
        {
            Construct();
        }
        public Character( string name, AttachmentPoint container, double totalVolume, double weight )
            : base(name, container, totalVolume, weight)
        {
            Construct();
        }
        private void Construct()
        {
            this.description = "an entity";

            Dictionary<string, object> attachmentData = new Dictionary<string, object>() {
                { "name", "Pocket" }, { "capacity", 0.5 }, { "quantity", -1 },
                { "types", new AttachmentType[] { AttachmentType.ALL } }
            };
            this.AddAttachmentPoint( attachmentData );
            this.AddAttachmentPoint( attachmentData );
            
            attachmentData["name"] = "Shoulder";
            attachmentData["quantity"] = 1;
            attachmentData["capacity"] = -1.0;
            attachmentData["types"] = new AttachmentType[] { AttachmentType.HANG };
            this.AddAttachmentPoint( attachmentData );
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