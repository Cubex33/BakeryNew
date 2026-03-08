using BakeryApp.Models;
using Microsoft.Maui.Controls;

namespace SP2.Pages
{
    public class Panel : ContentPage
    {
        BakeryDbContext dbContext = new BakeryDbContext();
        Button cassaOpenButton;
        Button reportOpenButton;
        Button addUserOpenButton;
        VerticalStackLayout cassaPanel;
        public VerticalStackLayout mainPanel;
        Dictionary<int, (int quatity, decimal price, int id)> selectitems = new();  
        Label count;
        int countProducts;

        public OpenReportPanel reportPanel;
        public OpenUserPanel userPanel;

        Picker picker = new Picker
        {
            Title = "Выберите клиента: "
        };
        int customerId = 0;
        bool panelIsOpened = false;

        public Panel() {
            reportPanel = new OpenReportPanel(this);
            userPanel = new OpenUserPanel(this);

            cassaOpenButton = new Button { Text = "Cassa", Margin = new Thickness(2) };

            reportOpenButton = new Button { Text = "Report", Margin = new Thickness(2) };

            addUserOpenButton = new Button { Text = "Users", Margin = new Thickness(2) };

            reportOpenButton.Clicked += (_, _) => reportPanel.OpenPanel();

            addUserOpenButton.Clicked += (_, _) => userPanel.OpenPanel();

            cassaPanel = new VerticalStackLayout { IsVisible = false };

            mainPanel = new VerticalStackLayout { Children = { cassaOpenButton, reportOpenButton, addUserOpenButton } };

            cassaOpenButton.Clicked += async (_, _) => await OpenCassa();

            if (Session.IsAdmin)
            {
                Content = mainPanel;
            }
            else
            {
                _ = OpenCassa();
            }

        }

        

        public async Task OpenCassa()
        {
            cassaPanel.Children.Clear();
            Product product = new();
            cassaPanel.IsVisible = true;

            var closeCassa = new Button
            {
                Text = "Назад",
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 0, 0, 10)
            };

            closeCassa.Clicked += (_, _) =>
            {
                cassaPanel.IsVisible = false;
                Content = mainPanel;
            };

            cassaPanel.Children.Add(closeCassa);

            var label = new Label
            {
                Text = "Products"
            };
            var countingButton = new Button
            {
                Text = "Посчитать"
            };

            cassaPanel.Children.Add( picker );

            if (!panelIsOpened)
            {
                picker.SelectedIndexChanged += async (_, _) => await ChangeCustomer();
            }

            foreach (var items in dbContext.Products)
            {
                selectitems[items.Id] = (quatity: 0, price: items.Price, id: items.Id);
                Label count = new Label { VerticalOptions = LayoutOptions.Center, Margin = new Thickness(2) };

                Button countLotButton = new Button { Text = "Добавить/изменить" };
                var border = new Border
                {
                    BackgroundColor = Colors.Black,
                    Margin = new Thickness(2),
                    HeightRequest = 50,
                    Content = new HorizontalStackLayout
                    {
                        Children = {
                            new Label
                            {
                                VerticalTextAlignment = TextAlignment.Center,
                                Text = $"{items.Name}, Цена: {items.Price}"
                            },
                            countLotButton,
                            count
                        }
                    }
                };
                countLotButton.Clicked += async (_, _) => await countLot(items, count);
                cassaPanel.Children.Add(border);
                product = items;
                Content = cassaPanel;
            }
            countingButton.Clicked += async (_, _) => await BuyLot(product);
            cassaPanel.Children.Add(countingButton);
            UpdatePicker();
        }

        public void UpdatePicker()
        {
            picker.Items.Clear();
            foreach (var customers in dbContext.Customers)
            {
                picker.Items.Add($"{customers.LastName} {customers.FirstName}");
            }
            picker.Items.Add("Добавить пользователя");
        }

