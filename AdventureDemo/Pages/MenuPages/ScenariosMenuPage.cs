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
    class ScenariosMenuPage : WaywardEngine.ContentPage
    {
        StackPanel scenariosPanel;

        public ScenariosMenuPage()
        {
            FrameworkElement content = GameManager.instance.GetResource<FrameworkElement>("ScenariosMenu");
            AddContent(content);
            SetTitle("Scenarios");
            element.ContextMenu = null;

            scenariosPanel = Utilities.FindNode<StackPanel>(element, "Scenarios");
            DisplayContent();
        }

        public override void Clear()
        {
            scenariosPanel.Children.Clear();
        }

        void DisplayContent()
        {
            ScenarioData[] scenarios = DataManager.instance.GetScenarioDatas();

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
    }
}
