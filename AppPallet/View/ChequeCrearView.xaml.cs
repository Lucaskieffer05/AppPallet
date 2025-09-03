using AppPallet.ViewModel;
using AppPallet.ViewModels;
using CommunityToolkit.Maui.Views;

namespace AppPallet.View;

public partial class ChequeCrearView : ContentView
{
    public ChequeCrearView(ChequeCrearViewModel _chequeCrearViewModel)
    {
        InitializeComponent();
        BindingContext = _chequeCrearViewModel;
    }
}