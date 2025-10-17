using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class LoteCrearView : ContentPage
{
	public LoteCrearView(LoteCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LoteCrearViewModel vm)
        {
            await vm.CargarProveedores();
        }
    }
}