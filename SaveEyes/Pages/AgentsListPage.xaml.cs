using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SaveEyes.Data;

namespace SaveEyes.Pages
{
    /// <summary>
    /// Interaction logic for AgentsListPage.xaml
    /// </summary>
    public partial class AgentsListPage : Page
    {
        public IEnumerable<Agent> Agents { get; set; }
        public IEnumerable<Agent> AgentsForFilters { get; set; }

        public int PageNumber { get; set; } = 1;

        public List<AgentType> AgentTypes { get; set; }
        public Dictionary<string, Func<Agent, object>> Sortings { get; set; }

        public AgentsListPage()
        {
            InitializeComponent();
            Agents = DataAccess.GetAgents();
            AgentsForFilters = Agents.ToList();
            DataAccess.RefreshList += DataAccess_RefreshList;
            AgentTypes = DataAccess.GetAgentTypes();
            AgentTypes.Insert(0, new AgentType { Title = "Все типы" });


            DataContext = this;
        }

        private void DataAccess_RefreshList()
        {
            Agents = DataAccess.GetAgents();
            AgentsForFilters = Agents.ToList();
            ApplyFilters(true);
        }

        private void SetPageNumbers()
        {
            PageNumbersPanel.Children.Clear();
            int pagesCount = AgentsForFilters.Count() % 10 == 0 ? AgentsForFilters.Count() / 10 : AgentsForFilters.Count() / 10 + 1;
            for (int i = 0; i < pagesCount; i++)
            {
                var hyperlink = new Hyperlink()
                {
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 25,
                    TextDecorations = null
                };
                hyperlink.Inlines.Add($"{i + 1}");
                hyperlink.Click += NavigateToPage;

                var textBlock = new TextBlock() { Margin = new Thickness(5, 0, 5, 0) };

                if (i == PageNumber - 1)
                    hyperlink.TextDecorations = TextDecorations.Underline;

                textBlock.Inlines.Add(hyperlink);

                PageNumbersPanel.Children.Add(textBlock);
            }
        }

        private void ApplyFilters(bool filtersChanged)
        {
            var filter = cbFilter.SelectedItem as AgentType;
            var text = tbSearch.Text.ToLower();
            if (filtersChanged)
                PageNumber = 1;

            if (filter != null)
            {
                if (filter.Title != "Все типы")
                    AgentsForFilters = Agents.Where(x => x.AgentType == filter).ToList();
                else
                    AgentsForFilters = Agents;

                AgentsForFilters = AgentsForFilters.ToList();

                AgentsForFilters = AgentsForFilters.Where(x => x.Title.ToLower().Contains(text) 
                                                    || x.Email.ToLower().Contains(text)
                                                    || x.Phone.ToLower().Contains(text)).ToList();

                lvAgents.ItemsSource = new ObservableCollection<Agent>(AgentsForFilters.Skip((PageNumber - 1) * 10).Take(10).ToList());
            }
            SetPageNumbers();
        }

        private void NavigateToPage(object sender, RoutedEventArgs e)
        {
            PageNumber = int.Parse(((sender as Hyperlink).Inlines.FirstOrDefault() as Run).Text);
            (sender as Hyperlink).TextDecorations = TextDecorations.Underline;
            ApplyFilters(false);
        }


        private void lvAgents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters(true);
        }

        private void FiltersChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters(true);
        }
        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageNumber > 1)
            {
                PageNumber--;
                ApplyFilters(false);
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            int pagesCount = AgentsForFilters.Count() % 10 == 0 ? AgentsForFilters.Count() / 10 : AgentsForFilters.Count() / 10 + 1;
            if (PageNumber < pagesCount)
            {
                PageNumber++;
                ApplyFilters(false);
            }
        }

    }
}
