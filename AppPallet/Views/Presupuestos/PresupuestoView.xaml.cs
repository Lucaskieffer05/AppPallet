using AppPallet.ViewModels;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class PresupuestoView : ContentPage
{
	public PresupuestoView(PresupuestoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PresupuestoViewModel vm)
        {
            await vm.CargarListaEmpresas();
        }
    }
}