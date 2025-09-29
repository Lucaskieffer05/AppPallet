using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class PedidoModificarView : ContentPage
{
	public PedidoModificarView(PedidoModificarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}