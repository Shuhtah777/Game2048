namespace Game2048
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell())
            {
                Width = 450,
                Height = 850,
                MinimumWidth = 400,
                MinimumHeight = 700
            };
        }
    }
}