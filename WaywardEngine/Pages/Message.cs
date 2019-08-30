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

        Point initialPosition = new Point();

        public Message( string message )
            : base("Message")
        {
            messageText = Utilities.FindNode<TextBlock>( element, "MessageText" );
            subtext = Utilities.FindNode<TextBlock>( element, "Subtext" );

            messageText.Text = message;
        }

        protected override void OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            base.OnMouseDown(sender, e);
            initialPosition = new Point( Canvas.GetLeft(element), Canvas.GetTop(element) );
        }
        protected override void OnMouseUp( object sender, MouseButtonEventArgs e )
        {
            Point newPosition = new Point( Canvas.GetLeft(element), Canvas.GetTop(element) );
            if( Utilities.Distance(newPosition, initialPosition) > 1 ) {
                CloseAction(sender, e);
            }
            base.OnMouseUp(sender, e);
        }
    }
}
