using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;

namespace AdventureCore
{
    class InputManager : InputManagerBase
    {
        #region Singleton

        private static InputManager _instance;
        public static InputManager instance {
            get {
                if( _instance == null ) {
                    _instance = new InputManager();
                }

                return _instance;
            }
        }

        #endregion

        private Dictionary<string[], InputDelegate> commands;

        private InputManager()
        {
            commands = new Dictionary<string[], InputDelegate>();
            commands.Add(new string[] { "message", "msg" }, DisplayCommand);
            commands.Add(new string[] { "close", "c" }, ClosePage);
            commands.Add(new string[] { "exit", "quit" }, ExitGame);
            commands.Add(new string[] { "open", "o" }, OpenPage);

            inputReceived += ManagerCommands;
        }

        private bool ManagerCommands( InputEventArgs e )
        {
            return CheckCommands(commands, e);
        }

        private bool DisplayCommand( InputEventArgs e )
        {
            int i = e.input.IndexOf(' ');
            WaywardManager.instance.DisplayMessage(e.input.Substring(i + 1));

            return true;
        }
        private bool ClosePage( InputEventArgs e )
        {
            if( e.parsed ) { return true; }

            if( e.words.Length <= 1 ) {
                WaywardManager.instance.CloseTopPage();
                return true;
            } else {
                switch( e.words[1].ToLower() ) {
                    case "all":
                        WaywardManager.instance.ClearPages();
                        break;
                    case "m": // XXX: This wont be necessary when autocomplete is added
                    case "msg":
                    case "msgs":
                    case "messages":
                        WaywardManager.instance.ClearPages(typeof(Message));
                        break;
                    case "o":
                    case "ovr":
                    case "overview":
                        WaywardManager.instance.ClearPages(typeof(OverviewPage));
                        break;
                    case "d":
                    case "dscr":
                    case "descriptive":
                        WaywardManager.instance.ClearPages(typeof(DescriptivePage));
                        break;
                    case "v":
                    case "vrb":
                    case "verbose":
                        WaywardManager.instance.ClearPages(typeof(VerbosePage));
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }
        private bool OpenPage( InputEventArgs e )
        {
            if( e.parsed ) { return true; }
            if( e.words.Length <= 1 ) { return false; }

            Point position = new Point(0, WaywardManager.instance.window.ActualHeight / 2);
            switch( e.words[1].ToLower() ) {
                case "o": // XXX: This wont be necessary when autocomplete is added
                case "ovr":
                case "overview":
                    position.X = WaywardManager.instance.window.ActualWidth * 0.75;
                    GameManager.instance.DisplayOverviewPage(position, GameManager.instance.world.player);
                    break;
                case "v":
                case "vrb":
                case "verbose":
                    position.X = WaywardManager.instance.window.ActualWidth * 0.25;
                    GameManager.instance.DisplayPlayerVerbose(position);
                    break;
                case "t":
                case "tml":
                case "timeline":
                    position.X = WaywardManager.instance.window.ActualWidth * 0.75;
                    GameManager.instance.DisplayTimelinePage(position, GameManager.instance.world.player);
                    break;
                default:
                    return false;
            }

            return true;
        }
        private bool ExitGame( InputEventArgs e )
        {
            if( e.parsed ) { return true; }

            GameManager.instance.Exit();
            return true;
        }
    }
}
