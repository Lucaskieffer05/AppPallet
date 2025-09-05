using AppPallet.ViewModels;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class EmpresaView : UraniumContentPage
{
    public EmpresaView(EmpresaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EmpresaViewModel vm)
        {
            await vm.CargarListaEmpresas();
        }
    }

}