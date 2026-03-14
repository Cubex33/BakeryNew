using Microsoft.Maui.Controls;

namespace SP2.Pages
{
    public class Panel : ContentPage
    {
        Button cassaButton = new Button { Text = "Cassa", Margin = new Thickness(2) };
        Button reportsButton = new Button { Text = "Reports", Margin = new Thickness(2) };
        Button usersButton = new Button { Text = "Users", Margin = new Thickness(2) };

        public Panel()
        {
            cassaButton.Clicked += async (_, _) => await Navigation.PushAsync(new CassaPage());
            reportsButton.Clicked += async (_, _) => await Navigation.PushAsync(new ReportsPage());
            usersButton.Clicked += async (_, _) => await Navigation.PushAsync(new UsersPage());
            

            Content = new VerticalStackLayout
            {
                Children = { cassaButton, reportsButton, usersButton },
                Padding = 10,
                Spacing = 10
            };
        }
    }
}