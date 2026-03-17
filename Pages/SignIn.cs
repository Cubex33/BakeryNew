using EF.Models;
using Microsoft.EntityFrameworkCore;

namespace SP2.Pages
{
    public class MainPage : ContentPage
    {
        Entry usernameInputField = new()
        {
            Text = "admin",
            Placeholder = "Username",
            Margin = new Thickness(2),
            WidthRequest = 500
        };
        Entry passwordInputField = new()
        {
            Text = "admin",
            Placeholder = "Password",
            Margin = new Thickness(2),
            WidthRequest = 460,
            IsPassword = true
        };
        Button signInButton = new()
        {
            Text = "Sign In",
            Margin = new Thickness(2),
            WidthRequest = 500
        };
        Button showPassword = new()
        {
            Text = "S",
            Margin = new Thickness(2),
        };

        public MainPage() {
            //usernameInputField = new Entry
            //{
            //    Text = "admin",
            //    Placeholder = "Username",
            //    Margin = new Thickness(2),
            //    WidthRequest = 500
            //};

            //passwordInputField = new Entry
            //{
            //    Text = "admin",
            //    Placeholder = "Password",
            //    Margin = new Thickness(2),
            //    WidthRequest = 460,
            //    IsPassword = true
            //};

            //signInButton = new Button
            //{
            //    Text = "Sign In",
            //    Margin = new Thickness(2),
            //    WidthRequest = 500
            //};

            //showPassword = new Button {
            //    Text = "S",
            //    Margin = new Thickness(2),
            //};

            //var Horizontal = new HorizontalStackLayout
            //{
            //    HorizontalOptions = LayoutOptions.Center,
            //    Children =
            //    {
            //        passwordInputField, showPassword
            //    }
            //};

            usernameInputField.Completed += (_, _) => passwordInputField.Focus();
            passwordInputField.Completed += async (_, _) => await CheckUser();
            signInButton.Clicked += async (_, _) => await CheckUser();
            showPassword.Clicked += (_, _) =>
            {
                passwordInputField.IsPassword = !passwordInputField.IsPassword;
                showPassword.Text = passwordInputField.IsPassword ? "S" : "H";
            };

            Content = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding=20,
                Children =
                {
                    usernameInputField,
                    new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Children =
                        {
                            passwordInputField, showPassword
                        }
                    }, 
                    signInButton,
                }
            };
        }

        //private async Task PasswordShower()
        //{
        //    passwordInputField.IsPassword = !passwordInputField.IsPassword;
        //    showPassword.Text = passwordInputField.IsPassword ? "S" : "H";
        //}

        //private async Task CheckUser()
        //{
        //    var users = dbContext.Users.Where(c => c.Username == usernameInputField.Text && c.Password == passwordInputField.Text).ToList();
        //    if (users.Count > 0)
        //    {
        //        var user = users.First();
        //        Session.UserId = user.Id;
        //        Session.IsAdmin = user.IsAdmin;
        //        await Navigation.PushAsync(new Panel());
        //    }
        //}
        private async Task CheckUser()
        {
            var user = await DataProvider.dbContext.Users.FirstOrDefaultAsync(u => u.Username == usernameInputField.Text && u.Password == passwordInputField.Text);
            if (user == null) return;
            Session.UserId = user.Id;
            Session.IsAdmin = user.IsAdmin;
            await Navigation.PushAsync(new Panel());
        }
    }
}
