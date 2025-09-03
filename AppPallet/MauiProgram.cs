using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.View;
using AppPallet.ViewModel;
using AppPallet.ViewModels;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFontAwesomeIconFonts();
                });

            builder.Services.AddDbContext<PalletContex>(options =>
            options.UseSqlServer("Data Source=LUCAS\\SQLEXPRESS;Initial Catalog=Pallet;Persist Security Info=True;User ID=sa;Password=42559251;Trust Server Certificate=True"));


#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<IPopupService, PopupService>();

            // Registro de Controladores

            builder.Services.AddTransient<ChequeController>();

            // Registro de Vistas y ViewModels

            builder.Services.AddTransient<View.ChequeView>();
            builder.Services.AddTransient<ViewModel.ChequeViewModel>();

            builder.Services.AddTransientPopup<ChequeCrearView, ChequeCrearViewModel>();
            builder.Services.AddTransientPopup<ChequeModificarView, ChequeModificarViewModel>();



            return builder.Build();
        }
    }
}
