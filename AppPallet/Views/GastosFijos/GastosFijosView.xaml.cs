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
}