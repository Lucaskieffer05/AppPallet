using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class ActivoPasivoView : ContentPage
{
	public ActivoPasivoView(ActivoPasivoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ActivoPasivoViewModel vm)
        {
            await vm.CargarActivosPasivos();
        }
    }

}