using AppPallet.ViewModels;
using AppPallet.Views;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class PresupuestoMostrarView : ContentPage
{
	public PresupuestoMostrarView(PresupuestoMostrarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}