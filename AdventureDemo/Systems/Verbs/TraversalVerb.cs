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

            physicalSelf = self as Physical;
        }

        public override bool Action( GameObject target )
        {
            return false;
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
            if( physicalSelf.container == container.GetContents() ) { return CheckResult.INVALID; }
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
    }
}
