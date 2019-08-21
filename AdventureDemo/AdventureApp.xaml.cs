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
    public partial class AdventureApp : Application
    {
        private void Application_Startup( object sender, StartupEventArgs e )
        {
            // Init the WaywardEngine
            WaywardManager.instance.Init(this, Resources);
            MainWindow window = WaywardManager.instance.window;
            
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

            GameManager.instance.Init(this);

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

        private void CreateOverviewPage( object sender, RoutedEventArgs e )
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            
            OverviewPage page = new OverviewPage();
            
            WaywardManager.instance.AddPage( page, mousePosition );

            for( int i = 0; i < GameManager.instance.RootCount(); i++ ) {
                string key = "primary" + i;
                page.AddOverview(key);
                page.DisplayObject( key, GameManager.instance.GetRoot(i) );
            }

            page.AddEventPanel("main");
            page.DisplayEvent("main", "You wake up.");
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