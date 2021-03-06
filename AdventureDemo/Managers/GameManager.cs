﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using WaywardEngine;

namespace AdventureCore
{
    class GameManager
    {
        #region Singleton
        
        public static GameManager _instance;
        public static GameManager instance {
            get {
                if( _instance == null ) {
                    _instance = new GameManager();
                }

                return _instance;
            }
        }
        private GameManager() {}

        #endregion

        #region Fields

        public AdventureApp application;

        // Prevents most functionality until after init
        public bool isInitialized = false;

        public WorldManager world;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Init the GameManager and start the game.
        /// </summary>
        /// <param name="app"></param>
        public void Init(AdventureApp app)
        {
            isInitialized = true;

            application = app;

            StartMenu();
        }

        /// <summary>
        /// Retrieve resource from application resources."
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetResource<T>( string key )
            where T : class
        {
            if( !isInitialized ) { return null; }

            T resource = application.Resources[key] as T;
            if( resource == null ) {
                throw new System.NullReferenceException($"GameManager could not find '{key}' resource");
            }
            return resource;
        }

        #endregion

        #region Game Methods

        public void StartMenu()
        {
            if( !isInitialized ) { return; }

            WaywardManager.instance.ClearPages();

            MainMenuPage page = new MainMenuPage();

            WaywardManager.instance.AddPage(page, WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.3));

            SetupMenuContextMenu();
        }

        private void SetupMenuContextMenu()
        {
            ContextMenuHelper.ClearContextMenu(WaywardManager.instance.window);

            ContextMenuHelper.AddContextMenuItem(WaywardManager.instance.window, "Exit", Exit);
        }

        public void CreateWorld( WorldData data )
        {
            if( !isInitialized ) { return; }

            world = new WorldManager(data, -1);
            world.GenerateWorld();
        }
        public void StartScenario( ScenarioData data )
        {
            if( !isInitialized ) { return; }

            WaywardManager.instance.ClearPages();

            world.LoadScenario(data);

            SetupPlayContextMenu();

            Point position = new Point(WaywardManager.instance.window.ActualWidth * 0.75, WaywardManager.instance.window.ActualHeight/3.1);
            DisplayOverviewPage(position);

            position = new Point(WaywardManager.instance.window.ActualWidth * 0.75, WaywardManager.instance.window.ActualHeight/1.4);
            DisplayTimelinePage(position);

            position = new Point(WaywardManager.instance.window.ActualWidth * 0.25, WaywardManager.instance.window.ActualHeight/2.2);
            DisplayPlayerVerbose(position);

            WaywardManager.instance.SelectInputPage();
        }
        public void LoadData( SaveData data )
        {
            // XXX: Add save loading here
        }

        private void SetupPlayContextMenu()
        {
            ContextMenuHelper.ClearContextMenu(WaywardManager.instance.window);
            ContextMenuHelper.AddContextMenuItem(WaywardManager.instance.window, "Exit", ExitGame);

            ContextMenuHelper.AddContextMenuHeader(WaywardManager.instance.window, "Page", new Dictionary<string, ContextMenuAction>() {
                { "Overview", CreateOverviewPage },
                { "Visual", CreateVerbosePage },
                { "Timeline", CreateTimelinePage },
                { "Input", WaywardManager.instance.SelectInputPage }
            });

            WaywardManager.instance.window.KeyDown += ( sender, e ) =>
            {
                if( e.Key != Key.Enter ||
                InputManager.instance.inputBusy ) { return; }

                WaywardManager.instance.SelectInputPage();
            };

        }

        public void Update(double time)
        {
            TimelineManager.instance.AdvanceTimeline(time);
        }

        #endregion

        #region Display Methods

        public void DisplayPlayerVerbose( Point position )
        {
            VerbosePage page = DisplayVerbosePage( position );
        }
        public void DisplayRoots( Point position )
        {
            throw new System.NotImplementedException("GameManager.DisplayRoots is not implemented. Observer needs refactor");
        }

        public bool CreateOverviewPage()
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            DisplayOverviewPage(mousePosition);

            return false;
        }
        public bool CreateVerbosePage()
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            DisplayPlayerVerbose(mousePosition);

            return false;
        }
        public bool CreateTimelinePage()
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            DisplayTimelinePage(mousePosition);

            return false;
        }
        public bool Exit()
        {
            Application.Current.Shutdown();
            return true;
        }
        public bool ExitGame()
        {
            WaywardManager.instance.ClearPages();
            WaywardManager.instance.AddPage( new ScenariosMenuPage(world.data), WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5) );

            world.Unload();

            SetupMenuContextMenu();

            return true;
        }

        public OverviewPage DisplayOverviewPage( Point position )
        {
            OverviewPage page = new OverviewPage( world.player );
            WaywardManager.instance.AddPage( page, position );

            return page;
        }
        public VerbosePage DisplayVerbosePage( Point position )
        {
            VerbosePage page = new VerbosePage( world.player );
            WaywardManager.instance.AddPage(page, position);

            return page;
        }
        public DescriptivePage DisplayDescriptivePage( GameObject target )
        {
            //Point mousePosition = WaywardManager.instance.GetMousePosition();

            return DisplayDescriptivePage(WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5), target);
        }
        public DescriptivePage DisplayDescriptivePage( Point position, GameObject target )
        {
            DescriptivePageSection[] sections = target.DisplayDescriptivePage().ToArray();
            DescriptivePage page = new DescriptivePage( world.player, target, sections );

            WaywardManager.instance.AddPage(page, position);

            return page;
        }
        public TimelinePage DisplayTimelinePage( Point position )
        {
            TimelinePage page = new TimelinePage( world.player );
            WaywardManager.instance.AddPage(page, position);

            return page;
        }

        #endregion
    }
}
