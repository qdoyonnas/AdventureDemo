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

            window.Show();

            //WaywardManager.instance.StartTutorial();
            GameManager.instance.Init(this);
            GameManager.instance.DisplayOverviewPage( new Point(800, 150), GameManager.instance.player );
            GameManager.instance.DisplayPlayerVerbose( new Point(150, 150) );
        }
        private void SetupContextMenu()
        {
            ContextMenuHelper.AddContextMenuHeader(WaywardManager.instance.window, "Page", new Dictionary<string, ContextMenuAction>() {
                { "Overview", CreateOverviewPage },
                { "Visual", CreateVerbosePage }
            });
        }

        private bool CreateOverviewPage()
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            GameManager.instance.DisplayOverviewPage(mousePosition, GameManager.instance.player);

            return false;
        }
        private bool CreateVerbosePage()
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            GameManager.instance.DisplayPlayerVerbose(mousePosition);

            return false;
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
