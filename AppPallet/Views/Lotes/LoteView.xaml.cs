using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class LoteView : ContentPage
{
	public LoteView(LoteViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LoteViewModel vm)
        {
            await vm.CargarListaLotes();
        }
    }

}