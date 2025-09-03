using AppPallet.View;

namespace AppPallet
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ChequeModificarView), typeof(ChequeModificarView));
        }
    }
}
