using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

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
                foreach (var discount in dbContext.Discounts.Where(d => d.CustomerId == user.Id))
                {
                    var border = new Border
                    {
                        BackgroundColor = Colors.Black,
                        Padding = 5,
                        Margin = new Thickness(5)
                    };

                    var button = new Button { Text = "изменить" };

                    var horizontalStack = new HorizontalStackLayout
                    {
                        Children =
                        {
                            new Label { Text = $"{user.Id}. {user.FirstName} {user.LastName} со скидкой: {discount.Discount1}", VerticalTextAlignment = TextAlignment.Center },
                            button
                        }
                    };

                    border.Content = horizontalStack;

                    layout.Children.Add(border);
                }
            }

            Content = new ScrollView { Content = layout };
        }
    }
}