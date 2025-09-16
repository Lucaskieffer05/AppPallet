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
            Routing.RegisterRoute(nameof(CostoPorPalletCrearView), typeof(CostoPorPalletCrearView));
            Routing.RegisterRoute(nameof(PresupuestoMostrarView), typeof(PresupuestoMostrarView));
            Routing.RegisterRoute(nameof(PresupuestoModificarView), typeof(PresupuestoModificarView));

        }
    }
}
