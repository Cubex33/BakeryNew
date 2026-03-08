using BakeryApp.Models;

namespace SP2.Pages
{
    public class OpenUserPanel
    {
        BakeryDbContext dbContext = new BakeryDbContext();
        public Panel panel;
        VerticalStackLayout userPanel;

        public OpenUserPanel(Panel panel)
        {
            this.panel = panel;
        }

        public void OpenPanel()
        {
            var closeButton = new Button
            {
                Text = "Назад",
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 0, 0, 10)
            };

            closeButton.Clicked += (_, _) => { panel.Content = panel.mainPanel; userPanel.Clear(); };

            userPanel = new VerticalStackLayout { Children = { closeButton } };
            panel.Content = userPanel;

            foreach (var users in dbContext.Employees)
            {

            }
        }
    }
}
