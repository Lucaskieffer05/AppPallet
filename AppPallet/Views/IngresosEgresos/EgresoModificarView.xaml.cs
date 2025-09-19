using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class EgresoModificarView : ContentPage
{
	public EgresoModificarView(EgresoModificarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

    }
}