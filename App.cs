    using SP2.Pages;

    namespace SP2
    {
        public partial class App : Application
        {
            protected override Window CreateWindow(IActivationState? activationState)
            {
                return new Window(new AppShell());
            }
        }
}