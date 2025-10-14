using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class PalletCrearView : ContentPage
{
	public PalletCrearView(PalletCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}