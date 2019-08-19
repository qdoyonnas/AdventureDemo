using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text;
using Newtonsoft.Json;

using WaywardEngine;

namespace AdventureDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class AdventureApp : Application
    {
        PlaceholderObject room1;
        PlaceholderObject room2;

        private void Application_Startup( object sender, StartupEventArgs e )
        {
            WaywardManager.instance.Init(this);
            MainWindow window = WaywardManager.instance.window;

            // Fetch WaywardEngine Resources
            Resources.MergedDictionaries.Add( new ResourceDictionary {
                Source = new Uri("/WaywardEngine;component/ResourceDictionaries/Pages.xaml", UriKind.RelativeOrAbsolute)
            } );

            #region Load Settings
            Settings settings = new Settings();

            string path = "settings.config";
            string longPath = $@"{Directory.GetCurrentDirectory()}\{path}";
            if( File.Exists(longPath) ) {
                StreamReader reader = new StreamReader(path);

                string sSettings = reader.ReadToEnd();
                settings = JsonConvert.DeserializeObject<Settings>( sSettings );
            } else {
                StreamWriter writer = new StreamWriter(path);
                string sSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);
                writer.Write(sSettings);
                writer.Close();
            }

            switch( settings.displayMode ) {
                case "borderless":
                    window.ResizeMode = ResizeMode.NoResize;
                    window.WindowStyle = WindowStyle.None;
                    window.WindowState = WindowState.Maximized;
                    break;
                case "fullscreen":
                    
                    break;
                default:
                    window.ResizeMode = ResizeMode.CanResizeWithGrip;
                    window.WindowStyle = WindowStyle.ThreeDBorderWindow;
                    window.Height = settings.height;
                    window.Width = settings.width;
                    break;
            }
            #endregion

            // Game Setup
            window.Title = "AdventureDemo";
            window.mainCanvas.Style = Resources["WindowBackground"] as Style;

            SetupContextMenu();

            SetupGame();

            window.Show();
        }
        private void SetupContextMenu()
        {
            ContextMenu menu = WaywardManager.instance.window.ContextMenu;

            MenuItem openItem = new MenuItem();
            openItem.Header = "Open";
            menu.Items.Insert(0, openItem );

            MenuItem newItem = new MenuItem();
            newItem.Header = "Overview Page";
            newItem.Click += CreateOverviewPage;
            openItem.Items.Insert(0, newItem );
        }
        
        private void SetupGame()
        {
            room1 = new PlaceholderObject("Dim Room", string.Empty, string.Empty);
            room1.contents.Add( new PlaceholderObject("You", "--", "??") );
            room1.contents.Add( new PlaceholderObject("Door", string.Empty, string.Empty) );

            PlaceholderObject table = new PlaceholderObject("Table", string.Empty, string.Empty);
            room1.contents.Add(table);

            table.contents.Add( new PlaceholderObject("Candle", string.Empty, string.Empty) );
            table.contents.Add( new PlaceholderObject("Mouse", "Cheese", "Eating Cheese") );
            table.contents.Add( new PlaceholderObject("Box", string.Empty, string.Empty) );

            PlaceholderObject bust = new PlaceholderObject("Bust", string.Empty, string.Empty);
            room1.contents.Add( bust );
            bust.contents.Add( new PlaceholderObject("Raven", "Scroll", "??") );

            room1.contents.Add( new PlaceholderObject("Torch", string.Empty, string.Empty) );

            room2 = new PlaceholderObject("Round Room", string.Empty, string.Empty);
            room2.contents.Add( new PlaceholderObject("Door", string.Empty, string.Empty) );
            room2.contents.Add( new PlaceholderObject("Chest", string.Empty, string.Empty) );
        }

        private void CreateOverviewPage( object sender, RoutedEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            FrameworkElement element = Resources["OverviewPage"] as FrameworkElement;
            OverviewPage page = new OverviewPage(element);
            WaywardManager.instance.AddPage( page, mousePosition );
            
            page.DisplayObject(room1);
            page.DisplayObject(room2);
            page.AddEvent("You wake up.");
            page.AddEvent("The mouse eats the Cheese");

            StackPanel events = LogicalTreeHelper.FindLogicalNode( element, "Events") as StackPanel;
        }

        private void Application_DispatcherUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e )
        {
            MessageBox.Show("An unhandled exception has occurred: " + e.Exception.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            this.Shutdown();
        }
    }

    public class Settings
    {
        public string displayMode = "windowed";
        public int width = 1024;
        public int height = 768;
    }
}
