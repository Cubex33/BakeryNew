using BakeryApp.Models;

namespace SP2.Pages
{
    public class MainPage : ContentPage
    {
        BakeryDbContext dbContext = new BakeryDbContext();

        Entry usernameInputField;
        Entry passwordInputField;
        Button signInButton;
        Button showPassword;

        public MainPage() {
            usernameInputField = new Entry
            {
                Text = "admin",
                Placeholder = "Username",
                Margin = new Thickness(2),
                WidthRequest = 500
            };

            passwordInputField = new Entry
            {
                Text = "admin",
                Placeholder = "Password",
                Margin = new Thickness(2),
                WidthRequest = 460,
                IsPassword = true
            };

            signInButton = new Button
            {
                Text = "Sign In",
                Margin = new Thickness(2),
                WidthRequest = 500
            };

            showPassword = new Button {
                Text = "S",
                Margin = new Thickness(2),
            };

            var Horizontal = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    passwordInputField, showPassword
                }
            };

            usernameInputField.Completed += (_, _) => passwordInputField.Focus();
            passwordInputField.Completed += async (_, _) => await CheckUser();
            signInButton.Clicked += async (_, _) => await CheckUser();
            showPassword.Clicked += async (_, _) => await PasswordShower();

            Content = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding=20,
                Children =
                {
                    usernameInputField, Horizontal, signInButton,
                }
            };
        }

        private async Task PasswordShower()
        {
            passwordInputField.IsPassword = !passwordInputField.IsPassword;
            showPassword.Text = passwordInputField.IsPassword ? "S" : "H";
        }

        private async Task CheckUser()
        {
            var users = dbContext.Users.Where(c => c.Username == usernameInputField.Text && c.Password == passwordInputField.Text).ToList();
            if (users.Count > 0)
            {
                var user = users.First();
                Session.UserId = user.Id;
                Session.IsAdmin = user.IsAdmin;
                await Navigation.PushAsync(new Panel());
            }
        }
    }
}
