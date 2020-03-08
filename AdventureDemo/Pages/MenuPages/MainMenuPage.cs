using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using WaywardEngine;

namespace AdventureDemo
{
    class MainMenuPage : WaywardEngine.ContentPage
    {
        public MainMenuPage()
        {
            SetTitle("Wayward Engine");
            element.ContextMenu = null;

            FrameworkElement contents = GameManager.instance.GetResource<FrameworkElement>("MainMenu");
            AddContent(contents);

            Span choice = Utilities.FindNode<Span>(element, "WorldsChoice");
            choice.MouseUp += OnWorldChoice;
            choice = Utilities.FindNode<Span>(element, "OptionsChoice");
            choice.MouseUp += OnOptionsChoice;
            choice = Utilities.FindNode<Span>(element, "ExitChoice");
            choice.MouseUp += OnExitChoice;
        }

        void OnWorldChoice( object sender, MouseButtonEventArgs e )
        {
            WaywardManager.instance.AddPage(new WorldsMenuPage(), WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));

            CloseAction();
        }
        void OnOptionsChoice( object sender, MouseButtonEventArgs e )
        {
        }
        void OnExitChoice( object sender, MouseButtonEventArgs e )
        {
            Application.Current.Shutdown();
        }
    }
}
