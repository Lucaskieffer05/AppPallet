using AppPallet.ViewModels;
using System.Globalization;
using UraniumUI.Pages;

namespace AppPallet.Views;

public partial class CostoPorPalletCrearView : UraniumContentPage
{
	public CostoPorPalletCrearView(CostoPorPalletCrearViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CostoPorPalletCrearViewModel vm)
        {
            await vm.CargarListas();
        }
    }

    private void MontoEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null) return;

        // Elimina cualquier caracter que no sea dígito
        string digitsOnly = new string(entry.Text?.Where(char.IsDigit).ToArray());

        if (string.IsNullOrEmpty(digitsOnly))
        {
            entry.Text = "";
            return;
        }

        // Convierte a número y lo formatea con separador de miles
        if (long.TryParse(digitsOnly, out long value))
        {
            string formatted = value.ToString("N0", CultureInfo.CurrentCulture);
            if (entry.Text != formatted)
            {
                entry.Text = formatted;
                entry.CursorPosition = formatted.Length; // Mantiene el cursor al final
            }
        }
    }
}