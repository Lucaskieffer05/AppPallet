using Microsoft.UI.Windowing;
using System.Globalization;
using Windows.Graphics;
using WinRT.Interop;

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


            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(App), (handler, view) =>
            {
#if WINDOWS
                var mauiWindow = handler.VirtualView;
                var nativeWindow = handler.PlatformView;
                var windowHandle = WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                // Obtener el área de trabajo de la pantalla
                var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);

                // Calcular 80% del ancho y alto de la pantalla
                int width = (int)(displayArea.WorkArea.Width * 0.9);
                int height = (int)(displayArea.WorkArea.Height * 0.9);

                appWindow.Resize(new SizeInt32(width, height));

                // Centrar ventana
                var centerX = displayArea.WorkArea.Width / 2 - width / 2;
                var centerY = displayArea.WorkArea.Height / 2 - height / 2;
                appWindow.Move(new PointInt32(centerX, centerY));
#endif
            });

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}