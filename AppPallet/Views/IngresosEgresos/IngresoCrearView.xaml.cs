using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class IngresoCrearView : ContentPage
{
	public IngresoCrearView(IngresoCrearViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}