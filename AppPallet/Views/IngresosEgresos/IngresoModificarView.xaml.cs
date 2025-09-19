using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class IngresoModificarView : ContentPage
{
	public IngresoModificarView(IngresoModificarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}