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
    class WorldsMenuPage : ContentPage
    {
        StackPanel worldsPanel;

        public WorldsMenuPage()
        {
            FrameworkElement content = GameManager.instance.GetResource<FrameworkElement>("WorldsMenu");
            AddContent(content);
            SetTitle("");
            element.ContextMenu = null;

            worldsPanel = Utilities.FindNode<StackPanel>(element, "Worlds");
            DisplayContent();

            ContextMenuHelper.AddContextMenuItem(element, "Back", NavigateBack);
        }

        void DisplayContent()
        {
            WorldData[] worlds = DataManager.instance.GetWorldDatas();

            foreach( WorldData world in worlds ) {
                TextBlock entry = GameManager.instance.GetResource<TextBlock>("WorldEntry");
                Span name = Utilities.FindNode<Span>(entry, "WorldName");
                name.Inlines.Add(world.name);
                name.MouseUp += ( sender, e ) => {
                    GameManager.instance.CreateWorld(world);
                    OnCreateWorld(world);
                };

                worldsPanel.Children.Add(entry);
            }
        }

        void OnCreateWorld( WorldData world )
        {
            WaywardManager.instance.AddPage(new ScenariosMenuPage(world), WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));

            CloseAction();
        }

        public bool NavigateBack()
        {
            CloseAction();

            WaywardManager.instance.AddPage( 
                new MainMenuPage(),
                WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.3)
            );

            return true;
        }
    }
}
