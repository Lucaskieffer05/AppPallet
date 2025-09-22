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
            Routing.RegisterRoute(nameof(IngresoCrearView), typeof(IngresoCrearView));
            Routing.RegisterRoute(nameof(IngresoModificarView), typeof(IngresoModificarView));
            Routing.RegisterRoute(nameof(EgresoModificarView), typeof(EgresoModificarView));
            Routing.RegisterRoute(nameof(EgresoCrearView), typeof(EgresoCrearView));
            Routing.RegisterRoute(nameof(ActivoPasivoModificarView), typeof(ActivoPasivoModificarView));
            Routing.RegisterRoute(nameof(ActivoPasivoCrearView), typeof(ActivoPasivoCrearView));
            Routing.RegisterRoute(nameof(VentaModificarView), typeof(VentaModificarView));
            Routing.RegisterRoute(nameof(VentaCrearView), typeof(VentaCrearView));

        }
    }
}
