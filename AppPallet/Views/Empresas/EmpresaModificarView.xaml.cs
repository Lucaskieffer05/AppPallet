using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class EmpresaModificarView : ContentView
{
    public EmpresaModificarView(EmpresaModificarViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}