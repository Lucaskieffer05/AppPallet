using AppPallet.Views;

namespace AppPallet
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ChequeView), typeof(ChequeView));
            Routing.RegisterRoute(nameof(EmpresaView), typeof(EmpresaView));
        }
    }
}
