using AppPallet.ViewModels;
using AppPallet.Views;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class PresupuestoMostrarView : UraniumContentPage
{
	public PresupuestoMostrarView(PresupuestoMostrarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}