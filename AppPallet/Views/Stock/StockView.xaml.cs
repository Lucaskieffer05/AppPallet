using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class StockView : ContentPage
{
	public StockView(StockViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StockViewModel vm)
        {
            await vm.LoadPallets();
        }
    }
}