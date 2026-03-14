using Microsoft.Maui.Controls;
using SP2.Pages;

namespace SP2
{
    public class AppShell : Shell
    {
        public AppShell()
        {
            var flyoutItem = new FlyoutItem { Title = "Меню" };
            flyoutItem.Items.Add(new ShellContent { Title = "Panel", ContentTemplate = new DataTemplate(typeof(Panel)), Route = nameof(Panel) });
            flyoutItem.Items.Add(new ShellContent { Title = "Cassa", ContentTemplate = new DataTemplate(typeof(CassaPage)), Route = nameof(CassaPage) });
            flyoutItem.Items.Add(new ShellContent { Title = "Reports", ContentTemplate = new DataTemplate(typeof(ReportsPage)), Route = nameof(ReportsPage) });
            flyoutItem.Items.Add(new ShellContent { Title = "Users", ContentTemplate = new DataTemplate(typeof(UsersPage)), Route = nameof(UsersPage) });
            Items.Add(flyoutItem);
        }

        public void ShowLogin()
        {

        }

        public void ShowApp()
        {

        }
    }
}