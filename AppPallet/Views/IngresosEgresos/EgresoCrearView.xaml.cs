using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class EgresoCrearView : ContentPage
{
	public EgresoCrearView(EgresoCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}