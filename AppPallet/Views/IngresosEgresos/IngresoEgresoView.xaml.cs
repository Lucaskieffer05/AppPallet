using AppPallet.ViewModels;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class IngresoEgresoView : ContentPage
{
	public IngresoEgresoView(IngresoEgresoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IngresoEgresoViewModel vm)
        {
            await vm.CargarListaIngresosEgresos();
        }
    }

}