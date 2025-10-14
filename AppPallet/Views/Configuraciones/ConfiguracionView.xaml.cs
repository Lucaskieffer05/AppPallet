using AppPallet.ViewModels;

namespace AppPallet.Views;

public partial class ConfiguracionView : ContentPage
{
	public ConfiguracionView(ConfiguracionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}