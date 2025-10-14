using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class HistorialHumedadView : ContentPage
{
	public HistorialHumedadView(HistorialHumedadViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}