using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class LoteModificarView : ContentPage
{
	public LoteModificarView(LoteModificarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}