        public async Task ChangeCustomer()
        {
            try
            {
                var lengthPicker = picker.Items.Count;
                if (picker.SelectedIndex == lengthPicker - 1)
                {
                    var firstname = await DisplayPromptAsync(
                        title: "Создание пользователя 1/4",
                        message: null,
                        accept: "Далее",
                        cancel: "Отмена",
                        placeholder: "Ввидите имя клиента"
                    );
                    if (firstname == null) picker.SelectedIndex = -1; return;
                    var lastname = await DisplayPromptAsync(
                        title: "Создание пользователя 2/4",
                        message: null,
                        accept: "Далее",
                        cancel: "Отмена",
                        placeholder: "Ввидите фамилию клиента"
                    );
                    if (lastname == null) picker.SelectedIndex = -1; return;
                    var phone = await DisplayPromptAsync(
                        title: "Создание пользователя 3/4",
                        message: null,
                        accept: "Далее",
                        cancel: "Отмена",
                        placeholder: "Ввидите номер клиента",
                        keyboard: Keyboard.Telephone
                    );
                    if (phone == null) picker.SelectedIndex = -1; return;
                    var email = await DisplayPromptAsync(
                        title: "Создание пользователя 4/4",
                        message: null,
                        accept: "Далее",
                        cancel: "Отмена",
                        placeholder: "Ввидите почту клиента",
                        keyboard: Keyboard.Email
                    );
                    if (email == null) picker.SelectedIndex = -1; return;
                    var newCustomers = new Customer
                    {
                        FirstName = firstname,
                        LastName = lastname,
                        Phone = phone,
                        Email = email
                    };
                    dbContext.Customers.Add(newCustomers);
                    await dbContext.SaveChangesAsync();
                    UpdatePicker();
                }
                else if (picker.SelectedIndex != -1 && picker.SelectedIndex != lengthPicker)
                {
                    customerId = picker.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ошибка", $"{ex.InnerException?.Message ?? ex.Message}", "Ok");
            }
        }

        public async Task countLot<T, R>(T item, R count) where T : Product where R : Label
        {
            string countInput;
            if (count.Text == null)
            {
                countInput = await DisplayPromptAsync(
                    title: $"Действие для товара '{item.Name}'",
                    message: null,
                    accept: "Добавить",
                    cancel: "Отмена",
                    placeholder: "Ввидете количество",
                    keyboard: Keyboard.Numeric
                );
            }
            else
            {
                countInput = await DisplayPromptAsync(
                    title: $"Действие для товара '{item.Name}'",
                    message: null,
                    accept: "Изменить",
                    cancel: "Отмена",
                    placeholder: "Ввидете новое количество",
                    keyboard: Keyboard.Numeric
                );
            }

            if (!string.IsNullOrEmpty(countInput))
            {
                if (int.TryParse(countInput, out int coint))
                {
                    countProducts = coint;
                    selectitems[item.Id] = (coint, item.Price * coint, item.Id);
                    UpdateText($"{countProducts} шт", label: count);
                }
            }
        }

        public async Task BuyLot<T>(T item) where T : Product
        {
            var acception = await DisplayAlertAsync(
                title: "Подверждение",
                message: "Подверждаете ли вы продажу товара?",
                accept: "Да",
                cancel: "Нет"
            );

            if (acception)
            {
                try
                {
                    var selectionitem = selectitems.Values.Where(x => x.quatity > 0).ToList();
                    if (!selectionitem.Any())
                    {
                        await DisplayAlertAsync("Ошибка", "Вы не выбрали товары", "Ок");
                        return;
                    }

                    if (picker.SelectedIndex == -1)
                    {
                        await DisplayAlertAsync("Ошибка", "Вы не выбрали покупателя", "Ок");
                        return;
                    }

                    foreach (var content in selectionitem)
                    {
                        var newOrder = new Order
                        {
                            EmployeeId = Session.UserId,
                            CustomerId = customerId
                        };
                        dbContext.Orders.Add(newOrder);
                        await dbContext.SaveChangesAsync();
                        foreach (var items in selectionitem)
                        {
                            dbContext.OrderItems.Add(new OrderItem
                            {
                                OrderId = newOrder.Id,
                                ProductId = items.id,
                                Quantity = items.quatity,
                                Price = items.price,
                            });
                        }

                        await dbContext.SaveChangesAsync();
                        await DisplayAlertAsync("Успешно", "Заказ умпешно создан", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlertAsync("Ошибка", $"{ex.InnerException?.Message ?? ex.Message}", "Ok");
                }
            }
        }

        public void UpdateText(string text, Label label)
        {
            label.Text = text;
        }
    }
}
