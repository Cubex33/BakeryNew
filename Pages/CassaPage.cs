using EF.Models;
using Microsoft.Maui.Controls;

namespace SP2.Pages
{
    public class CassaPage : ContentPage
    {
        Picker customerPicker = new() { Title = "Выберите клиента:" };
        VerticalStackLayout cassaLayout = new() { Padding = 10, Spacing = 5 };
        Dictionary<int, (int quantity, decimal price, int id)> selectedItems = new();
        bool isChecked;

        HorizontalStackLayout saleUseStack = new HorizontalStackLayout();

        List<int> customerIds = new();

        public CassaPage()
        {
            Title = "Cassa";

            customerPicker.SelectedIndexChanged += async (_, _) => { await ChangeCustomer(); await CheckDiscount(); } ;


            UpdatePicker();


            cassaLayout.Children.Add(customerPicker);

            var scrollview = new ScrollView { HeightRequest = 400 };

            var productsContainer = new VerticalStackLayout { Spacing = 10 };

            foreach (var product in DataProvider.dbContext.Products)
            {
                selectedItems[product.Id] = (0, product.Price, product.Id);

                var quantityLabel = new Label { Text = "0 шт" };
                var addButton = new Button { Text = "Добавить/изменить" };
                addButton.Clicked += async (_, _) => await CountLot(product, quantityLabel);

                var row = new HorizontalStackLayout
                {
                    Children =
                    {
                        new Label { Text = $"{product.Name}, Цена: {product.Price}", VerticalTextAlignment = TextAlignment.Center },
                        addButton,
                        quantityLabel
                    }
                };

                productsContainer.Children.Add(row);
            }

            var checkbox = new CheckBox
            {
                Margin = new Thickness(-10, 0, -10, 0)
            };

            var labelcheckbox = new Label
            {
                Text = "Использовать скидку",
                VerticalOptions = LayoutOptions.Center
            };

            saleUseStack = new HorizontalStackLayout
            {
                Spacing = 4,
                VerticalOptions = LayoutOptions.Center
            };

            saleUseStack.Children.Add(checkbox);
            saleUseStack.Children.Add(labelcheckbox);

            checkbox.CheckedChanged += (s, e) => isChecked = e.Value;

            scrollview.Content = productsContainer;

            cassaLayout.Children.Add(scrollview);

            var buyButton = new Button { Text = "Посчитать" };
            buyButton.Clicked += async (_, _) => await BuyLot();

            cassaLayout.Children.Add(saleUseStack);

            saleUseStack.IsVisible = false;

            cassaLayout.Children.Add(buyButton);
            Content = new ScrollView { Content = cassaLayout };
        }
        async Task CheckDiscount()
        {
            if (customerPicker.SelectedIndex == -1 ||
                customerPicker.SelectedIndex >= customerIds.Count)
            {
                saleUseStack.IsVisible = false;
                return;
            }

            int realId = customerIds[customerPicker.SelectedIndex];
            var customer = DataProvider.dbContext.Customers.FirstOrDefault(c => c.Id == realId);

            saleUseStack.IsVisible = customer?.Activion == 1;
        }
        void UpdatePicker()
        {
            customerPicker.Items.Clear();
            customerIds.Clear();
            foreach (var customer in DataProvider.dbContext.Customers)
            {
                customerPicker.Items.Add($"{customer.LastName} {customer.FirstName}");
                customerIds.Add(customer.Id); // сохраняем реальный ID
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

        public async Task ChangeCustomer()
        {
            try
            {
                var lengthPicker = customerPicker.Items.Count;
                if (customerPicker.SelectedIndex == lengthPicker - 1 && customerPicker.SelectedIndex != -1)
                {
                    var firstname = await DisplayPromptAsync(
                        title: "Создание пользователя 1/4",
                        message: null,
                        accept: "Далее",
                        cancel: "Отмена",
                        placeholder: "Ввидите имя клиента"
                    );
                    if (firstname == null) { customerPicker.SelectedIndex = -1; return; }
                    var lastname = await DisplayPromptAsync(
                        title: "Создание пользователя 2/4",
                        message: null,
                        accept: "Далее",
                        cancel: "Отмена",
                        placeholder: "Ввидите фамилию клиента"
                    );
                    if (lastname == null) { customerPicker.SelectedIndex = -1; return; }
                    var phone = await DisplayPromptAsync(
                        title: "Создание пользователя 3/4",
                        message: null,
                        accept: "Далее",
                        cancel: "Отмена",
                        placeholder: "Ввидите номер клиента",
                        keyboard: Keyboard.Telephone
                    );
                    if (phone == null) { customerPicker.SelectedIndex = -1; return; }
                    var email = await DisplayPromptAsync(
                        title: "Создание пользователя 4/4",
                        message: null,
                        accept: "Далее",
                        cancel: "Отмена",
                        placeholder: "Ввидите почту клиента",
                        keyboard: Keyboard.Email
                    );
                    if (email == null) { customerPicker.SelectedIndex = -1; return; }
                    var newCustomers = new Customer
                    {
                        FirstName = firstname,
                        LastName = lastname,
                        Phone = phone,
                        Email = email
                    };
                    DataProvider.dbContext.Customers.Add(newCustomers);
                    await DataProvider.dbContext.SaveChangesAsync();
                    customerPicker.SelectedIndex = -1;
                    UpdatePicker();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ошибка", $"{ex.InnerException?.Message ?? ex.Message}", "Ok");
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

            int realId = customerIds[customerPicker.SelectedIndex];

            var order = new Order
            {
                EmployeeId = Session.UserId,
                CustomerId = realId
            };
            DataProvider.dbContext.Orders.Add(order);

            var customer = await DataProvider.dbContext.Customers.FindAsync(realId);

            await DataProvider.dbContext.SaveChangesAsync();

            if (isChecked)
            {
                foreach (var item in itemsToBuy)
                {
                    DataProvider.dbContext.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.id,
                        Quantity = item.quantity,
                        Price = item.price * (1 - (customer.Discount ?? 0) / 100m)
                    });
                }
                customer.Activion = 0;
            }
            else
            {
                foreach (var item in itemsToBuy)
                {
                    DataProvider.dbContext.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.id,
                        Quantity = item.quantity,
                        Price = item.price
                    });
                }
            }

            await DataProvider.dbContext.SaveChangesAsync();
            await DisplayAlert("Успешно", "Заказ создан", "Ok");
        }
    }
}