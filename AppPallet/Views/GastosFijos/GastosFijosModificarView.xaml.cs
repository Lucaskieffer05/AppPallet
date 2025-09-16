using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class GastosFijosModificarView : ContentView
{
	public GastosFijosModificarView(GastosFijosModificarViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = viewModel;
	}
}