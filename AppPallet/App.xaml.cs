using System.Globalization;

namespace AppPallet
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Forzar el idioma a español
            var culture = new CultureInfo("es-ES"); // Cambia a "es-MX" si prefieres español latinoamericano
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}