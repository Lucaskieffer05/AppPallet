using AppPallet.ViewModels;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class GastosFijosView : UraniumContentPage
{
	public GastosFijosView(GastosFijosViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is GastosFijosViewModel vm)
        {
            await vm.CargarListaGastosFijos();
        }
    }


}