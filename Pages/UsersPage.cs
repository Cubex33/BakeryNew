using BakeryApp.Models;

namespace SP2.Pages
{
    public class UsersPage : ContentPage
    {
        BakeryDbContext dbContext = new BakeryDbContext();

        public UsersPage()
        {
            Title = "Users";
            var layout = new VerticalStackLayout { Padding = 10, Spacing = 5 };

            foreach (var user in dbContext.Employees)
            {
                layout.Children.Add(new Label { Text = $"{user.FirstName} {user.LastName}" });
            }

            Content = new ScrollView { Content = layout };
        }
    }
}