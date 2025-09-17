using AppPallet.ViewModels;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class IngresoEgresoView : UraniumContentPage
{
	public IngresoEgresoView(IngresoEgresoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}