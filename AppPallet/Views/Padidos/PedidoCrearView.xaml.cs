using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class PedidoCrearView : ContentPage
{
	public PedidoCrearView(PedidoCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}