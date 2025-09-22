using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class VentaModificarView : ContentPage
{
	public VentaModificarView(VentaModificarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}