using AppPallet.ViewModels;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class VentaView : UraniumContentPage
{
	public VentaView(VentaViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is VentaViewModel vm)
        {
            await vm.CargarListaVentas();
        }
    }


}