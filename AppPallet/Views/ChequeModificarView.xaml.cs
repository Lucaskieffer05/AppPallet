using AppPallet.ViewModels;
using UraniumUI.Pages;
using UraniumUI.Views;

namespace AppPallet.Views;

public partial class ChequeModificarView : StatefulContentView
{
	public ChequeModificarView(ChequeModificarViewModel _chequeModificarViewModel)
	{
		InitializeComponent();
		BindingContext = _chequeModificarViewModel;
    }
}