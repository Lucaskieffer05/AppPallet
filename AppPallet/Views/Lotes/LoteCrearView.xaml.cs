using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class LoteCrearView : ContentPage
{
	public LoteCrearView(LoteCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}