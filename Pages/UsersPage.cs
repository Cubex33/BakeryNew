using EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System;

namespace SP2.Pages
{
    public class UsersPage : ContentPage
    {
        public UsersPage()
        {
            Title = "Users";
            var layout = new VerticalStackLayout { Padding = 10, Spacing = 5 };
            foreach (var user in DataProvider.dbContext.Customers)
            {
                var border = new Border
                {
                    BackgroundColor = Colors.Black,
                    Padding = 5,
                    Margin = new Thickness(5)
                };

                var active = user.Activion == 1 ? "Нет" : "Да";

                var button = new Button { Text = "изменить" };

                var label = new Label { Text = $"{user.Id}. {user.FirstName} {user.LastName} со скидкой: {user.Discount}% активирован:{active}", VerticalTextAlignment = TextAlignment.Center };

                var horizontalStack = new HorizontalStackLayout
                {
                    Children =
                    {
                        label,
                        button
                    }
                };

                button.Clicked += async (_, _) => await updateDiscount(user, label);

                border.Content = horizontalStack;

                layout.Children.Add(border);
            }

            Content = new ScrollView { Content = layout };
        }

        public async Task updateDiscount(Customer user, Label label)
        {
            try
            {
                string discount = await DisplayPromptAsync(
                    title: "Скидка",
                    message: null,
                    placeholder: "Введите скидку",
                    accept: "Выдать",
                    cancel: "Отмена",
                    keyboard: Keyboard.Numeric
                );

                if (discount != null)
                {
                    if (!int.TryParse(discount, out int Discount) || Discount > 100 || Discount < 0)
                    {
                        await DisplayAlertAsync("Ошибка", "Введены не коректные данные", "Ок");
                        return;
                    }

                    user.Discount = Discount;
                    user.Activion = 1;

                    var active = user.Activion == 1 ? "Да" : "Нет";

                    label.Text = $"{user.Id}. {user.FirstName} {user.LastName} со скидкой: {user.Discount}% активирован:{active}";
                    DataProvider.dbContext.Customers.Update(user);
                    await DataProvider.dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ошибка", $"Ошибка: {ex.InnerException?.Message ?? ex.Message}", "Ок");
            }
        }
    }
}