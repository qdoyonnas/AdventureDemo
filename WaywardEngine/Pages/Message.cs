using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WaywardEngine
{
    public class Message : Page
    {
        TextBlock messageText;
        TextBlock subtext;

        bool doCloseAction = false;

        public Message( string message )
            : base("Message")
        {
            Construct(message, "Click to close");
        }
        public Message( string message, string subtext )
            : base("Message")
        {
            Construct(message, subtext);
        }
        private void Construct( string message, string subtext )
        {
            messageText = Utilities.FindNode<TextBlock>( element, "MessageText" );
            this.subtext = Utilities.FindNode<TextBlock>( element, "Subtext" );

            messageText.Text = message;
            this.subtext.Text = subtext;
        }

        protected override void OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            base.OnMouseDown(sender, e);
            //initialPosition = new Point( Canvas.GetLeft(element), Canvas.GetTop(element) );
            doCloseAction = true;
        }
        protected override void OnMouseMove( object sender, MouseEventArgs e )
        {
            base.OnMouseMove(sender, e);
            doCloseAction = false;
        }
        protected override void OnMouseUp( object sender, MouseButtonEventArgs e )
        {
            base.OnMouseUp(sender, e);

            Point newPosition = new Point( Canvas.GetLeft(element), Canvas.GetTop(element) );
            //if( Utilities.Distance(newPosition, initialPosition) < 1 ) {
            if( doCloseAction ) {
                CloseAction(sender, e);
            }
        }
    }
}
