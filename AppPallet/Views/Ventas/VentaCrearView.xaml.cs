using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class VentaCrearView : ContentPage
{
	public VentaCrearView(VentaCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}