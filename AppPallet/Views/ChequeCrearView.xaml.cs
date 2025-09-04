using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class ChequeCrearView : ContentView
{
    public ChequeCrearView(ChequeCrearViewModel _chequeCrearViewModel)
    {
        InitializeComponent();
        BindingContext = _chequeCrearViewModel;
    }
}