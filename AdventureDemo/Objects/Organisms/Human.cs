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

            AddPart( new BodyPart(this, "head", 2, Utilities.Pair<Material, double>(flesh, 1)) );
            AddPart( new BodyPart(this, "torso", 40, Utilities.Pair<Material, double>(flesh, 1)) );
            PhysicalAmalgam leftArm = new PhysicalAmalgam("left arm");
            AddPart( leftArm );
            leftArm.AddPart( new BodyPart(this, "arm", 4, Utilities.Pair<Material, double>(flesh, 1)) );
            leftArm.AddPart(  new ManipulatorBodyPart(this, "left hand", 0.5, Utilities.Pair<Material, double>(flesh, 1)) );

            PhysicalAmalgam rightArm = new PhysicalAmalgam("right arm");
            AddPart( rightArm );
            rightArm.AddPart( new BodyPart(this, "arm", 4, Utilities.Pair<Material, double>(flesh, 1)) );
            rightArm.AddPart(  new ManipulatorBodyPart(this, "right hand", 0.5, Utilities.Pair<Material, double>(flesh, 1)) );

            PhysicalAmalgam leftLeg = new PhysicalAmalgam("left leg");

            /*
            AddBodyPart( "torso", new BodyPart(this, "left arm", 4, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();
            AddBodyPart( "torso", new BodyPart(this, "right arm", 4, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();
            AddBodyPart( "torso", new BodyPart(this, "left leg", 6, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();
            AddBodyPart( "torso", new BodyPart(this, "right leg", 6, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();

            AddBodyPart("torso/left leg", new LocomotionBodyPart(this, "left foot", 0.5, Utilities.Pair<Material, double>(flesh, 1)) );
            AddBodyPart("torso/right leg", new LocomotionBodyPart(this, "right foot", 0.5, Utilities.Pair<Material, double>(flesh, 1)) );
            */
        }
    }
}
