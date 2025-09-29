using AppPallet.ViewModels;
using UraniumUI.Pages;
namespace AppPallet.Views;

public partial class ChequeView : ContentPage
{
    public ChequeView(ChequeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ChequeViewModel vm)
        {
            await vm.CargarListaCheques();
        }
    }

}