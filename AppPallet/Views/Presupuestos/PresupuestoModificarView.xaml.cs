using AppPallet.ViewModels;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class PresupuestoModificarView : UraniumContentPage
{
	public PresupuestoModificarView(PresupuestoModificarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}