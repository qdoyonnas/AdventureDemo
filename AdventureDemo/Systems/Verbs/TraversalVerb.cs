using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class TraversalVerb : Verb
    {
        Physical physicalSelf;

        public TraversalVerb( GameObject self )
            : base(self)
        {
            _displayLabel = "Walk";

            validInputs = new string[] {
                "walk", "move", "go"
            };

            physicalSelf = self as Physical;
        }

        public override bool Action( GameObject target )
        {
            if( Check(target) != CheckResult.VALID ) { return false; }

            Container container = target as Container;
            if( container != null ) {
                return EnterContainer(container);
            }

            Connection connection = target as Connection;
            if( connection != null ) {
                return EnterConnection(connection);
            }

            return false;
        }
        bool EnterContainer( Container container )
        {
            Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);
            return container.GetContents().Attach(parent);
        }
        bool EnterConnection( Connection connection )
        {
            Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);
            return connection.secondContainer.Attach(parent);
        }

        public override CheckResult Check( GameObject target )
        {
            Container container = target as Container;
            if( container == null ) { 
                Connection connection = target as Connection;
                if( connection == null ) { return CheckResult.INVALID; }
                
                return CheckConnection(connection);
            }

            return CheckContainer(container);
        }
        CheckResult CheckConnection( Connection connection )
        {
            if( physicalSelf.GetVolume() > connection.throughput ) { return CheckResult.INVALID; }

            return connection.secondContainer.CanAttach(self);
        }
        CheckResult CheckContainer( Container container )
        {
            if( container.Contains(physicalSelf) ) { return CheckResult.INVALID; }
            return container.CanContain(physicalSelf);
        }

        public override void Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            CheckResult check = Check(target);
            if( check >= CheckResult.RESTRICTED ) {
                Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();
                if( check == CheckResult.RESTRICTED ) {
                    ContextMenuHelper.AddContextMenuItem( span, WaywardTextParser.ParseAsBlock($@"<gray>{displayLabel}</gray>") , null, false );
                } else {
                    ContextMenuHelper.AddContextMenuItem( span, WaywardTextParser.ParseAsBlock(displayLabel) , delegate { return Action(target); } );
                }
            }
        }

        public override bool ParseInput( InputEventArgs e )
        {
            if( e.parsed ) { return true; }
            if( e.words.Length <= 1 ) { return false; }

            WaywardManager.instance.DisplayMessage($"Move to {e.words[1]}");

            return false;
        }
    }
}
