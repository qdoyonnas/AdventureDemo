using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WaywardEngine
{
    public class InputPage : Page
    {
        TextBox inputBox;

        public InputPage()
            : base("InputPage")
        {
            ContextMenuHelper.AddContextMenuItem(element, "Close", CloseAction);

            inputBox = Utilities.FindNode<TextBox>(element, "InputBox");
            inputBox.KeyDown += TextEntered;
        }

        void TextEntered( object sender, KeyEventArgs e )
        {
            if( e.Key == Key.Enter ) {
                WaywardManager.instance.DisplayMessage(inputBox.Text);
                inputBox.Clear();
            }
        }

        public void Focus()
        {
            inputBox.Focus();
        }
    }
}
