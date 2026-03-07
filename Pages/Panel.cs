
using BakeryApp.Models;

namespace SP2.Pages
{
    public class Panel : ContentPage
    {
        BakeryDbContext dbContext = new BakeryDbContext();
        Button cassaOpenButton;
        Button reportOpenButton;
        Button addUserOpenButton;
        VerticalStackLayout cassaPanel;
        Dictionary<int, (int quatity, decimal price, int id)> selectitems = new();  
        Label count;
        int countProducts;
        Picker picker = new Picker
        {
            Title = "Выберите клиента: "
        };
        int customerId = 0;

        public Panel() {
            cassaOpenButton = new Button { Text = "Cassa", Margin = new Thickness(2) };

            reportOpenButton = new Button { Text = "Report", Margin = new Thickness(2) };

            addUserOpenButton = new Button { Text = "Users", Margin = new Thickness(2) };

            cassaPanel = new VerticalStackLayout { IsVisible = false };

            cassaOpenButton.Clicked += async (_, _) => await OpenCassa();

            if (Session.IsAdmin)
            {
                Content = new VerticalStackLayout
                {
                    Children =
                    {
                        cassaOpenButton, reportOpenButton, addUserOpenButton
                    }
                };
            }
            else
            {
                _ = OpenCassa();
            }

        }

        public async Task OpenCassa()
        {
            Product product = new();
            cassaPanel.IsVisible = true;

            foreach (var customers in dbContext.Customers)
            {
                picker.Items.Add($"{customers.LastName} {customers.FirstName}");
            }

            var label = new Label
            {
                Text = "Products"
            };
            var countingButton = new Button
            {
                Text = "Посчитать"
            };

            cassaPanel.Children.Add( picker );

            picker.SelectedIndexChanged += async (_, _) => await ChangeCustomer();

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
        }

        public async Task ChangeCustomer()
        {
            if (picker.SelectedIndex != -1)
            {
                customerId = picker.SelectedIndex;
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
