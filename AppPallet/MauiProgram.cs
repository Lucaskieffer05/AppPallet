using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.ViewModels;
using AppPallet.Views;
using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UraniumUI;

namespace AppPallet
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFontAwesomeIconFonts();
                });

            /*
            builder.Services.AddDbContext<PalletContex>(options =>
            options.UseSqlServer("Data Source=LUCAS\\SQLEXPRESS;Initial Catalog=Pallet;Persist Security Info=True;User ID=sa;Password=42559251;Trust Server Certificate=True"));
            */

            builder.Services.AddDbContext<PalletContext>(options =>
                options.UseSqlServer(Preferences.Get("database_connection_string", "Server=localhost;Database=AppPallet;Trusted_Connection=true;")));

            //builder.Services.AddTransient<AppShell>();
            //builder.Services.AddTransient<IPopupService, PopupService>();

            // Registro de Controladores
            //builder.Services.AddSingleton<IPopupService, PopupService>();

            builder.Services.AddTransient<ChequeController>();
            builder.Services.AddTransient<EmpresaController>();
            builder.Services.AddTransient<GastosFijosController>();
            builder.Services.AddTransient<CostoPorPalletController>();
            builder.Services.AddTransient<ContactosEmpresaController>();
            builder.Services.AddTransient<AreaController>();
            builder.Services.AddTransient<CostoPorCamionController>();
            builder.Services.AddTransient<IngresoController>();
            builder.Services.AddTransient<EgresoController>();
            builder.Services.AddTransient<ActivoPasivoController>();
            builder.Services.AddTransient<VentaController>();
            builder.Services.AddTransient<LoteController>();
            builder.Services.AddTransient<PedidoController>();
            builder.Services.AddTransient<PalletController>();
            builder.Services.AddTransient<HistorialHumedadController>();


            // Registro de Vistas y ViewModels

            builder.Services.AddTransient<ChequeView>();
            builder.Services.AddTransient<ChequeViewModel>();

            builder.Services.AddTransient<EmpresaView>();
            builder.Services.AddTransient<EmpresaViewModel>();

            builder.Services.AddTransient<GastosFijosView>();
            builder.Services.AddTransient<GastosFijosViewModel>();

            builder.Services.AddTransient<CostoPorPalletCrearView>();
            builder.Services.AddTransient<CostoPorPalletCrearViewModel>();

            builder.Services.AddTransient<PresupuestoView>();
            builder.Services.AddTransient<PresupuestoViewModel>();

            builder.Services.AddTransient<PresupuestoMostrarView>();
            builder.Services.AddTransient<PresupuestoMostrarViewModel>();

            builder.Services.AddTransient<PresupuestoModificarView>();
            builder.Services.AddTransient<PresupuestoModificarViewModel>();

            builder.Services.AddTransient<IngresoEgresoView>();
            builder.Services.AddTransient<IngresoEgresoViewModel>();

            builder.Services.AddTransient<IngresoCrearView>();
            builder.Services.AddTransient<IngresoCrearViewModel>();

            builder.Services.AddTransient<IngresoModificarView>();
            builder.Services.AddTransient<IngresoModificarViewModel>();

            builder.Services.AddTransient<EgresoCrearView>();
            builder.Services.AddTransient<EgresoCrearViewModel>();

            builder.Services.AddTransient<EgresoModificarView>();
            builder.Services.AddTransient<EgresoModificarViewModel>();

            builder.Services.AddTransient<ActivoPasivoView>();
            builder.Services.AddTransient<ActivoPasivoViewModel>();

            builder.Services.AddTransient<ActivoPasivoCrearView>();
            builder.Services.AddTransient<ActivoPasivoCrearViewModel>();

            builder.Services.AddTransient<ActivoPasivoModificarView>();
            builder.Services.AddTransient<ActivoPasivoModificarViewModel>();

            builder.Services.AddTransient<VentaView>();
            builder.Services.AddTransient<VentaViewModel>();

            builder.Services.AddTransient<VentaCrearView>();
            builder.Services.AddTransient<VentaCrearViewModel>();

            builder.Services.AddTransient<VentaModificarView>();
            builder.Services.AddTransient<VentaModificarViewModel>();

            builder.Services.AddTransient<LoteView>();
            builder.Services.AddTransient<LoteViewModel>();

            builder.Services.AddTransient<LoteCrearView>();
            builder.Services.AddTransient<LoteCrearViewModel>();

            builder.Services.AddTransient<LoteModificarView>();
            builder.Services.AddTransient<LoteModificarViewModel>();

            builder.Services.AddTransient<PedidoView>();
            builder.Services.AddTransient<PedidoViewModel>();

            builder.Services.AddTransient<PedidoCrearView>();
            builder.Services.AddTransient<PedidoCrearViewModel>();

            builder.Services.AddTransient<PedidoModificarView>();
            builder.Services.AddTransient<PedidoModificarViewModel>();

            builder.Services.AddTransient<StockView>();
            builder.Services.AddTransient<StockViewModel>();

            builder.Services.AddTransient<PalletView>();
            builder.Services.AddTransient<PalletViewModel>();

            builder.Services.AddTransient<PalletCrearView>();
            builder.Services.AddTransient<PalletCrearViewModel>();

            builder.Services.AddTransient<PalletModificarView>();
            builder.Services.AddTransient<PalletModificarViewModel>();

            builder.Services.AddTransient<HistorialHumedadView>();
            builder.Services.AddTransient<HistorialHumedadViewModel>();

            builder.Services.AddTransient<HistorialHumedadCrearView>();
            builder.Services.AddTransient<HistorialHumedadCrearViewModel>();

            builder.Services.AddTransient<HistorialHumedadModificarView>();
            builder.Services.AddTransient<HistorialHumedadModificarViewModel>();

            builder.Services.AddTransient<ConfiguracionView>();
            builder.Services.AddTransient<ConfiguracionViewModel>();

            builder.Services.AddTransient<DashboardView>();
            builder.Services.AddTransient<DashboardViewModel>();








            builder.Services.AddTransientPopup<ChequeCrearView, ChequeCrearViewModel>();
            builder.Services.AddTransientPopup<ChequeModificarView, ChequeModificarViewModel>();
            builder.Services.AddTransientPopup<EmpresaCrearView, EmpresaCrearViewModel>();
            builder.Services.AddTransientPopup<EmpresaModificarView, EmpresaModificarViewModel>();
            builder.Services.AddTransientPopup<GastosFijosCrearView, GastosFijosCrearViewModel>();
            builder.Services.AddTransientPopup<GastosFijosModificarView, GastosFijosModificarViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif





            return builder.Build();
        }
    }
}
