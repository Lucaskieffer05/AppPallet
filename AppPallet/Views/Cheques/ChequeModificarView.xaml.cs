using AppPallet.ViewModels;
using UraniumUI.Views;

namespace AppPallet.Views;

public partial class ChequeModificarView : ContentView
{
    public ChequeModificarView(ChequeModificarViewModel _chequeModificarViewModel)
    {
        InitializeComponent();
        BindingContext = _chequeModificarViewModel;
    }
}