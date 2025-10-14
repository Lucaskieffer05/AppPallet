using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class DashboardView : ContentPage
{
	public DashboardView(DashboardViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel vm)
        {
            await vm.CargarDashboard();
        }
    }

}