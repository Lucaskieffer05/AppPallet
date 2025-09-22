using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class ActivoPasivoCrearView : ContentPage
{
	public ActivoPasivoCrearView(ActivoPasivoCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}