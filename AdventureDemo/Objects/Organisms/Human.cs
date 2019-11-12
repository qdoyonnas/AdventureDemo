using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaywardEngine;

namespace AdventureDemo
{
    class Human : Organism
    {
        public Human( Dictionary<string, object> data )
            : base(data)
        {
            Construct();
        }
        public Human( string name )
            : base(name)
        {
            Construct();
        }

        void Construct()
        {
            Material flesh = GameManager.instance.world.GetMaterial("flesh");
            if( flesh == null ) { return; }

            AddBodyPart( string.Empty, new BodyPart(this, "head", 2, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();
                
            AddBodyPart( "head", new BodyPart(this, "torso", 40, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint()
                .AddBodyAttachmentPoint()
                .AddBodyAttachmentPoint()
                .AddBodyAttachmentPoint();

            AddBodyPart( "torso", new BodyPart(this, "left arm", 4, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();
            AddBodyPart( "torso", new BodyPart(this, "right arm", 4, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();
            AddBodyPart( "torso", new BodyPart(this, "left leg", 6, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();
            AddBodyPart( "torso", new BodyPart(this, "right leg", 6, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();

            AddBodyPart("torso/left arm", new ManipulatorBodyPart(this, "left hand", 0.5, Utilities.Pair<Material, double>(flesh, 1)) );
            AddBodyPart("torso/right arm", new ManipulatorBodyPart(this, "right hand", 0.5, Utilities.Pair<Material, double>(flesh, 1)) );

            AddBodyPart("torso/left leg", new LocomotionBodyPart(this, "left foot", 0.5, Utilities.Pair<Material, double>(flesh, 1)) );
            AddBodyPart("torso/right leg", new LocomotionBodyPart(this, "right foot", 0.5, Utilities.Pair<Material, double>(flesh, 1)) );
        }
    }
}
