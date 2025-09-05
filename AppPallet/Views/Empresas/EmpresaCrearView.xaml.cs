using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class EmpresaCrearView : ContentView
{
    public EmpresaCrearView(EmpresaCrearViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}