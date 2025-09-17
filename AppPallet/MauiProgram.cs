using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.ViewModels;
using AppPallet.Views;
using CommunityToolkit.Maui;
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

            builder.Services.AddSingleton<PalletContext>();

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
