using BakeryApp.Models;
using Microsoft.Maui.Controls;

namespace SP2.Pages
{
    public class CassaPage : ContentPage
    {
        BakeryDbContext dbContext = new BakeryDbContext();
        Picker customerPicker = new() { Title = "Выберите клиента:" };
        VerticalStackLayout cassaLayout = new() { Padding = 10, Spacing = 5 };
        Dictionary<int, (int quantity, decimal price, int id)> selectedItems = new();

        public CassaPage()
        {
            Title = "Cassa";
            var backButton = new Button { Text = "Назад" };
            backButton.Clicked += (_, _) => Navigation.PopAsync();

            
            UpdatePicker();

            cassaLayout.Children.Add(backButton);
            cassaLayout.Children.Add(customerPicker);

            foreach (var product in dbContext.Products)
            {
                selectedItems[product.Id] = (0, product.Price, product.Id);

                var quantityLabel = new Label { Text = "0 шт" };
                var addButton = new Button { Text = "Добавить/изменить" };
                addButton.Clicked += async (_, _) => await CountLot(product, quantityLabel);

                cassaLayout.Children.Add(new HorizontalStackLayout
                {
                    Children =
                    {
                        new Label { Text = $"{product.Name}, Цена: {product.Price}", VerticalTextAlignment = TextAlignment.Center },
                        addButton,
                        quantityLabel
                    }
                });
            }

            var buyButton = new Button { Text = "Посчитать" };
            buyButton.Clicked += async (_, _) => await BuyLot();

            cassaLayout.Children.Add(buyButton);
            Content = new ScrollView { Content = cassaLayout };
        }

        void UpdatePicker()
        {
            customerPicker.Items.Clear();
            foreach (var customer in dbContext.Customers)
            {
                customerPicker.Items.Add($"{customer.LastName} {customer.FirstName}");
            }
            customerPicker.Items.Add("Добавить пользователя");
        }

        async Task CountLot(Product product, Label label)
        {
            string input = await DisplayPromptAsync(
                $"Количество для {product.Name}",
                "Введите количество",
                accept: "Ok",
                cancel: "Отмена",
                keyboard: Keyboard.Numeric
            );

            if (int.TryParse(input, out int count))
            {
                selectedItems[product.Id] = (count, product.Price * count, product.Id);
                label.Text = $"{count} шт";
            }
        }

        async Task BuyLot()
        {
            var itemsToBuy = selectedItems.Values.Where(x => x.quantity > 0).ToList();
            if (!itemsToBuy.Any())
            {
                await DisplayAlert("Ошибка", "Вы не выбрали товары", "Ok");
                return;
            }

            if (customerPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Ошибка", "Вы не выбрали покупателя", "Ok");
                return;
            }

            var confirmation = await DisplayAlert("Подтверждение", "Подтверждаете продажу?", "Да", "Нет");
            if (!confirmation) return;

            var order = new Order
            {
                EmployeeId = Session.UserId,
                CustomerId = customerPicker.SelectedIndex // упрощенно
            };
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            foreach (var item in itemsToBuy)
            {
                dbContext.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.id,
                    Quantity = item.quantity,
                    Price = item.price
                });
            }

            await dbContext.SaveChangesAsync();
            await DisplayAlert("Успешно", "Заказ создан", "Ok");
        }
    }
}