    using SP2.Pages;

    namespace SP2
    {
    public partial class App : Application
    {
        [Obsolete("This constructor is obsolete. Use the constructor with parameters instead.")]
        public App()
        {
            MainPage = new NavigationPage(new MainPage());
        }
    }
}