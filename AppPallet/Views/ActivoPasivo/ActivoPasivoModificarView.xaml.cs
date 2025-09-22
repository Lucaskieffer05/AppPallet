using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class ActivoPasivoModificarView : ContentPage
{
	public ActivoPasivoModificarView(ActivoPasivoModificarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}