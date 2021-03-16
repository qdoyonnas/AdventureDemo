using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text;
using Newtonsoft.Json;

using WaywardEngine;

namespace AdventureCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class AdventureApp : Application
    {
        private void Application_Startup( object sender, StartupEventArgs e )
        {
            // Init the WaywardEngine
            WaywardManager.instance.Init(this, InputManager.instance, Resources);

            DataManager.instance.Init(this);

            MainWindow window = WaywardManager.instance.window;

            #region Load Settings
            Settings settings = DataManager.instance.GetSettings();
            switch( settings.displayMode ) {
                case "borderless":
                    window.ResizeMode = ResizeMode.NoResize;
                    window.WindowStyle = WindowStyle.None;
                    window.WindowState = WindowState.Maximized;
                    break;
                case "fullscreen":
                    window.ResizeMode = ResizeMode.NoResize;
                    window.WindowStyle = WindowStyle.None;
                    window.WindowState = WindowState.Maximized;
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
            window.Title = "Adventure Core";
            window.mainCanvas.Style = Resources["WindowBackground"] as Style;

            window.Show();

            GameManager.instance.Init(this);
        }

        private void Application_DispatcherUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e )
        {
            MessageBox.Show("An unhandled exception has occurred: " + e.Exception.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            this.Shutdown();
        }
    }
}
