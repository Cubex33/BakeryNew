using EF.Models;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;

namespace SP2.Pages
{
    public class ReportsPage : ContentPage
    {

        public ReportsPage()
        {
            Title = "Reports";
            var layout = new VerticalStackLayout { Padding = 10, Spacing = 5 };


            var orders = DataProvider.dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();

            foreach (var order in orders)
            {
                var border = new Border
                {
                    BackgroundColor = Colors.Black,
                    Padding = 5,
                    Margin = new Thickness(5)
                };

                var innerLayout = new VerticalStackLayout();
                foreach (var item in order.OrderItems)
                {
                    innerLayout.Children.Add(new Label
                    {
                        Text = $"Заказ {order.Id}: {order.Employee.LastName} продал {item.Product.Name} покупателю {order.Customer.FirstName} {order.Customer.LastName} - {item.Quantity} шт. за {item.Price}₸, дата: {order.OrderDate}"
                    });
                }

                border.Content = innerLayout;
                layout.Children.Add(border);
            }

            Content = new ScrollView { Content = layout };
        }
    }
}