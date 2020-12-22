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

namespace AdventureCore
{
    class ScenariosMenuPage : ContentPage
    {
        StackPanel scenariosPanel;
        WorldData worldData;

        public ScenariosMenuPage( WorldData world )
        {
            worldData = world;

            FrameworkElement content = GameManager.instance.GetResource<FrameworkElement>("ScenariosMenu");
            AddContent(content);
            SetTitle("Scenarios");
            element.ContextMenu = null;

            scenariosPanel = Utilities.FindNode<StackPanel>(element, "Scenarios");
            DisplayContent();

            ContextMenuHelper.AddContextMenuItem(element, "Back", NavigateBack);
        }

        public override void Clear()
        {
            scenariosPanel.Children.Clear();
        }

        void DisplayContent()
        {
            ScenarioData[] scenarios = DataManager.instance.GetScenarioDatas(worldData);

            foreach( ScenarioData data in scenarios ) {
                AddScenario(data);
            }
        }
        void AddScenario( ScenarioData data )
        {
            Grid entry = GameManager.instance.GetResource<Grid>("ScenarioEntry");
            scenariosPanel.Children.Add(entry);

            Span scenarioName = Utilities.FindNode<Span>(entry, "ScenarioName");
            scenarioName.Inlines.Add(data.name);
            scenarioName.MouseUp += ( sender, e ) => {
                GameManager.instance.StartScenario(data);
            };

            TextBlock scenarioDescription = Utilities.FindNode<TextBlock>(entry, "ScenarioDescription");
            scenarioDescription.Text = data.description.Trim();
        }

        public bool NavigateBack()
        {
            CloseAction();

            GameManager.instance.world = null;

            WaywardManager.instance.AddPage(
                new WorldsMenuPage(),
                WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.3)
            );

            return true;
        }
    }
}
