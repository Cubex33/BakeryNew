using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;
using SP2.Pages;

namespace SP2
{
    public class OpenReportPanel : ContentPage
    {
        BakeryDbContext dbContext = new BakeryDbContext();

        public Panel panel;

        public VerticalStackLayout reportPanel;
        public ScrollView scrollView;

        public OpenReportPanel(Panel panel)
        {
            this.panel = panel;
        }

        public void OpenPanel()
        {
            var closePanel = new Button { Text = "Назад", HorizontalOptions = LayoutOptions.Start, Margin = new Thickness(0, 0, 0, 10) };
            scrollView = new ScrollView();
            reportPanel = new VerticalStackLayout() { Children = { closePanel } };
            scrollView.Content = reportPanel;
            panel.Content = scrollView;
            closePanel.Clicked += (_, _) => { panel.Content = panel.mainPanel; reportPanel.Clear(); };

            var ordersDetails = dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Include(o => o.Employee)
                .ToList();

            foreach (var order in ordersDetails)
            {
                var employees = dbContext.Employees.FirstOrDefault(e => e.Id == order.EmployeeId);
                var customer = dbContext.Customers.FirstOrDefault(c => c.Id ==  order.CustomerId);
                var orderitem = dbContext.OrderItems.FirstOrDefault(o => o.OrderId == order.Id);

                if (employees == null) continue;
                if (customer == null) continue;
                if (orderitem == null) continue;

                var border = new Border
                {
                    BackgroundColor = Colors.Black,
                    Padding = 10,
                    Margin = new Thickness(5)
                };

                var verticalStackLayout = new VerticalStackLayout();

                foreach (var item in order.OrderItems)
                {
                    var product = dbContext.Products.FirstOrDefault(c => c.Id == item.ProductId);
                    if (product == null) continue;

                    var label = new Label
                    {
                        Text = $"{order.Id}. Продавец {employees.LastName} {employees.FirstName} продал {product.Name} покупателю {customer.FirstName} за {item.Price}₸ в количестве {orderitem.Quantity} шт. Дата: {order.OrderDate}"
                    };

                    verticalStackLayout.Children.Add(label);
                }

                border.Content = verticalStackLayout;
                reportPanel.Children.Add(border);
            }
        }
    }
}
