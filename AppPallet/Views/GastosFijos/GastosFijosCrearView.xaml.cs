using AppPallet.ViewModels;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class GastosFijosCrearView : ContentView
{
	public GastosFijosCrearView(GastosFijosCrearViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}