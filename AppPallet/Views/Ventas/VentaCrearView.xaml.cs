using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class VentaCrearView : ContentPage
{
	public VentaCrearView(VentaCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is VentaCrearViewModel vm)
        {
            await vm.CargarListas();
        }
    }
}