using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            inputBox.GotFocus += OnFocus;
            inputBox.LostFocus += OnLostFocus;
        }

        void TextEntered( object sender, KeyEventArgs e )
        {
            if( e.Key == Key.Enter ) {
                manager.ParseInput(inputBox.Text);
                inputBox.Clear();
            }
        }
        void OnFocus( object sender, RoutedEventArgs e )
        {
            WaywardManager.instance.inputManager.inputBusy = true;
        }
        void OnLostFocus( object sender, RoutedEventArgs e )
        {
            WaywardManager.instance.inputManager.inputBusy = false;
        }

        public override bool CloseAction()
        {
            WaywardManager.instance.window.mainCanvas.Children.Remove(element);
            WaywardManager.instance.pages.Remove(this);
            WaywardManager.instance.inputPage = null;
            WaywardManager.instance.inputManager.inputBusy = false;

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
