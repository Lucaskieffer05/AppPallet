using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class PedidoView : ContentPage
{
	public PedidoView(PedidoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}