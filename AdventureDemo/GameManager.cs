﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
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
        #endregion

        public AdventureApp application;

        // Prevents most functionality until after init
        public bool isInitialized = false;

        public WorldManager world;

        private GameManager()
        {
            WaywardManager.instance.OnUpdate += Update;
        }

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
        public void StartMenu()
        {
            if( !isInitialized ) { return; }

            MainMenuPage page = new MainMenuPage();
            Point position = new Point( 200, 300);

            WaywardManager.instance.AddPage(page, position);
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

        public void StartScenario( SaveData data )
        {
            // XXX: Add save loading here
        }
        public void StartScenario( ScenarioData data )
        {
            WaywardManager.instance.ClearPages();

            SetupPlayContextMenu();

            world = new WorldManager(data);
        }
        private void SetupPlayContextMenu()
        {
            ContextMenuHelper.ClearContextMenu(WaywardManager.instance.window);

            ContextMenuHelper.AddContextMenuHeader(WaywardManager.instance.window, "Page", new Dictionary<string, ContextMenuAction>() {
                { "Overview", CreateOverviewPage },
                { "Visual", CreateVerbosePage }
            });
        }

        public void Update()
        {

        }
        
        public void DisplayPlayerVerbose( Point position )
        {
            VerbosePage page = DisplayVerbosePage( position, world.player.GetControlled() );
        }

        public void DisplayRoots( Point position )
        {
            throw new System.NotImplementedException("GameManager.DisplayRoots is not implemented. Observer needs refactor");
        }

        private bool CreateOverviewPage()
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            DisplayOverviewPage(mousePosition, world.player);

            return false;
        }
        private bool CreateVerbosePage()
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();
            DisplayPlayerVerbose(mousePosition);

            return false;
        }

        public OverviewPage DisplayOverviewPage( Point position, Actor actor )
        {
            OverviewPage page = new OverviewPage(actor);
            WaywardManager.instance.AddPage( page, position );

            return page;
        }
        public VerbosePage DisplayVerbosePage( Point position, GameObject subject )
        {
            VerbosePage page = new VerbosePage( world.player, subject );
            WaywardManager.instance.AddPage(page, position);

            return page;
        }
        public DescriptivePage DisplayDescriptivePage( GameObject target )
        {
            DescriptivePageSection[] sections = target.DisplayDescriptivePage().ToArray();
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            DescriptivePage page = new DescriptivePage( world.player, target, sections );
            WaywardManager.instance.AddPage(page, mousePosition);

            return page;
        }
    }
}
