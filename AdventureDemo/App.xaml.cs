﻿using System;
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
    public partial class App : Application
    {
        MainWindow window;

        private void Application_Startup( object sender, StartupEventArgs e )
        {
            window = new MainWindow();
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
            window.Style = Resources["WindowBackground"] as Style;
            MenuItem newPageMenuItem = new MenuItem();
            newPageMenuItem.Header = "New Page";
            newPageMenuItem.Click += CreateNewPage;
            window.ContextMenu.Items.Insert(0, newPageMenuItem );

            window.Show();
        }

        private void CreateNewPage( object sender, RoutedEventArgs e )
        {
            Point mousePosition = Mouse.GetPosition(window.mainCanvas);

            FrameworkElement newPage = Resources["BlankPage"] as FrameworkElement;
            newPage.Style = Resources["PageStyle"] as Style;
            window.mainCanvas.Children.Add(newPage);

            Canvas.SetLeft(newPage, mousePosition.X);
            Canvas.SetTop(newPage, mousePosition.Y);
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
