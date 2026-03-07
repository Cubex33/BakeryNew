    using SP2.Pages;

    namespace SP2
    {
        public class App : Application
        {
            public App()
            {
                MainPage = new NavigationPage(new MainPage());
            }
        }
    }