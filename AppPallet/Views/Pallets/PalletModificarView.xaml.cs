using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class PalletModificarView : ContentPage
{
	public PalletModificarView(PalletModificarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}