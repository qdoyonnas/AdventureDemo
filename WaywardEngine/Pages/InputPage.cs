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
        public InputManagerBase manager;
        TextBox inputBox;

        public InputPage( InputManagerBase manager )
            : base("InputPage")
        {
            this.manager = manager;
            ContextMenuHelper.AddContextMenuItem(element, "Close", CloseAction);

            inputBox = Utilities.FindNode<TextBox>(element, "InputBox");
            inputBox.KeyDown += TextEntered;
        }

        void TextEntered( object sender, KeyEventArgs e )
        {
            if( e.Key == Key.Enter ) {
                manager.ParseInput(inputBox.Text);
                inputBox.Clear();
            }
        }

        public override bool CloseAction()
        {
            WaywardManager.instance.window.mainCanvas.Children.Remove(element);
            WaywardManager.instance.pages.Remove(this);
            WaywardManager.instance.inputPage = null;

            if( WaywardManager.instance.pages.Count > 0 ) {
                WaywardManager.instance.pages.Last().SetBorderColor("HighlightedBorderBrush");
            }

            return false;
        }

        public void Focus()
        {
            inputBox.Focus();
        }
    }
}
