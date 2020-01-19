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
            SetTitle("Path to the Stars");
            element.ContextMenu = null;

            FrameworkElement contents = GameManager.instance.GetResource<FrameworkElement>("MainMenu");
            AddContent(contents);

            Span choice = Utilities.FindNode<Span>(element, "ScenariosChoice");
            choice.MouseUp += OnScenarioChoice;
            choice = Utilities.FindNode<Span>(element, "OptionsChoice");
            choice.MouseUp += OnOptionsChoice;
            choice = Utilities.FindNode<Span>(element, "ExitChoice");
            choice.MouseUp += OnExitChoice;
        }

        void OnScenarioChoice( object sender, MouseButtonEventArgs e )
        {
            Point position = new Point(WaywardManager.instance.window.ActualWidth / 2, WaywardManager.instance.window.ActualHeight / 2);
            WaywardManager.instance.AddPage(new ScenariosMenuPage(), position);

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
